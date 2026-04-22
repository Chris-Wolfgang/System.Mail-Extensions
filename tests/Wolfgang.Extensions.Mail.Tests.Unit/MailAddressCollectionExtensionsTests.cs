    using System.Net.Mail;
using Xunit;
using Assert = Xunit.Assert;
// ReSharper disable InvokeAsExtensionMember
#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;

public class MailAddressCollectionExtensionsTests
{
    // ---------- AddRange(IEnumerable<string>) ----------

    [Fact]
    public void AddRange_Enumerable_string_when_source_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailAddressCollectionExtensions.AddRange(null!, (IEnumerable<string>)["a@example.com"])
        );
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void AddRange_Enumerable_string_when_addresses_is_null_throws_ArgumentNullException()
    {
        using var msg = new MailMessage();
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailAddressCollectionExtensions.AddRange(msg.To, (IEnumerable<string>)null!)
        );
        Assert.Equal("addresses", ex.ParamName);
    }



    [Fact]
    public void AddRange_Enumerable_string_adds_all_in_order()
    {
        using var msg = new MailMessage();
        MailAddressCollectionExtensions.AddRange(msg.To, (IEnumerable<string>)["a1@example.com", "a2@example.com"]);

        var actual = msg.To.ToList();
        Assert.Equal(2, actual.Count);
        Assert.Equal("a1@example.com", actual[0].Address);
        Assert.Equal("a2@example.com", actual[1].Address);
    }



    [Fact]
    public void AddRange_Enumerable_string_when_addresses_is_empty_does_not_change_source()
    {
        using var msg = new MailMessage();
        MailAddressCollectionExtensions.AddRange(msg.To, Enumerable.Empty<string>());
        Assert.Empty(msg.To);
    }



    // ---------- AddRange(params string[]) ----------

    [Fact]
    public void AddRange_params_string_when_source_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailAddressCollectionExtensions.AddRange(null!, "a@example.com")
        );
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void AddRange_params_string_when_addresses_is_null_throws_ArgumentNullException()
    {
        using var msg = new MailMessage();
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailAddressCollectionExtensions.AddRange(msg.To, (string[])null!)
        );
        Assert.Equal("addresses", ex.ParamName);
    }



    [Fact]
    public void AddRange_params_string_adds_all_in_order()
    {
        using var msg = new MailMessage();
        MailAddressCollectionExtensions.AddRange(msg.To, "a1@example.com", "a2@example.com");

        var actual = msg.To.ToList();
        Assert.Equal(2, actual.Count);
        Assert.Equal("a1@example.com", actual[0].Address);
        Assert.Equal("a2@example.com", actual[1].Address);
    }



    [Fact]
    public void AddRange_params_string_when_passed_empty_list_does_not_change_source()
    {
        using var msg = new MailMessage();
        MailAddressCollectionExtensions.AddRange(msg.To, Array.Empty<string>());
        Assert.Empty(msg.To);
    }



    // ---------- AddRange(IEnumerable<MailAddress>) ----------

    [Fact]
    public void AddRange_Enumerable_MailAddress_when_source_is_null_throws_ArgumentNullException()
    {
        var a = new MailAddress("a@example.com");
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailAddressCollectionExtensions.AddRange(null!, (IEnumerable<MailAddress>)[a])
        );
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void AddRange_Enumerable_MailAddress_when_addresses_is_null_throws_ArgumentNullException()
    {
        using var msg = new MailMessage();
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailAddressCollectionExtensions.AddRange(msg.To, (IEnumerable<MailAddress>)null!)
        );
        Assert.Equal("addresses", ex.ParamName);
    }



    [Fact]
    public void AddRange_Enumerable_MailAddress_adds_all_in_order()
    {
        using var msg = new MailMessage();
        var a1 = new MailAddress("a1@example.com");
        var a2 = new MailAddress("a2@example.com");

        MailAddressCollectionExtensions.AddRange(msg.To, (IEnumerable<MailAddress>)[a1, a2]);

        var actual = msg.To.ToList();
        Assert.Equal(2, actual.Count);
        Assert.Equal("a1@example.com", actual[0].Address);
        Assert.Equal("a2@example.com", actual[1].Address);
    }



    [Fact]
    public void AddRange_Enumerable_MailAddress_when_addresses_is_empty_does_not_change_source()
    {
        using var msg = new MailMessage();
        MailAddressCollectionExtensions.AddRange(msg.To, Enumerable.Empty<MailAddress>());
        Assert.Empty(msg.To);
    }



    // ---------- AddRange(params MailAddress[]) ----------

    [Fact]
    public void AddRange_params_MailAddress_when_source_is_null_throws_ArgumentNullException()
    {
        var a = new MailAddress("a@example.com");
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailAddressCollectionExtensions.AddRange(null!, a)
        );
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void AddRange_params_MailAddress_when_addresses_is_null_throws_ArgumentNullException()
    {
        using var msg = new MailMessage();
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailAddressCollectionExtensions.AddRange(msg.To, (MailAddress[])null!)
        );
        Assert.Equal("addresses", ex.ParamName);
    }



    [Fact]
    public void AddRange_params_MailAddress_adds_all_in_order()
    {
        using var msg = new MailMessage();
        var a1 = new MailAddress("a1@example.com");
        var a2 = new MailAddress("a2@example.com");

        MailAddressCollectionExtensions.AddRange(msg.To, a1, a2);

        var actual = msg.To.ToList();
        Assert.Equal(2, actual.Count);
        Assert.Equal("a1@example.com", actual[0].Address);
        Assert.Equal("a2@example.com", actual[1].Address);
    }



    [Fact]
    public void AddRange_params_MailAddress_when_passed_empty_list_does_not_change_source()
    {
        using var msg = new MailMessage();
        MailAddressCollectionExtensions.AddRange(msg.To, Array.Empty<MailAddress>());
        Assert.Empty(msg.To);
    }



    // ---------- ToFormattedString ----------

    [Fact]
    public void ToFormattedString_when_source_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailAddressCollectionExtensions.ToFormattedString(null!)
        );
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void ToFormattedString_when_empty_collection_returns_empty_string()
    {
        using var msg = new MailMessage();
        Assert.Equal("", msg.To.ToFormattedString());
    }



    [Fact]
    public void ToFormattedString_when_address_without_display_name_returns_address_only()
    {
        using var msg = new MailMessage();
        msg.To.Add(new MailAddress("bob@example.com"));

        Assert.Equal("bob@example.com", msg.To.ToFormattedString());
    }



    [Fact]
    public void ToFormattedString_when_address_with_display_name_returns_quoted_format()
    {
        using var msg = new MailMessage();
        msg.To.Add(new MailAddress("alice@example.com", "Alice Smith"));

        Assert.Equal("\"Alice Smith\" <alice@example.com>", msg.To.ToFormattedString());
    }



    [Fact]
    public void ToFormattedString_when_multiple_addresses_joins_with_default_separator()
    {
        using var msg = new MailMessage();
        msg.To.Add(new MailAddress("alice@example.com", "Alice"));
        msg.To.Add(new MailAddress("bob@example.com"));

        var result = msg.To.ToFormattedString();

        Assert.Equal("\"Alice\" <alice@example.com>; bob@example.com", result);
    }



    [Fact]
    public void ToFormattedString_when_custom_separator_uses_separator()
    {
        using var msg = new MailMessage();
        msg.To.Add(new MailAddress("a@example.com"));
        msg.To.Add(new MailAddress("b@example.com"));

        var result = msg.To.ToFormattedString(", ");

        Assert.Equal("a@example.com, b@example.com", result);
    }



    // ---------- FormatMailAddress ----------

    [Fact]
    public void FormatMailAddress_when_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailAddressCollectionExtensions.FormatMailAddress(null!)
        );
        Assert.Equal("address", ex.ParamName);
    }
}
