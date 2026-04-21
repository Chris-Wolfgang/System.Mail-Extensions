using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;
// ReSharper disable InvokeAsExtensionMember
#pragma warning disable CA1707
#pragma warning disable MA0074 // xUnit Assert.Contains triggers this for string overloads

namespace Wolfgang.Extensions.Mail.Tests.Unit;

public class MailMessageExtensions_ToMimeString_Tests
{
    // ---------- Null guards ----------

    [Fact]
    public void ToMimeString_when_source_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailMessageExtensions.ToMimeString(null!)
        );
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void ToMimeString_when_From_is_null_throws_InvalidOperationException()
    {
        using var msg = new MailMessage();
        msg.To.Add("to@example.com");

        Assert.Throws<InvalidOperationException>
        (
            () => msg.ToMimeString()
        );
    }



    // ---------- Basic output ----------

    [Fact]
    public void ToMimeString_when_simple_text_message_returns_non_empty_string()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Test Subject";
        msg.Body = "Hello, World!";

        var mime = msg.ToMimeString();

        Assert.False(string.IsNullOrWhiteSpace(mime));
    }



    [Fact]
    public void ToMimeString_output_contains_From_header()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Test";
        msg.Body = "Body";

        var mime = msg.ToMimeString();

        Assert.Contains("from@example.com", mime);
    }



    [Fact]
    public void ToMimeString_output_contains_To_header()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Test";
        msg.Body = "Body";

        var mime = msg.ToMimeString();

        Assert.Contains("to@example.com", mime);
    }



    [Fact]
    public void ToMimeString_output_contains_Subject_header()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "My Test Subject";
        msg.Body = "Body";

        var mime = msg.ToMimeString();

        Assert.Contains("My Test Subject", mime);
    }



    [Fact]
    public void ToMimeString_output_contains_MIME_Version_header()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Test";
        msg.Body = "Body";

        var mime = msg.ToMimeString();

        Assert.Contains("MIME-Version: 1.0", mime);
    }



    // ---------- HTML ----------

    [Fact]
    public void ToMimeString_when_html_message_contains_Content_Type_text_html()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "HTML Test";
        msg.Body = "<h1>Hello</h1>";
        msg.IsBodyHtml = true;

        var mime = msg.ToMimeString();

        Assert.Contains("text/html", mime);
    }



    // ---------- Attachments ----------

    [Fact]
    public void ToMimeString_when_message_has_attachments_returns_multipart_mime()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "With Attachment";
        msg.Body = "See attached";
        msg.Attachments.Add(new Attachment(new MemoryStream(new byte[] { 1, 2, 3 }), "test.bin"));

        var mime = msg.ToMimeString();

        Assert.Contains("multipart", mime.ToLowerInvariant());
    }



    // ---------- CC / BCC ----------

    [Fact]
    public void ToMimeString_when_message_has_CC_includes_header()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.CC.Add("cc@example.com");
        msg.Subject = "Test";
        msg.Body = "Body";

        var mime = msg.ToMimeString();

        Assert.Contains("cc@example.com", mime);
    }



    // ---------- SaveToEmlAsync ----------

    [Fact]
    public async Task SaveToEmlAsync_when_source_is_null_throws_ArgumentNullException()
    {
        var ex = await Assert.ThrowsAsync<ArgumentNullException>
        (
            () => MailMessageExtensions.SaveToEmlAsync(null!, "test.eml")
        );
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public async Task SaveToEmlAsync_when_filePath_is_null_throws_ArgumentNullException()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Test";
        msg.Body = "Body";

        var ex = await Assert.ThrowsAsync<ArgumentNullException>
        (
            () => msg.SaveToEmlAsync(null!)
        );
        Assert.Equal("filePath", ex.ParamName);
    }



    [Fact]
    public async Task SaveToEmlAsync_when_valid_message_writes_file_with_mime_content()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "EML Test";
        msg.Body = "Body content";

        var filePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}.eml");

        try
        {
            await msg.SaveToEmlAsync(filePath);

            Assert.True(File.Exists(filePath));
            string content;
            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                content = await reader.ReadToEndAsync();
            }
            Assert.Contains("from@example.com", content);
            Assert.Contains("EML Test", content);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
