using System.Net.Mail;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// RFC 2047 encoded words in address display names. <see cref="MailMessage"/> writes a non-ASCII
/// display name as an encoded word (<c>=?utf-8?Q?...?=</c>), so a parser that does not decode
/// it loses the name on a <see cref="MailMessageExtensions.ToMimeString"/> round trip.
/// </summary>
public class EmlParserEncodedDisplayNameTests
{

    [Theory]
    [InlineData("From: =?utf-8?Q?J=C3=BCrgen_M=C3=BCller?= <j@example.com>", "Jürgen Müller")]
    [InlineData("From: \"=?utf-8?B?SsO8cmdlbg==?=\" <j@example.com>", "Jürgen")]
    [InlineData("From: =?iso-8859-1?Q?Fran=E7ois?= <j@example.com>", "François")]
    [InlineData("From: Plain Name <j@example.com>", "Plain Name")]
    public void Parse_when_From_display_name_is_encoded_decodes_it(string fromHeader, string expected)
    {
        using var msg = EmlParser.Parse(fromHeader + "\r\nTo: t@example.com\r\n\r\nBody");

        Assert.Equal("j@example.com", msg.From!.Address);
        Assert.Equal(expected, msg.From.DisplayName);
    }



    [Fact]
    public void Parse_when_recipient_display_names_are_encoded_decodes_each()
    {
        using var msg = EmlParser.Parse
        (
            "From: f@example.com\r\n" +
            "To: =?utf-8?Q?Ren=C3=A9e?= <r@example.com>, plain@example.com\r\n" +
            "Cc: =?utf-8?B?w5hyc3RlZA==?= <c@example.com>\r\n" +
            "\r\nBody"
        );

        Assert.Equal("Renée", msg.To[0].DisplayName);
        Assert.Equal("plain@example.com", msg.To[1].Address);
        Assert.Equal("Ørsted", msg.CC[0].DisplayName);
    }



    [Fact]
    public void ToMimeString_then_Parse_keeps_a_non_ascii_display_name()
    {
        using var original = new MailMessage
        (
            new MailAddress("sender@example.com", "éYY;"),
            new MailAddress("recipient@example.com", "Zoë Smith")
        );

        using var parsed = EmlParser.Parse(original.ToMimeString());

        Assert.Equal("éYY;", parsed.From!.DisplayName);
        Assert.Equal("Zoë Smith", parsed.To[0].DisplayName);
    }



    [Fact]
    public void ParseWithDiagnostics_when_display_name_encoded_word_is_malformed_keeps_it_and_reports_it()
    {
        var result = EmlParser.ParseWithDiagnostics("From: =?utf-8?B?not*base64?= <j@example.com>\r\n\r\nBody");

        using var msg = result.Message;
        Assert.Equal("=?utf-8?B?not*base64?=", msg.From!.DisplayName);
        var issue = Assert.Single(result.Issues);
        Assert.Equal("Malformed RFC 2047 encoded word: '=?utf-8?B?not*base64?='; kept verbatim.", issue.Message);
    }
}
