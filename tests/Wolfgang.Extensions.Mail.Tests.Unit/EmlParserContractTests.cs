using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Pins parser behaviour that the earlier suites exercised without asserting: which headers
/// are treated as standard, the header/body boundary, transfer-encoding edge positions,
/// RFC 2047 variants, multipart shapes, address-list splitting and the diagnostics text.
/// Each test names the contract it holds; several were added to kill Stryker survivors (#280).
/// </summary>
public class EmlParserContractTests
{

    // ---------- Standard vs custom headers ----------

    [Fact]
    public void Parse_when_standard_headers_present_does_not_copy_them_into_Headers()
    {
        var eml = Eml
        (
            "From: from@example.com",
            "To: to@example.com",
            "Cc: cc@example.com",
            "Bcc: bcc@example.com",
            "Reply-To: reply@example.com",
            "Subject: Hello",
            "Content-Type: text/plain",
            "Content-Transfer-Encoding: 7bit",
            "MIME-Version: 1.0",
            "Date: Mon, 1 Jan 2024 00:00:00 +0000",
            "Message-ID: <id@example.com>",
            "X-Custom: kept",
            "",
            "Body"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal("kept", msg.Headers["X-Custom"]);
        foreach (var name in new[] { "From", "To", "Cc", "Bcc", "Reply-To", "Subject", "Content-Type", "Content-Transfer-Encoding", "MIME-Version", "Date", "Message-ID" })
        {
            Assert.True(msg.Headers[name] == null, $"Standard header '{name}' was copied into Headers.");
        }
    }



    // ---------- Header / body boundary ----------

    [Theory]
    [InlineData("\r\n\r\nBody only")]
    [InlineData("\n\nBody only")]
    public void Parse_when_content_starts_with_the_blank_line_reads_everything_after_it_as_body(string eml)
    {
        using var msg = EmlParser.Parse(eml);

        Assert.Equal("Body only", msg.Body);
    }



    [Fact]
    public void Parse_when_there_is_no_blank_line_reads_headers_and_leaves_the_body_empty()
    {
        using var msg = EmlParser.Parse("From: from@example.com\r\nSubject: Headers only");

        Assert.Equal("from@example.com", msg.From!.Address);
        Assert.Equal("Headers only", msg.Subject);
        Assert.Equal(string.Empty, msg.Body);
    }



    [Fact]
    public void Parse_when_a_line_starts_with_a_colon_ends_the_previous_header()
    {
        // ":orphan" has no header name. It must end "Subject", so the folded line after it
        // is not appended to the subject.
        var eml = Eml
        (
            "From: from@example.com",
            "Subject: Hi",
            ":orphan",
            " continued",
            "",
            "Body"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal("Hi", msg.Subject);
    }



    // ---------- X-Priority ----------

    [Theory]
    [InlineData("4", MailPriority.Low)]
    [InlineData("5 (Lowest)", MailPriority.Low)]
    [InlineData("   ", MailPriority.Normal)]
    [InlineData("3", MailPriority.Normal)]
    public void Parse_when_X_Priority_is_set_maps_it_to_Priority(string value, MailPriority expected)
    {
        using var msg = EmlParser.Parse(Eml("From: from@example.com", "X-Priority: " + value, "", "Body"));

        Assert.Equal(expected, msg.Priority);
    }



    // ---------- Diagnostics ----------

    [Fact]
    public void ParseWithDiagnostics_when_From_is_blank_reports_nothing()
    {
        var result = EmlParser.ParseWithDiagnostics(Eml("From:    ", "To: to@example.com", "", "Body"));

        using var msg = result.Message;
        Assert.Null(msg.From);
        Assert.Empty(result.Issues);
    }



    [Fact]
    public void ParseWithDiagnostics_when_From_is_malformed_reports_the_value_and_property()
    {
        var result = EmlParser.ParseWithDiagnostics(Eml("From: not an address", "To: to@example.com", "", "Body"));

        using var msg = result.Message;
        var issue = Assert.Single(result.Issues);
        Assert.Equal("Malformed From address: 'not an address'.", issue.Message);
        Assert.Equal("From", issue.PropertyName);
    }



    [Theory]
    [InlineData("Cc", "CC")]
    [InlineData("Bcc", "Bcc")]
    [InlineData("Reply-To", "ReplyTo")]
    public void ParseWithDiagnostics_when_an_address_header_has_a_malformed_entry_names_the_property(string header, string property)
    {
        var result = EmlParser.ParseWithDiagnostics(Eml("From: from@example.com", "To: to@example.com", header + ": bad address", "", "Body"));

        using var msg = result.Message;
        var issue = Assert.Single(result.Issues);
        Assert.Equal($"Malformed {property} address: 'bad address'.", issue.Message);
        Assert.Equal(property, issue.PropertyName);
    }



    [Fact]
    public void ParseWithDiagnostics_when_base64_body_is_undecodable_reports_it()
    {
        var result = EmlParser.ParseWithDiagnostics
        (
            Eml("From: from@example.com", "Content-Type: text/plain", "Content-Transfer-Encoding: base64", "", "not base64 !!!")
        );

        using var msg = result.Message;
        var issue = Assert.Single(result.Issues);
        Assert.Equal("Undecodable base64 body part; kept as raw text.", issue.Message);
        Assert.Equal("Body", issue.PropertyName);
    }



    [Fact]
    public void ParseWithDiagnostics_when_base64_attachment_is_undecodable_reports_it()
    {
        var eml = Eml
        (
            "From: from@example.com",
            "Content-Type: multipart/mixed; boundary=\"b1\"",
            "",
            "--b1",
            "Content-Type: text/plain",
            "",
            "Body",
            "--b1",
            "Content-Type: application/octet-stream; name=\"data.bin\"",
            "Content-Transfer-Encoding: base64",
            "Content-Disposition: attachment; filename=\"data.bin\"",
            "",
            "not base64 !!!",
            "--b1--"
        );

        var result = EmlParser.ParseWithDiagnostics(eml);

        using var msg = result.Message;
        var issue = Assert.Single(result.Issues);
        Assert.Equal("Undecodable base64 attachment part; kept as raw bytes.", issue.Message);
        Assert.Equal("Attachments", issue.PropertyName);
    }



    // ---------- Null arguments ----------

    [Fact]
    public void ParseWithDiagnostics_when_content_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => EmlParser.ParseWithDiagnostics(null!));

        Assert.Equal("emlContent", ex.ParamName);
    }



