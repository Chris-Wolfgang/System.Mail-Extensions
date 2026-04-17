using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;
#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;

public class EmlParserTests
{
    // ---------- Null guards ----------

    [Fact]
    public void Parse_when_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => EmlParser.Parse(null!)
        );
        Assert.Equal("emlContent", ex.ParamName);
    }



    [Fact]
    public void ParseFile_when_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => EmlParser.ParseFile(null!)
        );
        Assert.Equal("filePath", ex.ParamName);
    }



    [Fact]
    public async Task ParseFileAsync_when_null_throws_ArgumentNullException()
    {
        var ex = await Assert.ThrowsAsync<ArgumentNullException>
        (
            () => EmlParser.ParseFileAsync(null!)
        );
        Assert.Equal("filePath", ex.ParamName);
    }



    // ---------- Basic parsing ----------

    [Fact]
    public void Parse_when_simple_text_message_extracts_headers_and_body()
    {
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: recipient@example.com",
            "Subject: Test Subject",
            "MIME-Version: 1.0",
            "Content-Type: text/plain",
            "",
            "Hello, World!"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal("sender@example.com", msg.From!.Address);
        Assert.Equal(1, msg.To.Count);
        Assert.Equal("recipient@example.com", msg.To[0].Address);
        Assert.Equal("Test Subject", msg.Subject);
        Assert.Equal("Hello, World!", msg.Body);
        Assert.False(msg.IsBodyHtml);
    }



    [Fact]
    public void Parse_when_html_message_sets_IsBodyHtml()
    {
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: recipient@example.com",
            "Subject: HTML Test",
            "Content-Type: text/html",
            "",
            "<h1>Hello</h1>"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.True(msg.IsBodyHtml);
        Assert.Contains("<h1>Hello</h1>", msg.Body);
    }



    // ---------- Address parsing ----------

    [Fact]
    public void Parse_when_display_name_in_From_extracts_name_and_address()
    {
        var eml = BuildEml
        (
            "From: \"John Doe\" <john@example.com>",
            "To: recipient@example.com",
            "",
            "Body"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal("john@example.com", msg.From!.Address);
        Assert.Equal("John Doe", msg.From.DisplayName);
    }



    [Fact]
    public void Parse_when_multiple_To_addresses_parses_all()
    {
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: a@example.com, b@example.com, c@example.com",
            "",
            "Body"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal(3, msg.To.Count);
    }



    [Fact]
    public void Parse_when_CC_and_BCC_present_parses_both()
    {
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: to@example.com",
            "CC: cc@example.com",
            "BCC: bcc@example.com",
            "",
            "Body"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal(1, msg.CC.Count);
        Assert.Equal(1, msg.Bcc.Count);
    }



    // ---------- Encoded subjects ----------

    [Fact]
    public void Parse_when_subject_is_base64_encoded_decodes_it()
    {
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("Héllo Wörld"));
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: to@example.com",
            $"Subject: =?UTF-8?B?{encoded}?=",
            "",
            "Body"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal("Héllo Wörld", msg.Subject);
    }



    [Fact]
    public void Parse_when_subject_is_q_encoded_decodes_it()
    {
        // "Héllo" in UTF-8 Q-encoding: H=C3=A9llo
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: to@example.com",
            "Subject: =?UTF-8?Q?H=C3=A9llo?=",
            "",
            "Body"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal("Héllo", msg.Subject);
    }



    // ---------- Base64 body ----------

    [Fact]
    public void Parse_when_body_is_base64_encoded_decodes_it()
    {
        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("Decoded content"));
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: to@example.com",
            "Content-Type: text/plain",
            "Content-Transfer-Encoding: base64",
            "",
            encoded
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal("Decoded content", msg.Body);
    }



    // ---------- Quoted-printable ----------

    [Fact]
    public void Parse_when_body_is_quoted_printable_decodes_it()
    {
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: to@example.com",
            "Content-Type: text/plain",
            "Content-Transfer-Encoding: quoted-printable",
            "",
            "Hello =C3=A9 World"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Contains("é", msg.Body);
    }



    // ---------- Multipart ----------

    [Fact]
    public void Parse_when_multipart_mixed_extracts_body_and_attachment()
    {
        var attachmentContent = Convert.ToBase64String(new byte[] { 1, 2, 3 });
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: to@example.com",
            "Subject: Multipart Test",
            "Content-Type: multipart/mixed; boundary=\"BOUNDARY\"",
            "",
            "--BOUNDARY",
            "Content-Type: text/plain",
            "",
            "Plain text body",
            "--BOUNDARY",
            "Content-Type: application/octet-stream",
            "Content-Disposition: attachment; filename=\"data.bin\"",
            "Content-Transfer-Encoding: base64",
            "",
            attachmentContent,
            "--BOUNDARY--"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal("Plain text body", msg.Body);
        Assert.Equal(1, msg.Attachments.Count);
        Assert.Equal("data.bin", msg.Attachments[0].Name);
    }



    // ---------- Priority ----------

    [Fact]
    public void Parse_when_x_priority_1_sets_high_priority()
    {
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: to@example.com",
            "X-Priority: 1",
            "",
            "Body"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal(MailPriority.High, msg.Priority);
    }



    // ---------- Custom headers ----------

    [Fact]
    public void Parse_when_custom_headers_present_preserves_them()
    {
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: to@example.com",
            "X-Custom-Header: custom-value",
            "",
            "Body"
        );

        using var msg = EmlParser.Parse(eml);

        Assert.Equal("custom-value", msg.Headers["X-Custom-Header"]);
    }



    // ---------- Round-trip ----------

    [Fact]
    public void Parse_round_trip_with_ToMimeString_preserves_key_fields()
    {
        using var original = new MailMessage("from@example.com", "to@example.com");
        original.Subject = "Round Trip";
        original.Body = "Test body";

        var eml = original.ToMimeString();
        using var parsed = EmlParser.Parse(eml);

        Assert.Equal("from@example.com", parsed.From!.Address);
        Assert.Contains(parsed.To, a => a.Address == "to@example.com");
        Assert.Equal("Round Trip", parsed.Subject);
        Assert.Contains("Test body", parsed.Body);
    }



    // ---------- File operations ----------

    [Fact]
    public void ParseFile_when_valid_file_parses_content()
    {
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: to@example.com",
            "Subject: File Test",
            "",
            "File body"
        );

        var filePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.eml");

        try
        {
            File.WriteAllText(filePath, eml);
            using var msg = EmlParser.ParseFile(filePath);

            Assert.Equal("File Test", msg.Subject);
        }
        finally
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }



    [Fact]
    public async Task ParseFileAsync_when_valid_file_parses_content()
    {
        var eml = BuildEml
        (
            "From: sender@example.com",
            "To: to@example.com",
            "Subject: Async File Test",
            "",
            "Async body"
        );

        var filePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.eml");

        try
        {
            File.WriteAllText(filePath, eml);
            using var msg = await EmlParser.ParseFileAsync(filePath);

            Assert.Equal("Async File Test", msg.Subject);
        }
        finally
        {
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }



    // ---------- Helpers ----------

    private static string BuildEml
    (
        params string[] lines
    )
    {
        return string.Join("\r\n", lines);
    }
}
