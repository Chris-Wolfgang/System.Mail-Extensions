using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// Provides methods to parse EML (RFC 2822 MIME) content into <see cref="MailMessage"/> objects.
/// </summary>
/// <example>
/// <code>
/// using var message = EmlParser.Parse(File.ReadAllText("message.eml"));
/// Console.WriteLine(message.Subject);
/// </code>
/// </example>
// ReSharper disable once UnusedType.Global
public static class EmlParser
{

    /// <summary>
    /// Parses a raw EML/MIME string into a <see cref="MailMessage"/>.
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

        var headerEndIndex = FindHeaderBodySeparator(emlContent);
        var headerSection = headerEndIndex >= 0
            ? emlContent.Substring(0, headerEndIndex)
            : emlContent;
        var bodySection = headerEndIndex >= 0
            ? emlContent.Substring(headerEndIndex).TrimStart('\r', '\n')
            : string.Empty;

        var headers = ParseHeaders(headerSection);
        var message = new MailMessage();

        // From
        if (headers.TryGetValue("from", out var from) && !string.IsNullOrWhiteSpace(from))
        {
            try { message.From = ParseMailAddressSafe(from); }
            catch (FormatException) { /* Skip malformed From */ }
        }

        // To
        if (headers.TryGetValue("to", out var to))
        {
            AddAddresses(message.To, to);
        }

        // CC
        if (headers.TryGetValue("cc", out var cc))
        {
            AddAddresses(message.CC, cc);
        }

        // BCC
        if (headers.TryGetValue("bcc", out var bcc))
        {
            AddAddresses(message.Bcc, bcc);
        }

        // Reply-To
        if (headers.TryGetValue("reply-to", out var replyTo))
        {
            AddAddresses(message.ReplyToList, replyTo);
        }

        // Subject — decode RFC 2047 encoded words
        if (headers.TryGetValue("subject", out var subject))
        {
            message.Subject = DecodeEncodedWords(subject);
        }

        // Content-Type
        var contentType = headers.TryGetValue("content-type", out var ct) ? ct : "text/plain";
        var isMultipart = contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;

        // Content-Transfer-Encoding
        var transferEncoding = headers.TryGetValue("content-transfer-encoding", out var cte)
            ? cte.Trim().ToLowerInvariant()
            : "7bit";

        if (isMultipart)
        {
            var boundary = ExtractBoundary(contentType);
            if (boundary != null)
            {
                ParseMultipart(message, bodySection, boundary);
            }
        }
        else
        {
            // Simple single-part message
            var isHtml = contentType.IndexOf("text/html", StringComparison.OrdinalIgnoreCase) >= 0;
            message.IsBodyHtml = isHtml;
            message.Body = DecodeBody(bodySection, transferEncoding);
        }

