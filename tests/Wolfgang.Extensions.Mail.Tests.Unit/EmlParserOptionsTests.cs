using System.Net.Mail;
using Wolfgang.Extensions.Mail;
using Wolfgang.Extensions.Mail.Validation;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Covers <see cref="EmlParserOptions"/> strict mode and
/// <see cref="EmlParser.ParseWithDiagnostics(string, EmlParserOptions?)"/>.
/// </summary>
public class EmlParserOptionsTests
{

    private const string MalformedFromEml =
        "From: @@@\r\nTo: recipient@example.com\r\nSubject: Test\r\n\r\nBody.\r\n";

    private const string MalformedToEml =
        "From: sender@example.com\r\nTo: good@example.com, @@@\r\nSubject: Test\r\n\r\nBody.\r\n";

    private const string UndecodableBase64Eml =
        "From: sender@example.com\r\nTo: recipient@example.com\r\n" +
        "Content-Type: text/plain; charset=utf-8\r\n" +
        "Content-Transfer-Encoding: base64\r\n\r\n" +
        "!!!not-valid-base64!!!\r\n";

    private const string MalformedEncodedWordEml =
        "From: sender@example.com\r\nTo: recipient@example.com\r\n" +
        "Subject: =?utf-8?B?not valid base64!?=\r\n\r\nBody.\r\n";



    // ---------- Strict mode throws ----------

    [Fact]
    public void Parse_strict_when_From_is_malformed_throws_EmlParseException()
    {
        Assert.Throws<EmlParseException>
        (
            () => EmlParser.Parse(MalformedFromEml, new EmlParserOptions { Strict = true })
        );
    }



    [Fact]
    public void Parse_strict_when_recipient_address_is_malformed_throws_EmlParseException()
    {
        Assert.Throws<EmlParseException>
        (
            () => EmlParser.Parse(MalformedToEml, new EmlParserOptions { Strict = true })
        );
    }



    [Fact]
    public void Parse_strict_when_base64_body_is_undecodable_throws_EmlParseException()
    {
        Assert.Throws<EmlParseException>
        (
            () => EmlParser.Parse(UndecodableBase64Eml, new EmlParserOptions { Strict = true })
        );
    }



    [Fact]
    public void Parse_strict_when_encoded_word_is_malformed_throws_EmlParseException()
    {
        Assert.Throws<EmlParseException>
        (
            () => EmlParser.Parse(MalformedEncodedWordEml, new EmlParserOptions { Strict = true })
        );
    }



    [Fact]
    public void EmlParseException_is_a_FormatException_so_existing_catch_blocks_work()
    {
        var caught = Assert.Throws<EmlParseException>
        (
            () => EmlParser.Parse(MalformedFromEml, new EmlParserOptions { Strict = true })
        );

        Assert.IsAssignableFrom<System.FormatException>(caught);
    }



    [Fact]
    public void Parse_strict_when_base64_attachment_is_undecodable_throws_EmlParseException()
    {
        const string eml =
            "From: sender@example.com\r\nTo: recipient@example.com\r\n" +
            "Content-Type: multipart/mixed; boundary=\"B\"\r\n\r\n" +
            "--B\r\nContent-Type: text/plain\r\n\r\nSee attachment.\r\n" +
            "--B\r\n" +
            "Content-Type: application/octet-stream; name=\"data.bin\"\r\n" +
            "Content-Transfer-Encoding: base64\r\n" +
            "Content-Disposition: attachment; filename=\"data.bin\"\r\n\r\n" +
            "!!!not-valid-base64!!!\r\n" +
            "--B--\r\n";

        Assert.Throws<EmlParseException>
        (
            () => EmlParser.Parse(eml, new EmlParserOptions { Strict = true })
        );
    }



    // ---------- Lenient behavior unchanged ----------

    [Fact]
    public void Parse_default_when_From_is_malformed_skips_silently()
    {
        using var message = EmlParser.Parse(MalformedFromEml);

        Assert.Null(message.From);
        Assert.Equal("recipient@example.com", message.To.Single().Address);
    }



    [Fact]
    public void Parse_non_strict_options_matches_default_lenient_behavior()
    {
        using var message = EmlParser.Parse(MalformedToEml, new EmlParserOptions { Strict = false });

        // The good recipient survives; the malformed one is skipped, not thrown.
        Assert.Equal("good@example.com", message.To.Single().Address);
    }



    // ---------- Diagnostics ----------

    [Fact]
    public void ParseWithDiagnostics_when_From_is_malformed_reports_the_issue()
    {
        var result = EmlParser.ParseWithDiagnostics(MalformedFromEml);

        Assert.True(result.HasIssues);
        var issue = Assert.Single(result.Issues);
        Assert.Equal(ValidationSeverity.Warning, issue.Severity);
        Assert.Equal("From", issue.PropertyName);
    }



    [Fact]
    public void ParseWithDiagnostics_when_content_is_clean_has_no_issues()
    {
        const string clean =
            "From: sender@example.com\r\nTo: recipient@example.com\r\nSubject: Clean\r\n\r\nBody.\r\n";

        var result = EmlParser.ParseWithDiagnostics(clean);

        Assert.False(result.HasIssues);
        Assert.Empty(result.Issues);
        Assert.Equal("sender@example.com", result.Message.From!.Address);
    }



    [Fact]
    public void ParseWithDiagnostics_populates_the_message_best_effort_alongside_issues()
    {
        var result = EmlParser.ParseWithDiagnostics(MalformedToEml);

        // The good recipient is still parsed even though a sibling was skipped.
        Assert.Equal("good@example.com", result.Message.To.Single().Address);
        Assert.True(result.HasIssues);
        Assert.Equal("To", Assert.Single(result.Issues).PropertyName);
    }



    [Fact]
    public void ParseWithDiagnostics_when_strict_throws_instead_of_reporting()
    {
        Assert.Throws<EmlParseException>
        (
            () => EmlParser.ParseWithDiagnostics(MalformedFromEml, new EmlParserOptions { Strict = true })
        );
    }



    // ---------- Null guards ----------

    [Fact]
    public void Parse_when_options_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => EmlParser.Parse("From: a@b.com\r\n\r\n", (EmlParserOptions)null!)
        );
    }



    [Fact]
    public void ParseWithDiagnostics_when_content_is_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => EmlParser.ParseWithDiagnostics(null!)
        );
    }
}
