using System;
using System.Collections.Generic;
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;       // [NotNullWhen(true)] on TFMs that ship it
#endif
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Extensions.Mail.Validation;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// Provides methods to parse EML (RFC 2822 MIME) content into <see cref="MailMessage"/> objects.
/// </summary>
/// <remarks>
/// Parsing is deliberately lenient, because real-world EML files routinely
/// contain malformed headers: an address that cannot be parsed is skipped
/// rather than throwing, and a malformed <c>From</c> header leaves
/// <see cref="MailMessage.From"/> <c>null</c>. To detect what a lenient
/// parse dropped, pair with
/// <see cref="MailMessageExtensions.Validate(MailMessage)"/>, which reports
/// a missing sender or recipients as structured issues.
/// </remarks>
/// <example>
/// <code>
/// using var message = await EmlParser.ParseFileAsync("message.eml");
/// Console.WriteLine(message.Subject);
/// </code>
/// </example>
// ReSharper disable once UnusedType.Global
public static class EmlParser
{

    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);

    private const RegexOptions DefaultRegexOptions =
        RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

    private const RegexOptions DefaultRegexOptionsNoCase =
        RegexOptions.ExplicitCapture;

    // Patterns are hoisted to static compiled instances: parsing a single
    // message hits several of these once per header line or MIME part, and
    // the static Regex.Match/Replace helpers only cache a limited number of
    // interpreted patterns.
    private static readonly Regex BoundaryRegex = new Regex
    (
        @"boundary=""?(?<boundary>[^"";\s]+)""?",
        DefaultRegexOptions | RegexOptions.Compiled,
        RegexTimeout
    );

    private static readonly Regex FileNameRegex = new Regex
    (
        @"filename=""?(?<filename>[^"";\s]+)""?",
        DefaultRegexOptions | RegexOptions.Compiled,
        RegexTimeout
    );

    private static readonly Regex NameRegex = new Regex
    (
        @"name=""?(?<name>[^"";\s]+)""?",
        DefaultRegexOptions | RegexOptions.Compiled,
        RegexTimeout
    );

    private static readonly Regex WhitespaceRegex = new Regex
    (
        @"\s+",
        DefaultRegexOptionsNoCase | RegexOptions.Compiled,
        RegexTimeout
    );

    private static readonly Regex EncodedWordRegex = new Regex
    (
        @"=\?(?<charset>[^?]+)\?(?<encoding>[BbQq])\?(?<text>[^?]+)\?=",
        DefaultRegexOptionsNoCase | RegexOptions.Compiled,
        RegexTimeout
    );

    private static readonly Regex DisplayNameAddressRegex = new Regex
    (
        @"^""?(?<name>[^""<]+?)""?\s*<(?<email>[^>]+)>$",
        DefaultRegexOptionsNoCase | RegexOptions.Compiled,
        RegexTimeout
    );

    private static readonly Regex AngleBracketAddressRegex = new Regex
    (
        @"^<(?<email>[^>]+)>$",
        DefaultRegexOptionsNoCase | RegexOptions.Compiled,
        RegexTimeout
    );

    private static readonly HashSet<string> StandardHeaderNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "from", "to", "cc", "bcc", "reply-to", "subject",
        "content-type", "content-transfer-encoding", "mime-version",
        "date", "message-id"
    };



    /// <summary>
    /// Parses a raw EML/MIME string into a <see cref="MailMessage"/> using the
    /// default lenient behavior (malformed constructs are skipped).
    /// </summary>
    /// <param name="emlContent">The EML content string.</param>
    /// <returns>A new <see cref="MailMessage"/> populated from the EML content.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="emlContent"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public static MailMessage Parse
    (
        string emlContent
    )
    {
        if (emlContent == null)
        {
            throw new ArgumentNullException(nameof(emlContent));
        }

        return ParseCore(emlContent, new ParseContext(new EmlParserOptions()));
    }



    /// <summary>
    /// Parses a raw EML/MIME string into a <see cref="MailMessage"/> using the
    /// supplied <paramref name="options"/>.
    /// </summary>
    /// <param name="emlContent">The EML content string.</param>
    /// <param name="options">Parsing options. Set <see cref="EmlParserOptions.Strict"/> to reject malformed input.</param>
    /// <returns>A new <see cref="MailMessage"/> populated from the EML content.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="emlContent"/> or <paramref name="options"/> is null.</exception>
    /// <exception cref="EmlParseException">A malformed construct was encountered and <see cref="EmlParserOptions.Strict"/> is set.</exception>
    // ReSharper disable once UnusedMember.Global
    public static MailMessage Parse
    (
        string emlContent,
        EmlParserOptions options
    )
    {
        if (emlContent == null)
        {
            throw new ArgumentNullException(nameof(emlContent));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return ParseCore(emlContent, new ParseContext(options));
    }



    /// <summary>
    /// Parses a raw EML/MIME string and returns both the message and the list of
    /// malformed constructs the lenient parser skipped.
    /// </summary>
    /// <param name="emlContent">The EML content string.</param>
    /// <param name="options">Optional parsing options. When <see cref="EmlParserOptions.Strict"/> is set, a malformed construct throws instead of being reported.</param>
    /// <returns>A <see cref="ParseResult"/> containing the message and any diagnostics.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="emlContent"/> is null.</exception>
    /// <exception cref="EmlParseException">A malformed construct was encountered and <see cref="EmlParserOptions.Strict"/> is set.</exception>
    // ReSharper disable once UnusedMember.Global
    public static ParseResult ParseWithDiagnostics
    (
        string emlContent,
        EmlParserOptions? options = null
    )
    {
        if (emlContent == null)
        {
            throw new ArgumentNullException(nameof(emlContent));
        }

        var context = new ParseContext(options ?? new EmlParserOptions());
        var message = ParseCore(emlContent, context);
        return new ParseResult(message, context.Issues);
    }



    private static MailMessage ParseCore
    (
        string emlContent,
        ParseContext context
    )
    {
        var headerEndIndex = FindHeaderBodySeparator(emlContent);
        var headerSection = headerEndIndex >= 0
            ? emlContent.Substring(0, headerEndIndex)
            : emlContent;
        var bodySection = headerEndIndex >= 0
            ? emlContent.Substring(headerEndIndex).TrimStart('\r', '\n')
            : string.Empty;

        var headers = ParseHeaders(headerSection);
        var message = new MailMessage();

        // In strict mode any Apply* step can throw EmlParseException partway
        // through — potentially after ApplyBodyOrMultipart has already attached
        // Attachments / AlternateViews that own MemoryStreams. Since the partial
        // message is never returned, the caller can't dispose it, so dispose it
        // here before rethrowing to avoid leaking those streams.
        try
        {
            ApplyAddressHeaders(message, headers, context);
            ApplySubject(message, headers, context);
            ApplyBodyOrMultipart(message, headers, bodySection, context);
            ApplyCustomHeaders(message, headers);
            ApplyPriority(message, headers);
        }
        catch
        {
            message.Dispose();
            throw;
        }

        return message;
    }



    /// <summary>
    /// Parses an EML file into a <see cref="MailMessage"/>.
    /// </summary>
    /// <param name="filePath">The path to the EML file.</param>
    /// <returns>A new <see cref="MailMessage"/> populated from the file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public static MailMessage ParseFile
    (
        string filePath
    )
    {
        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

#pragma warning disable RS0030 // File I/O - reading EML file
        var content = File.ReadAllText(filePath, Encoding.UTF8);
#pragma warning restore RS0030

        return Parse(content);
    }



    /// <summary>
    /// Parses an EML file into a <see cref="MailMessage"/> using the supplied
    /// <paramref name="options"/>.
    /// </summary>
    /// <param name="filePath">The path to the EML file.</param>
    /// <param name="options">Parsing options. Set <see cref="EmlParserOptions.Strict"/> to reject malformed input.</param>
    /// <returns>A new <see cref="MailMessage"/> populated from the file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> or <paramref name="options"/> is null.</exception>
    /// <exception cref="EmlParseException">A malformed construct was encountered and <see cref="EmlParserOptions.Strict"/> is set.</exception>
    // ReSharper disable once UnusedMember.Global
    public static MailMessage ParseFile
    (
        string filePath,
        EmlParserOptions options
    )
    {
        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

#pragma warning disable RS0030 // File I/O - reading EML file
        var content = File.ReadAllText(filePath, Encoding.UTF8);
#pragma warning restore RS0030

        return Parse(content, options);
    }



    /// <summary>
    /// Asynchronously parses an EML file into a <see cref="MailMessage"/>.
    /// </summary>
    /// <param name="filePath">The path to the EML file.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task containing the parsed <see cref="MailMessage"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public static async Task<MailMessage> ParseFileAsync
    (
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

#if NETSTANDARD2_0 || NET462
        cancellationToken.ThrowIfCancellationRequested();
#pragma warning disable RS0030
        var content = File.ReadAllText(filePath, Encoding.UTF8);
#pragma warning restore RS0030
        await Task.CompletedTask.ConfigureAwait(false);
#else
        var content = await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellationToken)
            .ConfigureAwait(false);
#endif

        return Parse(content);
    }



    /// <summary>
    /// Asynchronously parses an EML file into a <see cref="MailMessage"/> using
    /// the supplied <paramref name="options"/>.
    /// </summary>
    /// <param name="filePath">The path to the EML file.</param>
    /// <param name="options">Parsing options. Set <see cref="EmlParserOptions.Strict"/> to reject malformed input.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task containing the parsed <see cref="MailMessage"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> or <paramref name="options"/> is null.</exception>
    /// <exception cref="EmlParseException">A malformed construct was encountered and <see cref="EmlParserOptions.Strict"/> is set.</exception>
    // ReSharper disable once UnusedMember.Global
    public static async Task<MailMessage> ParseFileAsync
    (
        string filePath,
        EmlParserOptions options,
        CancellationToken cancellationToken = default
    )
    {
        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

#if NETSTANDARD2_0 || NET462
        cancellationToken.ThrowIfCancellationRequested();
#pragma warning disable RS0030
        var content = File.ReadAllText(filePath, Encoding.UTF8);
#pragma warning restore RS0030
        await Task.CompletedTask.ConfigureAwait(false);
#else
        var content = await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellationToken)
            .ConfigureAwait(false);
