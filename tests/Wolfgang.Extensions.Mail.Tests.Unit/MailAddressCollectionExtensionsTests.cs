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
}
