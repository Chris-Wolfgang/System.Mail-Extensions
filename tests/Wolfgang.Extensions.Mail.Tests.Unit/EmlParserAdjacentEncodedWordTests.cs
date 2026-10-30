using System.Net.Mail;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// RFC 2047 section 6.2: linear whitespace between two adjacent encoded words is not part of the
/// text. <see cref="MailMessage"/> folds a long non-ASCII header into several encoded words, so
/// keeping that whitespace inserts spaces that were never in the original.
/// </summary>
public class EmlParserAdjacentEncodedWordTests
{

    [Theory]
    [InlineData("=?utf-8?Q?ab?= =?utf-8?Q?cd?=", "abcd")]
    [InlineData("=?utf-8?Q?ab?=\r\n =?utf-8?B?Y2Q=?=", "abcd")]
    [InlineData("=?utf-8?Q?ab?=\t \t=?utf-8?Q?cd?=", "abcd")]
    [InlineData("=?utf-8?Q?ab?= plain =?utf-8?Q?cd?=", "ab plain cd")]
    [InlineData("plain =?utf-8?Q?ab?= tail", "plain ab tail")]
    [InlineData("=?utf-8?Q?a_b?= =?utf-8?Q?_c?=", "a b c")]
    public void Parse_when_subject_has_adjacent_encoded_words_joins_them(string subject, string expected)
    {
        using var msg = EmlParser.Parse("From: f@example.com\r\nSubject: " + subject + "\r\n\r\nBody");

        Assert.Equal(expected, msg.Subject);
    }



    /// <summary>
    /// .NET Framework's MailWriter splits a multi-byte UTF-8 character across two encoded words;
    /// the bytes of adjacent words in the same charset are decoded together.
    /// </summary>
    [Theory]
    [InlineData("=?utf-8?Q?Malm=C3?= =?utf-8?Q?=B6?=", "Malmö")]
    [InlineData("=?utf-8?B?TWFsbcM=?= =?UTF-8?B?tg==?=", "Malmö")]
    [InlineData("=?utf-8?Q?a?= =?iso-8859-1?Q?=E9?=", "aé")]
    public void Parse_when_a_character_is_split_across_encoded_words_decodes_it_whole(string subject, string expected)
    {
        using var msg = EmlParser.Parse("From: f@example.com\r\nSubject: " + subject + "\r\n\r\nBody");

        Assert.Equal(expected, msg.Subject);
    }



    [Fact]
    public void ParseWithDiagnostics_when_a_word_in_a_run_is_malformed_keeps_it_and_decodes_the_rest()
    {
        var result = EmlParser.ParseWithDiagnostics
        (
            "From: f@example.com\r\nSubject: =?utf-8?Q?a?= =?utf-8?B?not*base64?= =?utf-8?Q?b?=\r\n\r\nBody"
        );

        using var msg = result.Message;
        Assert.Equal("a=?utf-8?B?not*base64?=b", msg.Subject);
        var issue = Assert.Single(result.Issues);
        Assert.Equal("Malformed RFC 2047 encoded word: '=?utf-8?B?not*base64?='; kept verbatim.", issue.Message);
    }



    [Fact]
    public void Parse_when_display_name_is_folded_into_two_encoded_words_joins_them()
    {
        using var msg = EmlParser.Parse
        (
            "From: =?utf-8?Q?=2C=29=C3=B1=C3=A91=C3=BC=E6=BC=A2=C3=B6=C3=B1?=\r\n =?utf-8?Q?=E6=BC=A2?= <x@example.com>\r\n" +
            "\r\nBody"
        );

        Assert.Equal(",)ñé1ü漢öñ漢", msg.From!.DisplayName);
    }



    [Fact]
    public void ToMimeString_then_Parse_keeps_a_long_non_ascii_display_name_and_subject()
    {
        const string name = "<Y漢Xcß>ñ,>X漢:ä. Zoë Ørsted-Müller";
        const string subject = "Grüße aus Köln, Zürich und Malmö - 漢字 Überprüfung der Änderungen";
        using var original = new MailMessage(new MailAddress("x@example.com", name), new MailAddress("t@example.com"))
        {
            Subject = subject,
        };

        using var parsed = EmlParser.Parse(original.ToMimeString());

        Assert.Equal(name, parsed.From!.DisplayName);
        Assert.Equal(subject, parsed.Subject);
    }
}