#endif

        return Parse(content, options);
    }



    // ==========================================================================
    // Parse helpers
    // ==========================================================================

    private static void ApplyAddressHeaders
    (
        MailMessage message,
        Dictionary<string, string> headers,
        ParseContext context
    )
    {
        if (headers.TryGetValue("from", out var from) && !string.IsNullOrWhiteSpace(from))
        {
            if (TryParseMailAddress(from, out var fromAddress))
            {
                message.From = fromAddress;
            }
            else
            {
                context.Report($"Malformed From address: '{from.Trim()}'.", "From");
            }
        }

        if (headers.TryGetValue("to", out var to))
        {
            AddAddresses(message.To, to, context, "To");
        }

        if (headers.TryGetValue("cc", out var cc))
        {
            AddAddresses(message.CC, cc, context, "CC");
        }

        if (headers.TryGetValue("bcc", out var bcc))
        {
            AddAddresses(message.Bcc, bcc, context, "Bcc");
        }

        if (headers.TryGetValue("reply-to", out var replyTo))
        {
            AddAddresses(message.ReplyToList, replyTo, context, "ReplyTo");
        }
    }



    private static void ApplySubject
    (
        MailMessage message,
        Dictionary<string, string> headers,
        ParseContext context
    )
    {
        if (headers.TryGetValue("subject", out var subject))
        {
            message.Subject = DecodeEncodedWords(subject, context);
        }
    }



    private static void ApplyBodyOrMultipart
    (
        MailMessage message,
        Dictionary<string, string> headers,
        string bodySection,
        ParseContext context
    )
    {
        var contentType = headers.TryGetValue("content-type", out var ct) ? ct : "text/plain";
        var isMultipart = contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        var transferEncoding = headers.TryGetValue("content-transfer-encoding", out var cte)
            ? cte.Trim().ToLowerInvariant()
            : "7bit";

        if (isMultipart)
        {
            var boundary = ExtractBoundary(contentType);
            if (boundary != null)
            {
                ParseMultipart(message, bodySection, boundary, context);
            }
        }
        else
        {
            var isHtml = contentType.IndexOf("text/html", StringComparison.OrdinalIgnoreCase) >= 0;
            message.IsBodyHtml = isHtml;
            message.Body = TrimFinalLineBreak(DecodeBody(bodySection, transferEncoding, context));
        }
    }



    /// <summary>
    /// Removes exactly one trailing CRLF (or bare LF) from a decoded body.
    /// MIME writers terminate the final body line with a line break that is
    /// part of the wire format, not the content; keeping it made every
    /// serialize→parse round trip grow the body by one blank line.
    /// </summary>
    private static string TrimFinalLineBreak
    (
        string body
    )
    {
        if (body.Length >= 2 && body[body.Length - 2] == '\r' && body[body.Length - 1] == '\n')
        {
            return body.Substring(0, body.Length - 2);
        }

        if (body.Length >= 1 && body[body.Length - 1] == '\n')
        {
            return body.Substring(0, body.Length - 1);
        }

        return body;
    }



    private static void ApplyCustomHeaders
    (
        MailMessage message,
        Dictionary<string, string> headers
    )
    {
        var customHeaders = headers.Where(kvp => !StandardHeaderNames.Contains(kvp.Key));

        foreach (var kvp in customHeaders)
        {
            message.Headers[kvp.Key] = kvp.Value;
        }
    }



    private static void ApplyPriority
    (
        MailMessage message,
        Dictionary<string, string> headers
    )
    {
        if (!headers.TryGetValue("x-priority", out var priority))
        {
            return;
        }

        var trimmed = priority.Trim();
        if (trimmed.Length == 0)
        {
            return;
        }

        var firstChar = trimmed[0];
        if (firstChar == '1' || firstChar == '2')
        {
            message.Priority = MailPriority.High;
        }
        else if (firstChar == '4' || firstChar == '5')
        {
            message.Priority = MailPriority.Low;
        }
    }



    private static int FindHeaderBodySeparator
    (
        string content
    )
    {
        // Headers and body are separated by a blank line (\r\n\r\n or \n\n)
        var idx = content.IndexOf("\r\n\r\n", StringComparison.Ordinal);
        if (idx >= 0) return idx + 4;

        idx = content.IndexOf("\n\n", StringComparison.Ordinal);
        if (idx >= 0) return idx + 2;

        return -1;
    }



    private static Dictionary<string, string> ParseHeaders
    (
        string headerSection
    )
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string? currentKey = null;
        var currentValue = new StringBuilder();

        foreach (var rawLine in headerSection.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
        {
            if (rawLine.Length > 0 && (rawLine[0] == ' ' || rawLine[0] == '\t'))
            {
                // Continuation of previous header (folded)
                if (currentKey != null)
                {
                    currentValue.Append(' ');
                    currentValue.Append(rawLine.Trim());
                }
            }
            else
            {
                // Save previous header
                if (currentKey != null)
                {
                    headers[currentKey] = currentValue.ToString();
                }

                // Parse new header
                var colonIndex = rawLine.IndexOf(':');
                if (colonIndex > 0)
                {
                    currentKey = rawLine.Substring(0, colonIndex).Trim();
                    currentValue = new StringBuilder(rawLine.Substring(colonIndex + 1).Trim());
                }
                else
                {
                    currentKey = null;
                }
            }
        }

        // Save last header
        if (currentKey != null)
        {
            headers[currentKey] = currentValue.ToString();
        }

        return headers;
    }



    private static string? ExtractBoundary
    (
        string contentType
    )
    {
        var match = BoundaryRegex.Match(contentType);
        return match.Success ? match.Groups["boundary"].Value : null;
    }



    private static void ParseMultipart
    (
        MailMessage message,
        string body,
        string boundary,
        ParseContext context
    )
    {
        var delimiter = "--" + boundary;
        var parts = body.Split(new[] { delimiter }, StringSplitOptions.None);

        foreach (var rawPart in parts)
        {
            var part = rawPart.Trim('\r', '\n');

            if (string.IsNullOrWhiteSpace(part) || part.StartsWith("--", StringComparison.Ordinal))
            {
                continue;
            }

            var partHeaderEnd = FindHeaderBodySeparator(part);
            if (partHeaderEnd < 0) continue;

            var partHeaders = ParseHeaders(part.Substring(0, partHeaderEnd));
            var partBody = part.Substring(partHeaderEnd).TrimStart('\r', '\n');

            var partContentType = partHeaders.TryGetValue("content-type", out var pct)
                ? pct : "text/plain";
            var partTransferEncoding = partHeaders.TryGetValue("content-transfer-encoding", out var pcte)
                ? pcte.Trim().ToLowerInvariant() : "7bit";

            if (TryHandleNestedMultipart(message, partContentType, partBody, context))
            {
                continue;
            }

            ProcessSinglePart
            (
                message,
                partHeaders,
                partContentType,
                partTransferEncoding,
                partBody,
                context
            );
        }
    }



    private static bool TryHandleNestedMultipart
    (
        MailMessage message,
        string partContentType,
        string partBody,
        ParseContext context
    )
    {
        if (partContentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) < 0)
        {
            return false;
        }

        var nestedBoundary = ExtractBoundary(partContentType);
        if (nestedBoundary != null)
        {
            ParseMultipart(message, partBody, nestedBoundary, context);
        }

        return true;
    }



    private static void ProcessSinglePart
    (
        MailMessage message,
        Dictionary<string, string> partHeaders,
        string partContentType,
        string partTransferEncoding,
        string partBody,
        ParseContext context
    )
    {
        var isTextPlain = partContentType.IndexOf("text/plain", StringComparison.OrdinalIgnoreCase) >= 0;
        var isTextHtml = partContentType.IndexOf("text/html", StringComparison.OrdinalIgnoreCase) >= 0;

        var contentDisposition = partHeaders.TryGetValue("content-disposition", out var cd)
            ? cd : null;
        var isAttachment = contentDisposition != null &&
            contentDisposition.IndexOf("attachment", StringComparison.OrdinalIgnoreCase) >= 0;

        if (isAttachment || (!isTextPlain && !isTextHtml))
        {
            AddAttachmentPart(message, partHeaders, partContentType, partTransferEncoding, partBody, contentDisposition, context);
        }
        else if (isTextHtml)
        {
            AddHtmlPart(message, partBody, partTransferEncoding, context);
        }
        else if (isTextPlain && string.IsNullOrEmpty(message.Body))
        {
            message.Body = DecodeBody(partBody, partTransferEncoding, context);
            message.IsBodyHtml = false;
        }
    }



    private static void AddAttachmentPart
    (
        MailMessage message,
        Dictionary<string, string> partHeaders,
        string partContentType,
        string partTransferEncoding,
        string partBody,
        string? contentDisposition,
        ParseContext context
    )
    {
        var fileName = ExtractFileName(contentDisposition, partContentType) ?? "attachment.bin";
        var attachmentBytes = DecodeBodyBytes(partBody, partTransferEncoding, context);
        var stream = new MemoryStream(attachmentBytes, writable: false);
        var mimeType = partContentType.Split(';')[0].Trim();
        var attachment = new Attachment(stream, fileName, mimeType);

        if (partHeaders.TryGetValue("content-id", out var cid))
        {
            attachment.ContentId = cid.Trim('<', '>', ' ');
        }

        message.Attachments.Add(attachment);
    }



    private static void AddHtmlPart
    (
        MailMessage message,
        string partBody,
        string partTransferEncoding,
        ParseContext context
    )
    {
        if (string.IsNullOrEmpty(message.Body))
        {
            message.Body = DecodeBody(partBody, partTransferEncoding, context);
            message.IsBodyHtml = true;
        }
        else
        {
            var decodedHtml = DecodeBody(partBody, partTransferEncoding, context);
            var view = AlternateView.CreateAlternateViewFromString
            (
                decodedHtml,
                Encoding.UTF8,
                MediaTypeNames.Text.Html
            );
            message.AlternateViews.Add(view);
        }
    }



    private static string? ExtractFileName
    (
        string? contentDisposition,
        string contentType
    )
    {
        if (contentDisposition != null)
        {
            var match = FileNameRegex.Match(contentDisposition);
            if (match.Success) return match.Groups["filename"].Value;
        }

        var nameMatch = NameRegex.Match(contentType);
        return nameMatch.Success ? nameMatch.Groups["name"].Value : null;
    }



    private static string DecodeBody
    (
        string body,
        string transferEncoding,
        ParseContext context
    )
    {
        switch (transferEncoding)
        {
            case "base64":
                try
                {
                    var cleaned = WhitespaceRegex.Replace(body, "");
                    var bytes = Convert.FromBase64String(cleaned);
                    return Encoding.UTF8.GetString(bytes);
                }
                catch (FormatException)
                {
                    context.Report("Undecodable base64 body part; kept as raw text.", "Body");
                    return body;
                }

            case "quoted-printable":
                return DecodeQuotedPrintable(body);

            default:
                return body;
        }
    }



    private static byte[] DecodeBodyBytes
    (
        string body,
        string transferEncoding,
        ParseContext context
    )
    {
        switch (transferEncoding)
        {
            case "base64":
                try
                {
                    var cleaned = WhitespaceRegex.Replace(body, "");
                    return Convert.FromBase64String(cleaned);
                }
                catch (FormatException)
                {
                    context.Report("Undecodable base64 attachment part; kept as raw bytes.", "Attachments");
                    return Encoding.UTF8.GetBytes(body);
                }

            case "quoted-printable":
                return Encoding.UTF8.GetBytes(DecodeQuotedPrintable(body));

            default:
                return Encoding.UTF8.GetBytes(body);
        }
    }



    private static string DecodeQuotedPrintable
    (
        string input
    )
    {
        // Single pass: soft line breaks (=\r\n or =\n) are skipped inline and
        // =XX escapes are written as raw bytes, so multi-byte UTF-8 sequences
        // split across escapes still decode correctly. The buffer is sized
        // for the UTF-8 worst case (3 bytes per UTF-16 code unit) so it never
        // needs to grow.
        // checked: a >715 MB input would overflow int sizing; fail explicitly
        // rather than allocate a wrong-sized buffer.
        var buffer = new byte[checked(input.Length * 3)];
        var byteCount = 0;
        var i = 0;

        while (i < input.Length)
        {
            var current = input[i];

            if (current == '=')
            {
                if (i + 2 < input.Length && IsHexChar(input[i + 1]) && IsHexChar(input[i + 2]))
                {
                    buffer[byteCount++] = (byte)((HexValue(input[i + 1]) << 4) | HexValue(input[i + 2]));
                    i += 3;
                    continue;
                }

                if (i + 1 < input.Length && input[i + 1] == '\n')
                {
                    i += 2;
                    continue;
                }

                if (i + 2 < input.Length && input[i + 1] == '\r' && input[i + 2] == '\n')
                {
                    i += 3;
                    continue;
                }
            }

            if (current < 0x80)
            {
                buffer[byteCount++] = (byte)current;
                i++;
            }
            else
            {
                // Only a valid surrogate pair consumes two chars; a lone high
                // surrogate must not swallow the next character (it could be
                // an '=' starting an escape or soft break).
                var charCount = i + 1 < input.Length && char.IsSurrogatePair(input, i) ? 2 : 1;
                byteCount += Encoding.UTF8.GetBytes(input, i, charCount, buffer, byteCount);
                i += charCount;
            }
        }

        return Encoding.UTF8.GetString(buffer, 0, byteCount);
    }



    private static int HexValue
    (
        char c
    )
    {
        if (c <= '9')
        {
            return c - '0';
        }

        return (c <= 'F' ? c - 'A' : c - 'a') + 10;
    }



    private static bool IsHexChar
    (
        char c
    )
    {
        return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
    }



    private static string DecodeEncodedWords
    (
        string input,
        ParseContext context
    )
    {
        // RFC 2047: =?charset?encoding?encoded-text?=
        return EncodedWordRegex.Replace(input, m => DecodeEncodedWordMatch(m, context));
    }



    private static string DecodeEncodedWordMatch
    (
        Match m,
        ParseContext context
    )
    {
        var charset = m.Groups["charset"].Value;
        var encoding = m.Groups["encoding"].Value.ToUpperInvariant();
        var encodedText = m.Groups["text"].Value;

        try
        {
            var enc = Encoding.GetEncoding(charset);

            if (string.Equals(encoding, "B", StringComparison.Ordinal))
            {
                var bytes = Convert.FromBase64String(encodedText);
                return enc.GetString(bytes);
            }

            return DecodeQEncoding(encodedText, enc);
        }
        catch (FormatException)
        {
            context.Report($"Malformed RFC 2047 encoded word: '{m.Value}'; kept verbatim.", propertyName: null);
            return m.Value;
        }
        catch (ArgumentException)
        {
            context.Report($"Malformed RFC 2047 encoded word: '{m.Value}'; kept verbatim.", propertyName: null);
            return m.Value;
        }
    }



    private static string DecodeQEncoding
    (
        string encodedText,
        Encoding enc
    )
    {
        var qText = encodedText.Replace('_', ' ');
        var byteList = new List<byte>();
        var i = 0;

        while (i < qText.Length)
        {
            if (qText[i] == '=' && i + 2 < qText.Length
                && IsHexChar(qText[i + 1]) && IsHexChar(qText[i + 2]))
            {
                byteList.Add(Convert.ToByte(qText.Substring(i + 1, 2), 16));
                i += 3;
            }
            else
            {
                foreach (var b in enc.GetBytes(new[] { qText[i] }))
                {
                    byteList.Add(b);
                }

                i++;
            }
        }

        return enc.GetString(byteList.ToArray());
    }



    /// <summary>
    /// Attempts to parse a header address token in any of the common forms
    /// ("Display Name" &lt;email&gt;, bare &lt;email&gt;, or a plain address).
    /// Returns <c>false</c> for malformed input instead of throwing — the
    /// parser is deliberately lenient and skips addresses it cannot read.
    /// </summary>
    private static bool TryParseMailAddress
    (
        string addressString,
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        [NotNullWhen(true)]
#endif
        out MailAddress? result
    )
    {
        try
        {
            result = ParseMailAddress(addressString);
            return true;
        }
        catch (FormatException)
        {
            result = null;
            return false;
        }
    }



    private static MailAddress ParseMailAddress
    (
        string addressString
    )
    {
        var trimmed = addressString.Trim();

        // Handle "Display Name" <email@example.com> format
        var match = DisplayNameAddressRegex.Match(trimmed);
        if (match.Success)
        {
            return new MailAddress(match.Groups["email"].Value.Trim(), match.Groups["name"].Value.Trim());
        }

        // Handle bare <email@example.com>
        var angleBracket = AngleBracketAddressRegex.Match(trimmed);
        if (angleBracket.Success)
        {
            return new MailAddress(angleBracket.Groups["email"].Value.Trim());
        }

        return new MailAddress(trimmed);
    }



    private static void AddAddresses
    (
        MailAddressCollection collection,
        string addressList,
        ParseContext context,
        string propertyName
    )
    {
        // Split on comma, handling quoted display names
        var addresses = SplitAddresses(addressList);

        foreach (var addr in addresses)
        {
            var trimmed = addr.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;

            if (TryParseMailAddress(trimmed, out var parsed))
            {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
                collection.Add(parsed);
#else
                collection.Add(parsed!);
#endif
            }
            else
            {
                context.Report($"Malformed {propertyName} address: '{trimmed}'.", propertyName);
            }
        }
    }



    private static IEnumerable<string> SplitAddresses
    (
        string addressList
    )
    {
        var result = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;
        var inAngleBrackets = false;

        foreach (var ch in addressList)
        {
            if (ch == '"')
            {
                inQuotes = !inQuotes;
                current.Append(ch);
            }
            else if (ch == '<' && !inQuotes)
            {
                inAngleBrackets = true;
                current.Append(ch);
            }
            else if (ch == '>' && !inQuotes)
            {
                inAngleBrackets = false;
                current.Append(ch);
            }
            else if (ch == ',' && !inQuotes && !inAngleBrackets)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(ch);
            }
        }

        if (current.Length > 0)
        {
            result.Add(current.ToString());
        }

        return result;
    }



    /// <summary>
    /// Carries the parse options and accumulates diagnostics as the parser
    /// walks the message. A malformed construct is recorded as a
    /// <see cref="ValidationIssue"/>; when <see cref="EmlParserOptions.Strict"/>
    /// is set the same construct throws an <see cref="EmlParseException"/>.
    /// </summary>
    private sealed class ParseContext
    {

        internal ParseContext
        (
            EmlParserOptions options
        )
        {
            Options = options;
        }



        internal EmlParserOptions Options { get; }



        internal List<ValidationIssue> Issues { get; } = new List<ValidationIssue>();



        internal void Report
        (
            string message,
            string? propertyName
        )
        {
            Issues.Add(new ValidationIssue(ValidationSeverity.Warning, message, propertyName));

            if (Options.Strict)
            {
                throw new EmlParseException(message);
            }
        }
    }
}
