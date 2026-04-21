using System.Net.Mail;
using Xunit;
using Assert = Xunit.Assert;
#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;

public class MailAddressExtensionsTests
{
    // ---------- TryParse(string, out MailAddress) ----------

    [Fact]
    public void TryParse_when_valid_address_returns_true_and_parsed_address()
    {
        var success = MailAddress.TryParse("user@example.com", out var result);

        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal("user@example.com", result!.Address);
    }



    [Fact]
    public void TryParse_when_invalid_address_returns_false_and_null()
    {
        var success = MailAddress.TryParse("not-an-email", out var result);

        Assert.False(success);
        Assert.Null(result);
    }



    [Fact]
    public void TryParse_when_null_returns_false()
    {
        var success = MailAddress.TryParse(null, out var result);

        Assert.False(success);
        Assert.Null(result);
    }



    [Fact]
    public void TryParse_when_empty_string_returns_false()
    {
        var success = MailAddress.TryParse("", out var result);

        Assert.False(success);
        Assert.Null(result);
    }



    [Fact]
    public void TryParse_when_whitespace_returns_false()
    {
        var success = MailAddress.TryParse("   ", out var result);

        Assert.False(success);
        Assert.Null(result);
    }



    [Fact]
    public void TryParse_when_address_with_display_name_returns_parsed_address()
    {
        var success = MailAddress.TryParse("\"John Doe\" <john@example.com>", out var result);

        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal("john@example.com", result!.Address);
    }



    // ---------- TryParse(string, string, out MailAddress) ----------

    [Fact]
    public void TryParse_with_displayName_when_valid_returns_true_with_display_name()
    {
        var success = MailAddress.TryParse("user@example.com", "John Doe", out var result);

        Assert.True(success);
        Assert.NotNull(result);
        Assert.Equal("user@example.com", result!.Address);
        Assert.Equal("John Doe", result.DisplayName);
    }



    [Fact]
    public void TryParse_with_displayName_when_invalid_address_returns_false()
    {
        var success = MailAddress.TryParse("bad", "Name", out var result);

        Assert.False(success);
        Assert.Null(result);
    }



    [Fact]
    public void TryParse_with_displayName_when_null_address_returns_false()
    {
        var success = MailAddress.TryParse(null, "Name", out var result);

        Assert.False(success);
        Assert.Null(result);
    }
}
