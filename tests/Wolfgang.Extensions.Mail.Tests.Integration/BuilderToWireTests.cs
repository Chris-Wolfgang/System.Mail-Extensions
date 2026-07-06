using System.Text;
using Wolfgang.Extensions.Mail;
using Xunit;

namespace Wolfgang.Extensions.Mail.Tests.Integration;


/// <summary>
/// End-to-end composition: messages assembled with
/// <see cref="MailMessageBuilder"/> and <see cref="InlineHtmlBuilder"/> must
/// survive serialization to the wire format and parsing back.
/// </summary>
public class BuilderToWireTests
{

    [Fact]
    public void Build_when_full_message_composed_round_trips_through_mime()
    {
        var payload = Enumerable.Range(0, 2_048).Select(i => (byte)(i % 256)).ToArray();

        using var built = new MailMessageBuilder()
            .From("sender@example.com", "Sender Name")
            .To("recipient@example.com")
            .Cc("copy@example.com")
            .Subject("Composed message")
            .PlainTextBody("Plain text part.")
            .Attach(payload, "data.bin")
            .Build();

        using var parsed = EmlParser.Parse(built.ToMimeString());

        Assert.Equal("Composed message", parsed.Subject);
        Assert.Equal("sender@example.com", parsed.From!.Address);
        Assert.Equal("recipient@example.com", parsed.To.Single().Address);
        Assert.Equal("copy@example.com", parsed.CC.Single().Address);
        Assert.Single(parsed.Attachments);
    }



    [Fact]
    public void Build_when_inline_html_view_attached_serialized_output_carries_embedded_image()
    {
        var imageBytes = Encoding.ASCII.GetBytes("fake-png-payload-for-integration-test");

        var view = new InlineHtmlBuilder()
            .Html("<html><body><img src=\"cid:{0}\" /></body></html>")
            .EmbedImage(imageBytes, "pixel.png", "image/png")
            .Build();

        using var message = new MailMessageBuilder()
            .From("sender@example.com")
            .To("recipient@example.com")
            .Subject("Inline image")
            .PlainTextBody("Fallback.")
            .Build();
        message.AlternateViews.Add(view);

        var mime = message.ToMimeString();

        Assert.Contains("image/png", mime, StringComparison.OrdinalIgnoreCase);
        Assert.Contains
        (
            Convert.ToBase64String(imageBytes).Substring(0, 20),
            mime,
            StringComparison.Ordinal
        );
    }
}
