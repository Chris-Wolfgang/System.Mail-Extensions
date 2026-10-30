using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace Wolfgang.Extensions.Mail.Tests.Snapshots;

/// <summary>
/// Pins the exact MIME text <see cref="MailMessageExtensions.ToMimeString"/> produces, so a
/// header, encoding or multipart-layout change shows up as a snapshot diff instead of slipping
/// past targeted assertions. The <c>Date</c> header and the GUID-based multipart boundaries
/// change on every run and are scrubbed.
/// </summary>
public class MimeSerializationSnapshotTests
{

    private static SettingsTask VerifyMime(string mime)
    {
        return Verifier
            .Verify(mime)
            .ScrubInlineGuids()
            .ScrubLinesWithReplace(line => line.StartsWith("Date: ", System.StringComparison.Ordinal) ? "Date: {scrubbed}" : line);
    }



    [Fact]
    public Task ToMimeString_when_plain_text_message_matches_snapshot()
    {
        using var message = new MailMessageBuilder()
            .From("sender@example.com", "Sender Name")
            .To("first@example.com")
            .To("second@example.com")
            .Cc("copy@example.com")
            .Subject("Quarterly report")
            .PlainTextBody("Hello,\r\n\r\nThe report is attached.\r\n")
            .Build();

        return VerifyMime(message.ToMimeString());
    }



    [Fact]
    public Task ToMimeString_when_html_body_has_attachment_matches_snapshot()
    {
        using var message = new MailMessageBuilder()
            .From("sender@example.com")
            .To("recipient@example.com")
            .Subject("Invoice")
            .HtmlBody("<p>Your invoice is attached.</p>")
            .Attach(Encoding.ASCII.GetBytes("total,42\n"), "invoice.csv")
            .Header("X-Correlation-Id", "abc-123")
            .Priority(MailPriority.High)
            .Build();

        return VerifyMime(message.ToMimeString());
    }



    [Fact]
    public Task ToMimeString_when_subject_and_display_name_are_not_ascii_matches_snapshot()
    {
        using var message = new MailMessageBuilder()
            .From("sender@example.com", "Jürgen Müller")
            .To("recipient@example.com")
            .Subject("Grüße aus Köln")
            .SubjectEncoding(Encoding.UTF8)
            .BodyEncoding(Encoding.UTF8)
            .PlainTextBody("Schöne Grüße")
            .Build();

        return VerifyMime(message.ToMimeString());
    }



    [Fact]
    public Task ToMimeString_when_message_has_alternate_views_matches_snapshot()
    {
        using var message = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Subject = "Alternatives",
            Body = "Plain text version",
        };
        message.AlternateViews.Add
        (
            AlternateView.CreateAlternateViewFromString("<b>HTML version</b>", Encoding.UTF8, MediaTypeNames.Text.Html)
        );

        return VerifyMime(message.ToMimeString());
    }
}
