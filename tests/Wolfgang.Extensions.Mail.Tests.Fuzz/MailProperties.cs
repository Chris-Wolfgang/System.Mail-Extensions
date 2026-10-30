using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CsCheck;
using Xunit;

namespace Wolfgang.Extensions.Mail.Tests.Fuzz;

/// <summary>
/// Property-based fuzz tests over the public surface: invariants (round trips, never-throw
/// guarantees, consistency between related members) checked over generated inputs instead of
/// chosen examples.
/// </summary>
/// <remarks>
/// <c>Sample</c> evaluates cases on several threads, so every property is a pure function of its
/// generated input. Run length and replay are controlled by <see cref="FuzzSettings"/>.
/// </remarks>
public sealed class MailProperties
{

    private static readonly Gen<string> LocalPart = Gen.String[Gen.Char["abcdefghijklmnopqrstuvwxyz0123456789._-"], 1, 20]
        .Where(s => char.IsLetterOrDigit(s[0]) && char.IsLetterOrDigit(s[^1]) && !s.Contains("..", StringComparison.Ordinal));



    private static readonly Gen<string> Domain = Gen.String[Gen.Char["abcdefghijklmnopqrstuvwxyz"], 1, 12]
        .Select(Gen.OneOfConst("com", "org", "example", "co.uk"), (label, tld) => $"{label}.{tld}");



    private static readonly Gen<string> AnyAddress = LocalPart.Select(Domain, (local, domain) => $"{local}@{domain}");



    /// <summary>Display names with spaces, punctuation, quotes, backslashes and non-ASCII letters.</summary>
    private static readonly Gen<string> DisplayName = Gen.String[Gen.Char["abcXYZ 019,.;:'\"\\()<>@äöüßéñ漢"], 1, 24]
        .Where(s => s.Trim().Length > 0 && string.Equals(s.Trim(), s, StringComparison.Ordinal));



    /// <summary>Single-line text (subjects): printable ASCII plus a few non-ASCII letters.</summary>
    private static readonly Gen<string> SingleLineText = Gen.String[Gen.Char[" abcdefXYZ0123456789.,:;!?-_()'äöüßéñ漢€"], 1, 60]
        .Where(s => s.Length > 0 && string.Equals(s.Trim(), s, StringComparison.Ordinal) && !s.Contains("  ", StringComparison.Ordinal));



    /// <summary>Body text: lines of printable text joined with CRLF.</summary>
    private static readonly Gen<string> Body = Gen.String[Gen.Char[" abcdefXYZ0123456789.,:;!?-_=()äöü€"], 1, 50]
        .Array[1, 5]
        .Select(lines => string.Join("\r\n", lines.Select(l => l.TrimEnd())));



    private static readonly Regex MimeNoise = new Regex
    (
        @"^Date: .*$|boundary_\d+_[0-9a-f-]{36}",
        RegexOptions.Multiline | RegexOptions.CultureInvariant,
        TimeSpan.FromSeconds(1)
    );



    [Fact]
    public void Parse_when_input_is_arbitrary_text_never_throws_in_lenient_mode()
    {
        var anyText = Gen.String[Gen.Char.AlphaNumeric, 0, 200]
            .Select(Gen.String[Gen.Char[":;=\"<>@,\r\n\t -?_"], 0, 60], Gen.Int[0, 3], (a, b, n) => n switch
            {
                0 => a,
                1 => a + b,
                2 => b + a,
                _ => $"{a}:{b}\r\n\r\n{a}",
            });

        anyText.Sample
        (
            text =>
            {
                using var message = EmlParser.Parse(text);
            },
            seed: FuzzSettings.Seed,
            iter: FuzzSettings.Iterations,
            time: FuzzSettings.TimeSeconds
        );
    }



