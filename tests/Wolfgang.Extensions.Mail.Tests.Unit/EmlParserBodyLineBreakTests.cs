using System.Net.Mail;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// The final line break of a single-part body. For quoted-printable and 7bit the writer's last
/// CRLF is part of the wire format and is removed; for base64 every byte of the decoded content
/// is the sender's, so a body that ends in a line break must keep it.
/// </summary>
public class EmlParserBodyLineBreakTests
{

    [Theory]
    [InlineData("Line\r\n")]
    [InlineData("Line\r\n\r\n")]
    [InlineData("Line")]
    [InlineData("\r\n")]
    public void ToMimeString_then_Parse_keeps_the_body_of_a_base64_message(string body)
    {
        using var original = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Body = "Grüße " + body,
            BodyEncoding = Encoding.UTF8,
        };

        var mime = original.ToMimeString();
        using var parsed = EmlParser.Parse(mime);

        Assert.Contains("Content-Transfer-Encoding: base64", mime, System.StringComparison.Ordinal);
        Assert.Equal(original.Body, parsed.Body);
    }



    [Theory]
    [InlineData("Line\r\n")]
    [InlineData("Line")]
    public void ToMimeString_then_Parse_keeps_the_body_of_a_quoted_printable_message(string body)
    {
        using var original = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Body = body,
        };

        var mime = original.ToMimeString();
        using var parsed = EmlParser.Parse(mime);

        Assert.Contains("Content-Transfer-Encoding: quoted-printable", mime, System.StringComparison.Ordinal);
        Assert.Equal(body, parsed.Body);
    }



    [Fact]
    public void Parse_when_base64_body_ends_with_a_line_break_keeps_it()
    {
        // "Hi\r\n" in base64, followed by the line break that ends the encoded line.
        using var msg = EmlParser.Parse
        (
            "From: f@example.com\r\nContent-Type: text/plain; charset=utf-8\r\n" +
            "Content-Transfer-Encoding: base64\r\n\r\nSGkNCg==\r\n"
        );

        Assert.Equal("Hi\r\n", msg.Body);
    }



    [Fact]
    public void Parse_when_7bit_body_ends_with_a_line_break_removes_only_the_final_one()
    {
        using var msg = EmlParser.Parse("From: f@example.com\r\n\r\nHi\r\n\r\n");

        Assert.Equal("Hi\r\n", msg.Body);
    }
}
