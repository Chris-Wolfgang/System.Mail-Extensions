using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// An address whose angle brackets hold nothing must be reported as malformed, not thrown:
/// <see cref="System.Net.Mail.MailAddress"/> raises <see cref="System.ArgumentException"/> for an empty
/// address, which lenient parsing has to treat like any other bad address (found by the fuzz run).
/// </summary>
public class EmlParserEmptyAddressTests
{

    [Theory]
    [InlineData("Cc: -@=_< >")]
    [InlineData("Cc: Name <>")]
    [InlineData("Cc: \"Name\" < >")]
    public void ParseWithDiagnostics_when_angle_brackets_are_empty_reports_malformed_address(string header)
    {
        var result = EmlParser.ParseWithDiagnostics
        (
            "From: f@example.com\r\nTo: t@example.com\r\n" + header + "\r\n\r\nBody"
        );
        using var msg = result.Message;

        Assert.Empty(msg.CC);
        Assert.NotEmpty(result.Issues);
    }
}