    /// <summary>
    /// Display names with a <c>"</c> are excluded: System.Net.Mail's <c>MailWriter</c> writes them
    /// unescaped (<c>From: ""1" &lt;a@b.c&gt;</c>), which no parser can read back (#366).
    /// </summary>
    [Fact]
    public void ToMimeString_then_Parse_round_trips_addresses_subject_and_body()
    {
        Gen.Select(AnyAddress, DisplayName.Where(n => !n.Contains('"', StringComparison.Ordinal)), AnyAddress.Array[1, 4], SingleLineText, Body)
            .Sample
            (
                (from, fromName, to, subject, body) =>
                {
                    var builder = new MailMessageBuilder()
                        .From(from, fromName)
                        .Subject(subject)
                        .PlainTextBody(body);
                    foreach (var recipient in to)
                    {
                        builder.To(recipient);
                    }

                    using var original = builder.Build();
                    using var parsed = EmlParser.Parse(original.ToMimeString());

                    // Assertions rather than a bool, so a failure names the field that differs.
                    Assert.NotNull(parsed.From);
                    Assert.Equal(from, parsed.From!.Address);
                    Assert.Equal(fromName, parsed.From.DisplayName);
                    Assert.Equal(to, parsed.To.Select(a => a.Address));
                    Assert.Equal(subject, parsed.Subject);
                    Assert.Equal(body, parsed.Body);
                },
                seed: FuzzSettings.Seed,
                iter: FuzzSettings.Iterations,
                time: FuzzSettings.TimeSeconds
            );
    }



    [Fact]
    public void Clone_serializes_to_the_same_MIME_as_the_original()
    {
        Gen.Select(AnyAddress, AnyAddress, SingleLineText, Body, Gen.Bool)
            .Sample
            (
                (from, to, subject, body, html) =>
                {
                    using var original = new MailMessageBuilder()
                        .From(from)
                        .To(to)
                        .Subject(subject)
                        .Header("X-Fuzz", "1")
                        .Build();
                    original.Body = body;
                    original.IsBodyHtml = html;

                    using var clone = original.Clone();

                    return string.Equals
                    (
                        MimeNoise.Replace(clone.ToMimeString(), string.Empty),
                        MimeNoise.Replace(original.ToMimeString(), string.Empty),
                        StringComparison.Ordinal
                    );
                },
                seed: FuzzSettings.Seed,
                iter: FuzzSettings.Iterations,
                time: FuzzSettings.TimeSeconds
            );
    }



    /// <summary>
    /// The output is <c>"quoted-string" &lt;address&gt;</c> where every <c>"</c> and <c>\</c> of the
    /// display name is a quoted-pair (RFC 5322 3.2.4), so undoing the escaping gives the name back.
    /// </summary>
    [Fact]
    public void FormatMailAddress_output_is_a_quoted_string_that_unescapes_to_the_display_name()
    {
        // MailAddress normalizes the name it is given (it strips one pair of enclosing quotes, so
        // "\"\"" becomes empty, which FormatMailAddress then omits); such names are not generated.
        var keptNames = DisplayName.Where(n => new MailAddress("a@example.com", n).DisplayName.Length > 0);

        Gen.Select(AnyAddress, keptNames)
            .Sample
            (
                (address, displayName) =>
                {
                    var source = new MailAddress(address, displayName);
                    var formatted = source.FormatMailAddress();
                    var suffix = $"\" <{address}>";

                    Assert.StartsWith("\"", formatted, StringComparison.Ordinal);
                    Assert.EndsWith(suffix, formatted, StringComparison.Ordinal);

                    var quoted = formatted.Substring(1, formatted.Length - 1 - suffix.Length);
                    var unescaped = new System.Text.StringBuilder();
                    for (var i = 0; i < quoted.Length; i++)
                    {
                        Assert.NotEqual('"', quoted[i]);
                        unescaped.Append(quoted[i] == '\\' ? quoted[++i] : quoted[i]);
                    }

                    Assert.Equal(source.DisplayName, unescaped.ToString());
                },
                seed: FuzzSettings.Seed,
                iter: FuzzSettings.Iterations,
                time: FuzzSettings.TimeSeconds
            );
    }



