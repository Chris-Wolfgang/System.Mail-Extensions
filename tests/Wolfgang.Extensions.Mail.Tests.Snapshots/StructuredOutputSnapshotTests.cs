using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using VerifyXunit;
using Wolfgang.Extensions.Mail.Validation;
using Xunit;

namespace Wolfgang.Extensions.Mail.Tests.Snapshots;

/// <summary>
/// Pins the shape of the library's structured and formatted output: address formatting,
/// what <see cref="EmlParser"/> extracts from a message, the issues it and
/// <see cref="MailMessageExtensions.Validate(MailMessage, ValidationOptions)"/> report, and the
/// content-type table <see cref="AttachmentFactory.InferContentType"/> uses.
/// </summary>
public class StructuredOutputSnapshotTests
{

    private const string MultipartEml =
        "From: \"Sender Name\" <sender@example.com>\r\n" +
        "To: first@example.com, \"Second, Person\" <second@example.com>\r\n" +
        "Cc: copy@example.com\r\n" +
        "Subject: =?utf-8?B?R3LDvMOfZQ==?=\r\n" +
        "X-Priority: 1\r\n" +
        "X-Custom: custom value\r\n" +
        "MIME-Version: 1.0\r\n" +
        "Content-Type: multipart/mixed; boundary=\"outer\"\r\n" +
        "\r\n" +
        "--outer\r\n" +
        "Content-Type: multipart/alternative; boundary=\"inner\"\r\n" +
        "\r\n" +
        "--inner\r\n" +
        "Content-Type: text/plain; charset=utf-8\r\n" +
        "Content-Transfer-Encoding: quoted-printable\r\n" +
        "\r\n" +
        "Plain =C3=BC text\r\n" +
        "--inner\r\n" +
        "Content-Type: text/html; charset=utf-8\r\n" +
        "\r\n" +
        "<p>HTML text</p>\r\n" +
        "--inner--\r\n" +
        "--outer\r\n" +
        "Content-Type: text/csv; name=\"data.csv\"\r\n" +
        "Content-Disposition: attachment; filename=\"data.csv\"\r\n" +
        "Content-Transfer-Encoding: base64\r\n" +
        "\r\n" +
        "YSxiCjEsMgo=\r\n" +
        "--outer--\r\n";



    private static object Describe(MailMessage message)
    {
        return new
        {
            From = message.From?.ToString(),
            To = message.To.Select(a => a.ToString()).ToArray(),
            Cc = message.CC.Select(a => a.ToString()).ToArray(),
            message.Subject,
            message.Priority,
            message.IsBodyHtml,
            message.Body,
            CustomHeaders = message.Headers.AllKeys.OrderBy(k => k, System.StringComparer.Ordinal).Select(k => $"{k}: {message.Headers[k]}").ToArray(),
            AlternateViews = message.AlternateViews.Select(v => v.ContentType.MediaType).ToArray(),
            Attachments = message.Attachments.Select(a => new { a.Name, a.ContentType.MediaType, Length = a.ContentStream.Length }).ToArray(),
        };
    }



    [Fact]
    public Task ToFormattedString_when_display_names_need_escaping_matches_snapshot()
    {
        using var message = new MailMessage();
        message.To.Add(new MailAddress("plain@example.com"));
        message.To.Add(new MailAddress("named@example.com", "Named Person"));
        message.To.Add(new MailAddress("comma@example.com", "Last, First"));
        message.To.Add(new MailAddress("quote@example.com", "Say \"hi\""));
        message.To.Add(new MailAddress("slash@example.com", "Back\\slash"));

        return Verifier.Verify
        (
            new
            {
                Default = message.To.ToFormattedString(),
                CommaSeparated = message.To.ToFormattedString(", "),
                Each = message.To.Select(a => a.FormatMailAddress()).ToArray(),
            }
        );
    }



    [Fact]
    public Task Parse_when_message_is_multipart_matches_snapshot()
    {
        using var message = EmlParser.Parse(MultipartEml);

        return Verifier.Verify(Describe(message));
    }



    [Fact]
    public Task ParseWithDiagnostics_when_headers_are_malformed_matches_snapshot()
    {
        const string eml =
            "From: not an address\r\n" +
            "To: good@example.com, also bad\r\n" +
            "Subject: Broken\r\n" +
            "\r\n" +
            "Body\r\n";

        var result = EmlParser.ParseWithDiagnostics(eml);
        using var message = result.Message;

        return Verifier.Verify
        (
            new
            {
                result.HasIssues,
                Issues = result.Issues.Select(i => i.ToString()).ToArray(),
                Parsed = Describe(message),
            }
        );
    }



    private static object DescribeValidation(string cultureName)
    {
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
        try
        {
            using var message = new MailMessage();
            message.Attachments.Add(AttachmentFactory.FromBytes(new byte[2048], "big.bin"));
            var options = new ValidationOptions
            {
                RequireSubject = true,
                RequireBody = true,
                MaxAttachmentSizeBytes = 1024,
            };

            var result = message.Validate(options);

            return new
            {
                result.IsValid,
                Errors = result.Errors.Select(i => i.ToString()).ToArray(),
                Warnings = result.Warnings.Select(i => i.ToString()).ToArray(),
            };
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }



    [Fact]
    public Task Validate_when_message_has_several_problems_matches_snapshot()
    {
        return Verifier.Verify(DescribeValidation("en-US"));
    }



    /// <summary>
    /// Size limits in validation messages are formatted with the current culture (group
    /// separators), so the same message reads "2.048 bytes" under de-DE.
    /// </summary>
    [Fact]
    public Task Validate_when_current_culture_is_de_DE_formats_sizes_with_that_culture()
    {
        return Verifier.Verify(DescribeValidation("de-DE"));
    }



    [Fact]
    public Task InferContentType_for_common_extensions_matches_snapshot()
    {
        var names = new[]
        {
            "report.pdf", "photo.jpg", "photo.JPEG", "image.png", "anim.gif", "page.html", "notes.txt",
            "data.csv", "data.json", "data.xml", "archive.zip", "sheet.xlsx", "doc.docx", "mail.eml",
            "noextension", "unknown.xyz",
        };

        return Verifier.Verify(names.ToDictionary(n => n, AttachmentFactory.InferContentType));
    }
}
