using System.Net.Mail;
using Xunit;
using Assert = Xunit.Assert;
// ReSharper disable InvokeAsExtensionMember
#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;

public class AttachmentCollectionExtensionsTests
{
    // ---------- Add(params Attachment[]) ----------

    [Fact]
    public void AddRange_params_Attachment_when_source_is_null_throws_ArgumentNullException()
    {
        using var a = CreateAttachment("a.txt");
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => AttachmentCollectionExtensions.AddRange(null!, a)
        );
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void AddRange_params_Attachment_when_Attachment_is_null_throws_ArgumentNullException()
    {
        using var msg = new MailMessage();
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => AttachmentCollectionExtensions.AddRange(msg.Attachments, (Attachment[])null!)
        );
        Assert.Equal("attachments", ex.ParamName);
    }



    [Fact]
    public void AddRange_params_Attachment_Adds_All_In_Order()
    {
        using var msg = new MailMessage();
        using var a1 = CreateAttachment("a1.txt");
        using var a2 = CreateAttachment("a2.txt");

        msg.Attachments.AddRange(a1, a2);

        var actual = msg.Attachments.ToList();
        Assert.Equal(2, actual.Count);
        Assert.Same(a1, actual[0]);
        Assert.Same(a2, actual[1]);
    }



    [Fact]
    public void AddRange_params_Attachment_when_passed_an_empty_list_does_not_change_source()
    {
        using var msg = new MailMessage();
        msg.Attachments.AddRange(Array.Empty<Attachment>());
        Assert.Equal(0, msg.Attachments.Count);
    }




    // ---------- Add(params string[] fileNames) ----------

    [Fact]
    public void AddRange_params_string_when_source_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => AttachmentCollectionExtensions.AddRange(null!, "test file.txt")
        );
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void AddRange_params_string_when_string_is_null_throws_ArgumentNullException()
    {
        using var msg = new MailMessage();
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => AttachmentCollectionExtensions.AddRange(msg.Attachments, (string[])null!)
        );
        Assert.Equal("fileNames", ex.ParamName);
    }



    [Fact]
    public void AddRange_params_string_adds_all_in_order()
    {
        var f1 = CreateTempFile("f1", ".txt");
        var f2 = CreateTempFile("f2", ".log");

        try
        {
            using var msg = new MailMessage();
            msg.Attachments.AddRange(f1, f2);

            var actual = msg.Attachments.ToList();
            Assert.Equal(2, actual.Count);
            Assert.Equal(Path.GetFileName(f1), actual[0].Name);
            Assert.Equal(Path.GetFileName(f2), actual[1].Name);
        }
        finally
        {
            File.Delete(f1);
            File.Delete(f2);
        }
    }



    [Fact]
    public void AddRange_params_string_when_passed_empty_list_does_not_change_source()
    {
        using var msg = new MailMessage();
        msg.Attachments.AddRange(Array.Empty<string>());
        Assert.Equal(0, msg.Attachments.Count);
    }




    // ---------- AddRange(IEnumerable<Attachment>) ----------

    [Fact]
    public void AddRange_Enumerable_Attachments_when_source_is_null_throws_ArgumentNullException()
    {
        using var a = CreateAttachment("a.txt");
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => AttachmentCollectionExtensions.AddRange(null!, a)
        );
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void AddRange_Enumerable_Attachments_when_attachments_is_null_throws_ArgumentNullException()
    {
        using var msg = new MailMessage();
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => msg.Attachments.AddRange((IEnumerable<Attachment>)null!)
        );
        Assert.Equal("attachments", ex.ParamName);
    }



    [Fact]
    public void AddRange_Enumerable_Attachments_adds_all_in_order()
    {
        using var msg = new MailMessage();
        using var a1 = CreateAttachment("a1.txt");
        using var a2 = CreateAttachment("a2.txt");

        AttachmentCollectionExtensions.AddRange(msg.Attachments, (IEnumerable<Attachment>)[a1, a2]);

        var actual = msg.Attachments.ToList();
        Assert.Equal(2, actual.Count);
        Assert.Same(a1, actual[0]);
        Assert.Same(a2, actual[1]);
    }



    [Fact]
    public void AddRange_Enumerable_Attachments_when_attachments_is_empty_does_not_change_source()
    {
        using var msg = new MailMessage();
        msg.Attachments.AddRange(Enumerable.Empty<Attachment>());
        Assert.Equal(0, msg.Attachments.Count);
    }



    // ---------- AddRange(IEnumerable<string> fileNames) ----------

    [Fact]
    public void AddRange_Enumerable_string_when_source_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => AttachmentCollectionExtensions.AddRange(null!, "filename.txt")
        );
        Assert.Equal("source", ex.ParamName);
    }



    [Fact]
    public void AddRange_Enumerable_string_when_fileNames_is_null_throws_ArgumentNullException()
    {
        using var msg = new MailMessage();
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => msg.Attachments.AddRange((IEnumerable<string>)null!)
        );
        Assert.Equal("fileNames", ex.ParamName);
    }



    [Fact]
    public void AddRange_Enumerable_string_adds_all_in_order()
    {
        var f1 = CreateTempFile("f1", ".txt");
        var f2 = CreateTempFile("f2", ".log");

        try
        {
            using var msg = new MailMessage();
            AttachmentCollectionExtensions.AddRange(msg.Attachments, (IEnumerable<string>)[f1, f2]);

            var actual = msg.Attachments.ToList();
            Assert.Equal(2, actual.Count);
            Assert.Equal(Path.GetFileName(f1), actual[0].Name);
            Assert.Equal(Path.GetFileName(f2), actual[1].Name);
        }
        finally
        {
            File.Delete(f1);
            File.Delete(f2);
        }
    }



    [Fact]
    public void AddRange_Enumerable_string_when_fileNames_is_empty_does_not_change_source()
    {
        using var msg = new MailMessage();
        msg.Attachments.AddRange(Enumerable.Empty<string>());
        Assert.Equal(0, msg.Attachments.Count);
    }




    // ---------- Helpers ----------

    private static Attachment CreateAttachment(string name)
    {
        var ms = new MemoryStream([1, 2, 3], writable: false);
        return new Attachment(ms, name);
    }



    private static string CreateTempFile(string? prefix = null, string? extension = null)
    {
        var file = Path.Combine(
            Path.GetTempPath(),
            $"{prefix ?? "file"}_{Guid.NewGuid():N}{extension ?? ".tmp"}"
        );
        File.WriteAllText(file, "test-content");
        return file;
    }
}