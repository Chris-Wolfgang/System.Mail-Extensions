using System;
using System.Linq;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Boundary cases of <see cref="EmlParser"/> that the Stryker run reported as surviving
/// mutants (issue #280): bodies that are exactly a line break, a header line that starts with a colon,
/// multipart edge cases, line-wrapped base64 parts, quoted-printable next to non-ASCII text,
/// and adjacent encoded words in different charsets.
/// </summary>
public class EmlParserBoundaryCaseTests
{

    private const string Envelope = "From: f@example.com\r\nTo: t@example.com\r\n";



    [Theory]
    [InlineData("=0D=0A")]
    [InlineData("=0A")]
    public void Parse_when_quoted_printable_body_is_only_a_line_break_returns_empty_body(string encodedBody)
    {
        using var msg = EmlParser.Parse
        (
            Envelope + "Content-Transfer-Encoding: quoted-printable\r\n\r\n" + encodedBody
        );

        Assert.Equal(string.Empty, msg.Body);
    }



    [Fact]
    public void ParseWithDiagnostics_when_header_line_starts_with_a_colon_ignores_it_silently()
    {
        var result = EmlParser.ParseWithDiagnostics(Envelope + ":orphan value\r\n\r\nBody");
        using var msg = result.Message;

        Assert.Empty(result.Issues);
        Assert.Equal("Body", msg.Body);
    }



    [Fact]
    public void Parse_when_multipart_text_contains_a_double_hyphen_keeps_it()
    {
        using var msg = EmlParser.Parse
        (
            Envelope
            + "Content-Type: multipart/mixed; boundary=\"XYZ\"\r\n\r\n"
            + "--XYZ\r\nContent-Type: text/plain\r\n\r\npre--post\r\n"
            + "--XYZ--\r\n"
        );

        Assert.Equal("pre--post", msg.Body);
    }



    [Fact]
    public void Parse_when_multipart_has_no_boundary_parameter_leaves_the_body_empty()
    {
        using var msg = EmlParser.Parse
        (
            Envelope
            + "Content-Type: multipart/mixed\r\n\r\n"
            + "--\r\nContent-Type: text/plain\r\n\r\nhello\r\n"
        );

        Assert.True(string.IsNullOrEmpty(msg.Body));
        Assert.Empty(msg.Attachments);
    }



    [Fact]
    public void Parse_when_epilogue_after_closing_delimiter_looks_like_a_part_ignores_it()
    {
        using var msg = EmlParser.Parse
        (
            Envelope
            + "Content-Type: multipart/mixed; boundary=\"B\"\r\n\r\n"
            + "--B\r\nContent-Type: text/plain\r\n\r\nhello\r\n"
            + "--B--\r\nContent-Type: application/octet-stream\r\n\r\nzzz\r\n"
        );

        Assert.Equal("hello", msg.Body);
        Assert.Empty(msg.Attachments);
    }



    [Fact]
    public void Parse_when_closing_delimiter_is_missing_keeps_the_last_part()
    {
        using var msg = EmlParser.Parse
        (
            Envelope
            + "Content-Type: multipart/mixed; boundary=\"B\"\r\n\r\n"
            + "--B\r\nContent-Type: text/plain\r\n\r\nhello"
        );

        Assert.Equal("hello", msg.Body);
    }



    [Fact]
    public void Parse_when_text_part_has_inline_disposition_uses_it_as_the_body()
    {
        using var msg = EmlParser.Parse
        (
            Envelope
            + "Content-Type: multipart/mixed; boundary=\"B\"\r\n\r\n"
            + "--B\r\nContent-Type: text/plain\r\nContent-Disposition: inline\r\n\r\nhello\r\n"
            + "--B--\r\n"
        );

        Assert.Equal("hello", msg.Body);
        Assert.Empty(msg.Attachments);
    }



    [Fact]
    public void ParseWithDiagnostics_when_base64_attachment_is_line_wrapped_decodes_it()
    {
        var bytes = Enumerable.Range(0, 120).Select(i => (byte)i).ToArray();
        var base64 = Convert.ToBase64String(bytes);
        var wrapped = base64.Substring(0, 76) + "\r\n" + base64.Substring(76);

        var result = EmlParser.ParseWithDiagnostics
        (
            Envelope
            + "Content-Type: multipart/mixed; boundary=\"B\"\r\n\r\n"
            + "--B\r\nContent-Type: application/octet-stream; name=\"data.bin\"\r\n"
            + "Content-Transfer-Encoding: base64\r\n"
            + "Content-Disposition: attachment; filename=\"data.bin\"\r\n\r\n"
            + wrapped + "\r\n"
            + "--B--\r\n"
        );
        using var msg = result.Message;

        Assert.Empty(result.Issues);
        var attachment = Assert.Single(msg.Attachments);
        using var copy = new System.IO.MemoryStream();
        attachment.ContentStream.CopyTo(copy);
        Assert.Equal(bytes, copy.ToArray());
    }



    [Fact]
    public void ParseWithDiagnostics_when_quoted_printable_attachment_has_non_ascii_text_before_an_escape_decodes_both()
    {
        var result = EmlParser.ParseWithDiagnostics
        (
            Envelope
            + "Content-Type: multipart/mixed; boundary=\"B\"\r\n\r\n"
            + "--B\r\nContent-Type: application/octet-stream; name=\"t.txt\"\r\n"
            + "Content-Transfer-Encoding: quoted-printable\r\n"
            + "Content-Disposition: attachment; filename=\"t.txt\"\r\n\r\n"
            + "é=41\r\n"
            + "--B--\r\n"
        );
        using var msg = result.Message;

        var attachment = Assert.Single(msg.Attachments);
        using var copy = new System.IO.MemoryStream();
        attachment.ContentStream.CopyTo(copy);
        Assert.Equal("éA", System.Text.Encoding.UTF8.GetString(copy.ToArray()));
    }



    [Fact]
    public void Parse_when_adjacent_encoded_words_use_different_charsets_decodes_each_in_its_own()
    {
        using var msg = EmlParser.Parse
        (
            Envelope + "Subject: =?ISO-8859-1?Q?=E9?= =?UTF-8?Q?=C3=A9?=\r\n\r\nBody"
        );

        Assert.Equal("éé", msg.Subject);
    }
}