    [Fact]
    public void ParseFile_with_options_when_arguments_are_null_names_the_parameter()
    {
        var pathEx = Assert.Throws<ArgumentNullException>(() => EmlParser.ParseFile(null!, new EmlParserOptions()));
        var optionsEx = Assert.Throws<ArgumentNullException>(() => EmlParser.ParseFile("unused.eml", null!));

        Assert.Equal("filePath", pathEx.ParamName);
        Assert.Equal("options", optionsEx.ParamName);
    }



    [Fact]
    public async Task ParseFileAsync_with_options_when_arguments_are_null_names_the_parameter()
    {
        var pathEx = await Assert.ThrowsAsync<ArgumentNullException>(() => EmlParser.ParseFileAsync(null!, new EmlParserOptions()));
        var optionsEx = await Assert.ThrowsAsync<ArgumentNullException>(() => EmlParser.ParseFileAsync("unused.eml", null!));

        Assert.Equal("filePath", pathEx.ParamName);
        Assert.Equal("options", optionsEx.ParamName);
    }



    // ---------- Quoted-printable edge positions ----------

    [Theory]
    [InlineData("abc=", "abc=")]
    [InlineData("ab=4", "ab=4")]
    [InlineData("a=4Fb", "aOb")]
    [InlineData("=4f=4F=30=39=61=66=41=46", "OO09afAF")]
    [InlineData("soft=\r\nbreak", "softbreak")]
    [InlineData("soft=\nbreak", "softbreak")]
    [InlineData("x=\r", "x=\r")]
    public void Parse_when_body_is_quoted_printable_decodes_edge_positions(string encoded, string expected)
    {
        using var msg = EmlParser.Parse
        (
            Eml("From: from@example.com", "Content-Type: text/plain; charset=utf-8", "Content-Transfer-Encoding: quoted-printable", "", encoded)
        );

        Assert.Equal(expected, msg.Body);
    }