    /// <summary>
    /// <see cref="MailAddress"/> reads the output back unchanged when the display name has no
    /// <c>"</c> or <c>\</c>. With those characters it does not undo the quoted-pair escaping
    /// (<c>new MailAddress("\"\\\"\" &lt;a@b.c&gt;").DisplayName</c> is <c>\"</c>), which is a
    /// System.Net.Mail limitation, not a formatting error, so they are excluded here and checked
    /// by the property above instead.
    /// </summary>
    [Fact]
    public void FormatMailAddress_output_parses_back_with_MailAddress_when_the_name_needs_no_escaping()
    {
        Gen.Select(AnyAddress, DisplayName.Where(n => n.IndexOfAny(['"', '\\']) < 0))
            .Sample
            (
                (address, displayName) =>
                {
                    var reparsed = new MailAddress(new MailAddress(address, displayName).FormatMailAddress());

                    return string.Equals(reparsed.Address, address, StringComparison.Ordinal)
                        && string.Equals(reparsed.DisplayName, displayName, StringComparison.Ordinal);
                },
                seed: FuzzSettings.Seed,
                iter: FuzzSettings.Iterations,
                time: FuzzSettings.TimeSeconds
            );
    }



    [Fact]
    public void TryParse_never_throws_and_agrees_with_the_MailAddress_constructor()
    {
        Gen.OneOf(AnyAddress, Gen.String, Gen.String[Gen.Char["a@<>\" .,;:"], 0, 30])
            .Sample
            (
                text =>
                {
                    var parsed = MailAddress.TryParse(text, out var result);
                    bool constructed;
                    try
                    {
                        _ = new MailAddress(text);
                        constructed = !string.IsNullOrWhiteSpace(text);
                    }
                    catch (FormatException)
                    {
                        constructed = false;
                    }
                    catch (ArgumentException)
                    {
                        constructed = false;
                    }

                    return parsed == constructed && parsed == (result != null);
                },
                seed: FuzzSettings.Seed,
                iter: FuzzSettings.Iterations,
                time: FuzzSettings.TimeSeconds
            );
    }



    [Fact]
    public void InferContentType_always_returns_a_media_type()
    {
        Gen.String
            .Sample
            (
                fileName =>
                {
                    var contentType = AttachmentFactory.InferContentType(fileName);
                    var slash = contentType.IndexOf('/', StringComparison.Ordinal);

                    return slash > 0 && slash < contentType.Length - 1;
                },
                seed: FuzzSettings.Seed,
                iter: FuzzSettings.Iterations,
                time: FuzzSettings.TimeSeconds
            );
    }



    [Fact]
    public void FromBase64_and_TotalSize_agree_with_the_original_bytes()
    {
        Gen.Byte.Array[0, 4096].Array[1, 4]
            .Sample
            (
                payloads =>
                {
                    using var message = new MailMessage();
                    for (var i = 0; i < payloads.Length; i++)
                    {
                        message.Attachments.Add(AttachmentFactory.FromBase64(Convert.ToBase64String(payloads[i]), $"file{i}.bin"));
                    }

                    var expectedTotal = payloads.Sum(p => (long)p.Length);
                    var contentMatches = message.Attachments
                        .Select((attachment, i) => ReadAll(attachment).SequenceEqual(payloads[i]))
                        .All(matches => matches);

                    return contentMatches
                        && message.Attachments.TotalSize() == expectedTotal
                        && message.Attachments.ExceedsLimit(expectedTotal) == false
                        && message.Attachments.ExceedsLimit(expectedTotal - 1);
                },
                seed: FuzzSettings.Seed,
                iter: FuzzSettings.Iterations,
                time: FuzzSettings.TimeSeconds
            );
    }



    private static byte[] ReadAll(Attachment attachment)
    {
        attachment.ContentStream.Position = 0;
        using var copy = new System.IO.MemoryStream();
        attachment.ContentStream.CopyTo(copy);
        return copy.ToArray();
    }
}
