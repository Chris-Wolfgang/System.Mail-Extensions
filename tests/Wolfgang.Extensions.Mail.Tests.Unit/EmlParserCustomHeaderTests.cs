using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Custom headers that <see cref="System.Net.Mail.MailMessage.Headers"/> refuses: the lenient
/// parser must skip them instead of throwing, and strict mode must report a malformed name.
/// </summary>
public class EmlParserCustomHeaderTests
{

    [Theory]
    [InlineData("X-Empty:", "X-Empty")]
    [InlineData("X-Blank:    ", "X-Blank")]
    [InlineData("X-Priority:   ", "X-Priority")]
    public void Parse_when_a_custom_header_is_empty_skips_it(string headerLine, string headerName)
    {
        using var msg = EmlParser.Parse("From: a@example.com\r\n" + headerLine + "\r\nX-Kept: yes\r\n\r\nBody");

        Assert.Equal("Body", msg.Body);
        Assert.Equal("yes", msg.Headers["X-Kept"]);
        Assert.Null(msg.Headers[headerName]);
    }



    [Fact]
    public void ParseWithDiagnostics_when_a_custom_header_is_empty_reports_nothing()
    {
        var result = EmlParser.ParseWithDiagnostics("From: a@example.com\r\nX-Empty:\r\n\r\nBody");

        using var msg = result.Message;
        Assert.Empty(result.Issues);
    }



    [Fact]
    public void ParseWithDiagnostics_when_a_header_name_is_not_ascii_skips_and_reports_it()
    {
        var result = EmlParser.ParseWithDiagnostics("From: a@example.com\r\nX-Ü: value\r\nX-Kept: yes\r\n\r\nBody");

        using var msg = result.Message;
        Assert.Equal("Body", msg.Body);
        Assert.Equal("yes", msg.Headers["X-Kept"]);
        var issue = Assert.Single(result.Issues);
        Assert.Equal("Malformed header name 'X-Ü'; header skipped.", issue.Message);
        Assert.Equal("Headers", issue.PropertyName);
    }



    [Fact]
    public void Parse_strict_when_a_header_name_is_not_ascii_throws_EmlParseException()
    {
        var ex = Assert.Throws<EmlParseException>
        (
            () => EmlParser.Parse("From: a@example.com\r\nX-Ü: value\r\n\r\nBody", new EmlParserOptions { Strict = true })
        );

        Assert.Equal("Malformed header name 'X-Ü'; header skipped.", ex.Message);
    }



    /// <summary>
    /// A header line that starts with a whitespace character other than space or tab (which would
    /// make it a folded continuation), such as a bare carriage return or a form feed, has a field
    /// name that trims to nothing. Found by the fuzz suite.
    /// </summary>
    [Theory]
    [InlineData("\r: value")]
    [InlineData("\f: value")]
    [InlineData("\v\r: value")]
    public void Parse_when_a_header_name_is_only_whitespace_skips_it(string headerLine)
    {
        using var msg = EmlParser.Parse("From: a@example.com\n" + headerLine + "\nX-Kept: yes\n\nBody");

        Assert.Equal("Body", msg.Body);
        Assert.Equal("yes", msg.Headers["X-Kept"]);
        Assert.Equal("a@example.com", msg.From!.Address);
    }



    [Fact]
    public void ParseWithDiagnostics_when_a_header_name_is_only_whitespace_reports_it()
    {
        var result = EmlParser.ParseWithDiagnostics("From: a@example.com\n\r: value\n\nBody");

        using var msg = result.Message;
        Assert.Equal("Body", msg.Body);
        var issue = Assert.Single(result.Issues);
        Assert.Equal("Header with an empty field name; header skipped.", issue.Message);
        Assert.Equal("Headers", issue.PropertyName);
    }



    [Fact]
    public void Parse_strict_when_a_header_name_is_only_whitespace_throws_EmlParseException()
    {
        var ex = Assert.Throws<EmlParseException>
        (
            () => EmlParser.Parse("From: a@example.com\n\f: value\n\nBody", new EmlParserOptions { Strict = true })
        );

        Assert.Equal("Header with an empty field name; header skipped.", ex.Message);
    }
}
