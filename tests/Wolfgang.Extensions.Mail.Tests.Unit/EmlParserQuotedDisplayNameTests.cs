using System.Net.Mail;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Quoted display names that contain <c>&lt;</c> or <c>&gt;</c>. Inside a quoted string those
/// are ordinary characters (RFC 5322 3.2.4), and <see cref="MailMessage"/> writes such a name
/// quoted, so the parser must not treat them as the start of the address.
/// </summary>
public class EmlParserQuotedDisplayNameTests
{

    [Theory]
    [InlineData("From: \"1cc<\" <z@example.com>", "1cc<")]
    [InlineData("From: \"a > b\" <z@example.com>", "a > b")]
    [InlineData("From: \"<Team>\" <z@example.com>", "<Team>")]
    [InlineData("From: \"Last, First <x>\"<z@example.com>", "Last, First <x>")]
    public void Parse_when_quoted_From_display_name_has_angle_brackets_keeps_it(string fromHeader, string expected)
    {
        using var msg = EmlParser.Parse(fromHeader + "\r\nTo: t@example.com\r\n\r\nBody");

        Assert.NotNull(msg.From);
        Assert.Equal("z@example.com", msg.From!.Address);
        Assert.Equal(expected, msg.From.DisplayName);
    }



    [Fact]
    public void Parse_when_quoted_recipient_display_name_has_angle_brackets_keeps_every_recipient()
    {
        using var msg = EmlParser.Parse
        (
            "From: f@example.com\r\n" +
            "To: \"9<\" <a@example.com>, plain@example.com, \"x>y, z\" <b@example.com>\r\n" +
            "\r\nBody"
        );

        Assert.Equal(3, msg.To.Count);
        Assert.Equal("9<", msg.To[0].DisplayName);
        Assert.Equal("plain@example.com", msg.To[1].Address);
        Assert.Equal("x>y, z", msg.To[2].DisplayName);
        Assert.Equal("b@example.com", msg.To[2].Address);
    }



    [Fact]
    public void ToMimeString_then_Parse_keeps_a_display_name_with_angle_brackets()
    {
        using var original = new MailMessage
        (
            new MailAddress("sender@example.com", "Ops <on call>"),
            new MailAddress("recipient@example.com")
        );

        using var parsed = EmlParser.Parse(original.ToMimeString());

        Assert.Equal("sender@example.com", parsed.From!.Address);
        Assert.Equal("Ops <on call>", parsed.From.DisplayName);
    }
}
