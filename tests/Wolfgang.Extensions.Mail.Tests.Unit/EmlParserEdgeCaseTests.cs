using System.Text;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Targeted coverage for the less-travelled parser branches: alternate
/// header forms, LF-only line endings, quoted-printable soft breaks and
/// surrogates, RFC 2047 encoded words, and address-splitting corner cases.
/// </summary>
public class EmlParserEdgeCaseTests
{

    [Fact]
    public void Parse_when_ReplyTo_header_present_populates_ReplyToList()
    {
        const string eml =
            "From: sender@example.com\r\n" +
            "To: recipient@example.com\r\n" +
            "Reply-To: replies@example.com\r\n" +
            "Subject: Reply-to\r\n\r\n" +
            "Body.\r\n";

        using var message = EmlParser.Parse(eml);

        Assert.Equal("replies@example.com", Assert.Single(message.ReplyToList).Address);
    }



    [Fact]
    public void Parse_when_line_endings_are_bare_LF_splits_headers_and_body()
    {
        // LF-only forces the "\n\n" separator branch, the bare-LF body trim,
        // and the final-header flush.
        const string eml =
            "From: sender@example.com\n" +
            "To: recipient@example.com\n" +
            "Subject: Bare LF\n\n" +
            "Line one\n";

        using var message = EmlParser.Parse(eml);

        Assert.Equal("Bare LF", message.Subject);
        Assert.Equal("Line one", message.Body);
    }



    [Fact]
    public void Parse_when_quoted_printable_has_soft_breaks_joins_lines()
    {
        // Both a CRLF soft break and a bare-LF soft break.
        const string eml =
            "From: sender@example.com\r\n" +
            "To: recipient@example.com\r\n" +
            "Content-Type: text/plain; charset=utf-8\r\n" +
            "Content-Transfer-Encoding: quoted-printable\r\n\r\n" +
            "one=\r\ntwo=\nthree lower =c3=a9\r\n";

        using var message = EmlParser.Parse(eml);

        Assert.Contains("onetwothree", message.Body, StringComparison.Ordinal);
        Assert.Contains('é', message.Body);   // =c3=a9 decoded via lowercase hex
    }



    [Fact]
    public void Parse_when_quoted_printable_body_contains_non_bmp_char_preserves_it()
    {
        // A literal astral character (surrogate pair) in a QP body exercises
        // the surrogate-pair path of the decoder.
        var eml =
            "From: sender@example.com\r\n" +
            "To: recipient@example.com\r\n" +
            "Content-Type: text/plain; charset=utf-8\r\n" +
            "Content-Transfer-Encoding: quoted-printable\r\n\r\n" +
            "smile \U0001F600 here\r\n";

        using var message = EmlParser.Parse(eml);

        Assert.Contains("\U0001F600", message.Body, StringComparison.Ordinal);
    }



    [Fact]
    public void Parse_when_subject_is_Q_encoded_word_decodes_it()
    {
        const string eml =
            "From: sender@example.com\r\n" +
            "To: recipient@example.com\r\n" +
            "Subject: =?utf-8?Q?caf=C3=A9_r=C3=A9sum=C3=A9?=\r\n\r\n" +
            "Body.\r\n";

        using var message = EmlParser.Parse(eml);

        Assert.Equal("café résumé", message.Subject);
    }



    [Fact]
    public void Parse_when_subject_is_B_encoded_word_decodes_it()
    {
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("Ünïcödé"));
        var eml =
            "From: sender@example.com\r\n" +
            "To: recipient@example.com\r\n" +
            $"Subject: =?utf-8?B?{encoded}?=\r\n\r\n" +
            "Body.\r\n";

        using var message = EmlParser.Parse(eml);

        Assert.Equal("Ünïcödé", message.Subject);
    }



    [Fact]
    public void Parse_when_encoded_word_is_malformed_leaves_it_verbatim()
    {
        // Invalid base64 payload -> decode throws -> the raw token is kept.
        const string eml =
            "From: sender@example.com\r\n" +
            "To: recipient@example.com\r\n" +
            "Subject: =?utf-8?B?not valid base64!?=\r\n\r\n" +
            "Body.\r\n";

        using var message = EmlParser.Parse(eml);

        Assert.Contains("=?utf-8?B?", message.Subject, StringComparison.Ordinal);
    }



    [Fact]
    public void Parse_when_From_is_bare_angle_address_reads_email()
    {
        const string eml =
            "From: <sender@example.com>\r\n" +
            "To: recipient@example.com\r\n" +
            "Subject: Bare angle\r\n\r\n" +
            "Body.\r\n";

        using var message = EmlParser.Parse(eml);

        Assert.Equal("sender@example.com", message.From!.Address);
    }



    [Fact]
    public void Parse_when_recipient_display_name_contains_comma_does_not_split_it()
    {
        const string eml =
            "From: sender@example.com\r\n" +
            "To: \"Doe, John\" <john@example.com>, jane@example.com\r\n" +
            "Subject: Quoted comma\r\n\r\n" +
            "Body.\r\n";

        using var message = EmlParser.Parse(eml);

        Assert.Equal(2, message.To.Count);
        Assert.Contains(message.To, a => string.Equals(a.Address, "john@example.com", StringComparison.Ordinal));
        Assert.Contains(message.To, a => string.Equals(a.Address, "jane@example.com", StringComparison.Ordinal));
    }
}
