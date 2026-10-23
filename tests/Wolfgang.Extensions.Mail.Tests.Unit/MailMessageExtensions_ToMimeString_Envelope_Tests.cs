using System;
using System.Net.Mail;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// <c>ToMimeString</c> writes the message, not the SMTP pickup-directory envelope: no
/// <c>X-Sender</c> / <c>X-Receiver</c> lines, so Bcc recipients never appear in the text.
/// </summary>
public class MailMessageExtensions_ToMimeString_Envelope_Tests
{

    [Fact]
    public void ToMimeString_does_not_disclose_Bcc_recipients()
    {
        using var msg = Message();
        msg.Bcc.Add("hidden@example.com");

        var mime = msg.ToMimeString();

        Assert.DoesNotContain("hidden@example.com", mime, StringComparison.Ordinal);
    }



    [Fact]
    public void ToMimeString_does_not_write_the_pickup_directory_envelope()
    {
        using var msg = Message();

        var mime = msg.ToMimeString();

        Assert.DoesNotContain("X-Sender:", mime, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("X-Receiver:", mime, StringComparison.OrdinalIgnoreCase);
    }



    [Fact]
    public void ToMimeString_still_writes_the_visible_recipients()
    {
        using var msg = Message();
        msg.CC.Add("cc@example.com");

        var mime = msg.ToMimeString();

        Assert.Contains("To: to@example.com\r\n", mime, StringComparison.Ordinal);
        Assert.Contains("Cc: cc@example.com\r\n", mime, StringComparison.Ordinal);
    }



    [Fact]
    public void ToMimeString_then_Parse_does_not_add_envelope_headers()
    {
        using var msg = Message();
        msg.Bcc.Add("hidden@example.com");

        using var parsed = EmlParser.Parse(msg.ToMimeString());

        Assert.Null(parsed.Headers["X-Sender"]);
        Assert.Null(parsed.Headers["X-Receiver"]);
        Assert.Empty(parsed.Bcc);
    }



    private static MailMessage Message()
    {
        return new MailMessage("from@example.com", "to@example.com")
        {
            Subject = "S",
            Body = "Body"
        };
    }
}