    [Theory]
    [InlineData("\u0080")]
    [InlineData("café")]
    [InlineData("smile \U0001F600 end")]
    public void Parse_when_quoted_printable_body_has_raw_non_ascii_keeps_it(string text)
    {
        using var msg = EmlParser.Parse
        (
            Eml("From: from@example.com", "Content-Type: text/plain; charset=utf-8", "Content-Transfer-Encoding: quoted-printable", "", text)
        );

        Assert.Equal(text, msg.Body);
    }



    [Fact]
    public void Parse_when_quoted_printable_body_ends_with_a_lone_high_surrogate_keeps_the_text_before_it()
    {
        using var msg = EmlParser.Parse
        (
            Eml("From: from@example.com", "Content-Type: text/plain; charset=utf-8", "Content-Transfer-Encoding: quoted-printable", "", "end\uD83D")
        );

        Assert.StartsWith("end", msg.Body, StringComparison.Ordinal);
        Assert.Equal(4, msg.Body.Length);
    }



    // ---------- RFC 2047 encoded words ----------

    [Theory]
    [InlineData("=?utf-8?b?SGVsbG8=?=", "Hello")]
    [InlineData("=?UTF-8?q?Hello_World?=", "Hello World")]
    [InlineData("=?utf-8?Q?a=4?=", "a=4")]
    [InlineData("=?utf-8?Q?caf=C3=A9?=", "café")]
    public void Parse_when_subject_is_an_encoded_word_decodes_it(string subject, string expected)
    {
        using var msg = EmlParser.Parse(Eml("From: from@example.com", "Subject: " + subject, "", "Body"));

        Assert.Equal(expected, msg.Subject);
    }



    [Fact]
    public void ParseWithDiagnostics_when_encoded_word_charset_is_unknown_keeps_it_and_reports_it()
    {
        var result = EmlParser.ParseWithDiagnostics(Eml("From: from@example.com", "Subject: =?x-no-such-charset?Q?abc?=", "", "Body"));

        using var msg = result.Message;
        Assert.Equal("=?x-no-such-charset?Q?abc?=", msg.Subject);
        var issue = Assert.Single(result.Issues);
        Assert.Equal("Malformed RFC 2047 encoded word: '=?x-no-such-charset?Q?abc?='; kept verbatim.", issue.Message);
    }



    [Fact]
    public void ParseWithDiagnostics_when_encoded_word_base64_is_malformed_reports_it()
    {
        var result = EmlParser.ParseWithDiagnostics(Eml("From: from@example.com", "Subject: =?utf-8?B?***?=", "", "Body"));

        using var msg = result.Message;
        var issue = Assert.Single(result.Issues);
        Assert.Equal("Malformed RFC 2047 encoded word: '=?utf-8?B?***?='; kept verbatim.", issue.Message);
    }



    // ---------- Address lists ----------

    [Fact]
    public void Parse_when_a_quoted_display_name_contains_a_comma_keeps_it_in_one_address()
    {
        using var msg = EmlParser.Parse(Eml("From: from@example.com", "To: \"Smith, John\" <john@example.com>, jane@example.com", "", "Body"));

        Assert.Equal(2, msg.To.Count);
        Assert.Equal("john@example.com", msg.To[0].Address);
        Assert.Equal("Smith, John", msg.To[0].DisplayName);
        Assert.Equal("jane@example.com", msg.To[1].Address);
    }



    [Fact]
    public void ParseWithDiagnostics_when_the_address_list_has_a_trailing_comma_reports_nothing()
    {
        var result = EmlParser.ParseWithDiagnostics(Eml("From: from@example.com", "To: a@example.com, b@example.com,", "", "Body"));

        using var msg = result.Message;
        Assert.Equal(2, msg.To.Count);
        Assert.Empty(result.Issues);
    }



    [Fact]
    public void Parse_when_an_angle_address_contains_a_comma_in_its_display_name_keeps_one_address()
    {
        using var msg = EmlParser.Parse(Eml("From: from@example.com", "To: <a@example.com>, \"B, Bee\" <b@example.com>", "", "Body"));

        Assert.Equal(new[] { "a@example.com", "b@example.com" }, msg.To.Select(a => a.Address).ToArray());
    }



    // ---------- Multipart shapes ----------

    [Fact]
    public void Parse_when_multipart_has_no_boundary_parameter_does_not_split_it()
    {
        using var msg = EmlParser.Parse(Eml("From: from@example.com", "Content-Type: multipart/mixed", "", "--x", "Content-Type: text/plain", "", "Hi", "--x--"));

        Assert.Empty(msg.Attachments);
        Assert.Empty(msg.AlternateViews);
    }



