using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// How <see cref="EmlParser"/> splits an address-list header on commas: a comma inside
/// angle brackets does not split the list, and a <c>&lt;</c> inside a quoted display name
/// does not open an angle-bracket section. Both were surviving Stryker mutants (issue #280).
/// </summary>
public class EmlParserAddressListSplitTests
{

    [Fact]
    public void ParseWithDiagnostics_when_comma_is_inside_angle_brackets_reports_the_address_whole()
    {
        var result = EmlParser.ParseWithDiagnostics
        (
            "From: f@example.com\r\nTo: Name <a,b@example.com>\r\n\r\nBody"
        );
        using var msg = result.Message;

        var issue = Assert.Single
        (
            result.Issues,
            i => string.Equals(i.PropertyName, "To", System.StringComparison.Ordinal)
        );
        Assert.Contains("'Name <a,b@example.com>'", issue.Message, System.StringComparison.Ordinal);
    }



    [Fact]
    public void ParseWithDiagnostics_when_quoted_display_name_contains_a_less_than_sign_still_splits_on_the_next_comma()
    {
        var result = EmlParser.ParseWithDiagnostics
        (
            "From: f@example.com\r\nTo: \"a<b\" c@example.com, d@example.com\r\n\r\nBody"
        );
        using var msg = result.Message;

        Assert.Contains
        (
            msg.To,
            address => string.Equals(address.Address, "d@example.com", System.StringComparison.Ordinal)
        );
    }
}
