using System.Net.Mail;
using System.Text;
using Wolfgang.Extensions.Mail;
using Xunit;

namespace Wolfgang.Extensions.Mail.Tests.Integration;


/// <summary>
/// End-to-end wire-format fidelity: a message serialized with
/// <see cref="MailMessageExtensions.ToMimeString"/> must parse back via
/// <see cref="EmlParser.Parse"/> into an equivalent message.
/// </summary>
public class RoundTripTests
{

    [Fact]
    public void ToMimeString_when_parsed_back_preserves_subject_and_addresses()
    {
        using var original = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Subject = "Round trip subject",
            Body = "Round trip body."
        };
        original.CC.Add("copy@example.com");

        using var parsed = EmlParser.Parse(original.ToMimeString());

        Assert.Equal("sender@example.com", parsed.From!.Address);
        Assert.Equal("recipient@example.com", parsed.To.Single().Address);
        Assert.Equal("copy@example.com", parsed.CC.Single().Address);
        Assert.Equal("Round trip subject", parsed.Subject);
    }



    [Fact]
    public async Task ToMimeString_when_message_has_binary_attachment_parsed_back_preserves_content()
    {
        var payload = Enumerable.Range(0, 8_192).Select(i => (byte)(i % 256)).ToArray();

        using var original = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Subject = "Attachment round trip",
            Body = "See attachment."
        };
        original.Attachments.Add(AttachmentFactory.FromBytes(payload, "data.bin"));

        using var parsed = EmlParser.Parse(original.ToMimeString());

        var attachment = Assert.Single(parsed.Attachments);
        var roundTripped = await ReadAllBytesAsync(attachment.ContentStream);
        Assert.Equal(payload, roundTripped);
    }



    [Fact]
    public void ToMimeString_when_message_has_html_alternate_view_parsed_back_preserves_html()
    {
        const string html = "<html><body><p>alternate view content</p></body></html>";

        using var original = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Subject = "Alternate view round trip",
            Body = "Plain fallback."
        };
        original.AlternateViews.Add
        (
            AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, "text/html")
        );

        using var parsed = EmlParser.Parse(original.ToMimeString());

        Assert.Single(parsed.AlternateViews);
    }



    [Fact]
    public void ToMimeString_when_serialize_parse_repeated_body_is_stable()
    {
        using var original = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Subject = "Stability",
            Body = "The body must survive repeated round trips unchanged."
        };

        using var firstPass = EmlParser.Parse(original.ToMimeString());
        using var secondPass = EmlParser.Parse(firstPass.ToMimeString());

        Assert.Equal(original.Body, firstPass.Body);
        Assert.Equal(firstPass.Body, secondPass.Body);
        Assert.Equal(firstPass.Subject, secondPass.Subject);
    }



    private static async Task<byte[]> ReadAllBytesAsync
    (
        Stream stream
    )
    {
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer);
        return buffer.ToArray();
    }
}