    [Fact]
    public void Parse_when_a_part_has_no_blank_line_ignores_it()
    {
        using var msg = EmlParser.Parse
        (
            Eml("From: from@example.com", "Content-Type: multipart/mixed; boundary=\"b1\"", "", "--b1", "Content-Type: text/plain", "--b1", "Content-Type: text/plain", "", "Kept", "--b1--")
        );

        Assert.Equal("Kept", msg.Body);
        Assert.Empty(msg.Attachments);
    }



    [Fact]
    public void Parse_when_a_part_has_no_content_type_treats_it_as_plain_text()
    {
        using var msg = EmlParser.Parse
        (
            Eml("From: from@example.com", "Content-Type: multipart/mixed; boundary=\"b1\"", "", "--b1", "X-Part: 1", "", "Plain part", "--b1--")
        );

        Assert.Equal("Plain part", msg.Body);
        Assert.False(msg.IsBodyHtml);
        Assert.Empty(msg.Attachments);
    }



    [Fact]
    public void Parse_when_two_plain_parts_exist_keeps_the_first_as_body()
    {
        using var msg = EmlParser.Parse
        (
            Eml("From: from@example.com", "Content-Type: multipart/mixed; boundary=\"b1\"", "", "--b1", "Content-Type: text/plain", "", "First", "--b1", "Content-Type: text/plain", "", "Second", "--b1--")
        );

        Assert.Equal("First", msg.Body);
    }



    [Fact]
    public void Parse_when_the_only_text_part_is_html_sets_it_as_an_html_body()
    {
        using var msg = EmlParser.Parse
        (
            Eml("From: from@example.com", "Content-Type: multipart/alternative; boundary=\"b1\"", "", "--b1", "Content-Type: text/html", "", "<p>Hi</p>", "--b1--")
        );

        Assert.True(msg.IsBodyHtml);
        Assert.Equal("<p>Hi</p>", msg.Body);
        Assert.Empty(msg.AlternateViews);
    }



    [Fact]
    public void Parse_when_a_text_part_is_marked_attachment_without_a_name_adds_attachment_bin()
    {
        using var msg = EmlParser.Parse
        (
            Eml("From: from@example.com", "Content-Type: multipart/mixed; boundary=\"b1\"", "", "--b1", "Content-Type: text/plain", "", "Body", "--b1", "Content-Type: text/plain", "Content-Disposition: attachment", "", "Attached text", "--b1--")
        );

        Assert.Equal("Body", msg.Body);
        var attachment = Assert.Single(msg.Attachments);
        Assert.Equal("attachment.bin", attachment.Name);
        Assert.Equal("text/plain", attachment.ContentType.MediaType);
        Assert.False(attachment.ContentStream.CanWrite);
    }



    [Fact]
    public void Parse_when_attachment_name_is_only_in_the_content_type_uses_it()
    {
        using var msg = EmlParser.Parse
        (
            Eml("From: from@example.com", "Content-Type: multipart/mixed; boundary=\"b1\"", "", "--b1", "Content-Type: text/plain", "", "Body", "--b1", "Content-Type: application/pdf; name=\"doc.pdf\"", "Content-Disposition: attachment", "", "PDF", "--b1--")
        );

        var attachment = Assert.Single(msg.Attachments);
        Assert.Equal("doc.pdf", attachment.Name);
    }



    [Fact]
    public void Parse_when_a_part_is_a_nested_multipart_does_not_also_add_it_as_an_attachment()
    {
        var eml = Eml
        (
            "From: from@example.com",
            "Content-Type: multipart/mixed; boundary=\"outer\"",
            "",
            "--outer",
            "Content-Type: multipart/alternative; boundary=\"inner\"",
            "",
            "--inner",
            "Content-Type: text/plain",
            "",
            "Plain",
            "--inner",
            "Content-Type: text/html",
            "",
            "<p>Html</p>",
            "--inner--",
            "--outer--"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal("Plain", msg.Body);
        Assert.Single(msg.AlternateViews);
        Assert.Empty(msg.Attachments);
    }



    private static string Eml(params string[] lines) => string.Join("\r\n", lines);
}