        // Custom headers (skip standard ones)
        var standardHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "from", "to", "cc", "bcc", "reply-to", "subject",
            "content-type", "content-transfer-encoding", "mime-version",
            "date", "message-id"
        };

        foreach (var kvp in headers)
        {
            if (!standardHeaders.Contains(kvp.Key))
            {
                message.Headers[kvp.Key] = kvp.Value;
            }
        }

        // Priority
        if (headers.TryGetValue("x-priority", out var priority))
        {
            if (priority.Trim().StartsWith("1") || priority.Trim().StartsWith("2"))
            {
                message.Priority = MailPriority.High;
            }
            else if (priority.Trim().StartsWith("4") || priority.Trim().StartsWith("5"))
            {
                message.Priority = MailPriority.Low;
            }
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
        var content = File.ReadAllText(filePath, Encoding.UTF8);
        await Task.CompletedTask.ConfigureAwait(false);
#else
        var content = await File.ReadAllTextAsync(filePath, Encoding.UTF8, cancellationToken)
            .ConfigureAwait(false);
#endif

        return Parse(content);
    }



    // ==========================================================================
    // Private helpers
    // ==========================================================================

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
        var match = Regex.Match(contentType, @"boundary=""?([^"";\s]+)""?", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value : null;
    }



    private static void ParseMultipart
    (
        MailMessage message,
        string body,
        string boundary
    )
    {
        var delimiter = "--" + boundary;
        var parts = body.Split(new[] { delimiter }, StringSplitOptions.None);

        foreach (var rawPart in parts)
        {
            var part = rawPart.Trim('\r', '\n');

            // Skip empty parts and closing boundary
            if (string.IsNullOrWhiteSpace(part) || part.StartsWith("--"))
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

            // Check for nested multipart
            if (partContentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var nestedBoundary = ExtractBoundary(partContentType);
                if (nestedBoundary != null)
                {
                    ParseMultipart(message, partBody, nestedBoundary);
                }
                continue;
            }

            var isTextPlain = partContentType.IndexOf("text/plain", StringComparison.OrdinalIgnoreCase) >= 0;
            var isTextHtml = partContentType.IndexOf("text/html", StringComparison.OrdinalIgnoreCase) >= 0;

            var contentDisposition = partHeaders.TryGetValue("content-disposition", out var cd)
                ? cd : null;
            var isAttachment = contentDisposition != null &&
                contentDisposition.IndexOf("attachment", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isAttachment || (!isTextPlain && !isTextHtml))
            {
                // Treat as attachment
                var fileName = ExtractFileName(contentDisposition, partContentType) ?? "attachment.bin";
                var attachmentBytes = DecodeBodyBytes(partBody, partTransferEncoding);
                var stream = new MemoryStream(attachmentBytes, writable: false);
                var attachment = new Attachment(stream, fileName);

                if (partHeaders.TryGetValue("content-id", out var cid))
                {
                    attachment.ContentId = cid.Trim('<', '>', ' ');
                }

                message.Attachments.Add(attachment);
            }
            else if (isTextHtml)
            {
                if (string.IsNullOrEmpty(message.Body))
                {
                    message.Body = DecodeBody(partBody, partTransferEncoding);
                    message.IsBodyHtml = true;
                }
                else
                {
                    // Already have a body — add as alternate view
                    var decodedHtml = DecodeBody(partBody, partTransferEncoding);
                    var view = AlternateView.CreateAlternateViewFromString
                    (
                        decodedHtml,
                        Encoding.UTF8,
                        MediaTypeNames.Text.Html
                    );
                    message.AlternateViews.Add(view);
                }
            }
            else if (isTextPlain)
            {
                if (string.IsNullOrEmpty(message.Body))
                {
                    message.Body = DecodeBody(partBody, partTransferEncoding);
                    message.IsBodyHtml = false;
                }
            }
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
            var match = Regex.Match(contentDisposition, @"filename=""?([^"";\s]+)""?", RegexOptions.IgnoreCase);
            if (match.Success) return match.Groups[1].Value;
        }

        var nameMatch = Regex.Match(contentType, @"name=""?([^"";\s]+)""?", RegexOptions.IgnoreCase);
        return nameMatch.Success ? nameMatch.Groups[1].Value : null;
    }



    private static string DecodeBody
    (
        string body,
        string transferEncoding
    )
    {
        switch (transferEncoding)
        {
            case "base64":
                try
                {
                    var cleaned = Regex.Replace(body, @"\s+", "");
                    var bytes = Convert.FromBase64String(cleaned);
                    return Encoding.UTF8.GetString(bytes);
                }
                catch (FormatException)
                {
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
        string transferEncoding
    )
    {
        switch (transferEncoding)
        {
            case "base64":
                try
                {
                    var cleaned = Regex.Replace(body, @"\s+", "");
                    return Convert.FromBase64String(cleaned);
                }
                catch (FormatException)
                {
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
        // Remove soft line breaks (= at end of line)
        var cleaned = Regex.Replace(input, @"=\r?\n", "");

        // Collect bytes for proper multi-byte UTF-8 decoding
        var result = new List<byte>();

        for (var i = 0; i < cleaned.Length; i++)
        {
            if (cleaned[i] == '=' && i + 2 < cleaned.Length
                && IsHexChar(cleaned[i + 1]) && IsHexChar(cleaned[i + 2]))
            {
                result.Add(Convert.ToByte(cleaned.Substring(i + 1, 2), 16));
                i += 2;
            }
            else
            {
                // Flush any pending bytes, then add the literal char
                var ch = cleaned[i];
                var charBytes = Encoding.UTF8.GetBytes(new[] { ch });
                foreach (var b in charBytes)
                {
                    result.Add(b);
                }
            }
        }

        return Encoding.UTF8.GetString(result.ToArray());
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
        string input
    )
    {
        // RFC 2047: =?charset?encoding?encoded-text?=
        return Regex.Replace
        (
            input,
            @"=\?([^?]+)\?([BbQq])\?([^?]+)\?=",
            m =>
            {
                var charset = m.Groups[1].Value;
                var encoding = m.Groups[2].Value.ToUpperInvariant();
                var encodedText = m.Groups[3].Value;

                try
                {
                    var enc = Encoding.GetEncoding(charset);

                    if (encoding == "B")
                    {
                        var bytes = Convert.FromBase64String(encodedText);
                        return enc.GetString(bytes);
                    }
                    else // Q encoding
                    {
                        var decoded = encodedText
                            .Replace('_', ' ');
                        decoded = Regex.Replace
                        (
                            decoded,
                            @"=([0-9A-Fa-f]{2})",
                            hex => ((char)Convert.ToByte(hex.Groups[1].Value, 16)).ToString()
                        );
                        return decoded;
                    }
                }
                catch
                {
                    return m.Value;
                }
            }
        );
    }



    private static MailAddress ParseMailAddressSafe
    (
        string addressString
    )
    {
        // Handle "Display Name" <email@example.com> format
        var match = Regex.Match(addressString.Trim(), @"^""?([^""<]+?)""?\s*<([^>]+)>$");
        if (match.Success)
        {
            return new MailAddress(match.Groups[2].Value.Trim(), match.Groups[1].Value.Trim());
        }

        // Handle bare <email@example.com>
        var angleBracket = Regex.Match(addressString.Trim(), @"^<([^>]+)>$");
        if (angleBracket.Success)
        {
            return new MailAddress(angleBracket.Groups[1].Value.Trim());
        }

        return new MailAddress(addressString.Trim());
    }



    private static void AddAddresses
    (
        MailAddressCollection collection,
        string addressList
    )
    {
        // Split on comma, handling quoted display names
        var addresses = SplitAddresses(addressList);

        foreach (var addr in addresses)
        {
            var trimmed = addr.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;

            try
            {
                collection.Add(ParseMailAddressSafe(trimmed));
            }
            catch (FormatException)
            {
                // Skip malformed addresses
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
}
