using System.Net.Mail;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;
// ReSharper disable InvokeAsExtensionMember
#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;

public class MailMessageExtensions_Clone_Tests
{
    // ---------- Null guard ----------

    [Fact]
    public void Clone_when_source_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailMessageExtensions.Clone(null!)
        );
        Assert.Equal("source", ex.ParamName);
    }



    // ---------- Basic copy ----------

    [Fact]
    public void Clone_when_minimal_message_returns_independent_copy()
    {
        using var original = new MailMessage("from@example.com", "to@example.com");
        original.Subject = "Test";

        using var clone = original.Clone();

        Assert.NotSame(original, clone);
        Assert.Equal("Test", clone.Subject);
    }



    // ---------- From / Sender ----------

    [Fact]
    public void Clone_when_From_is_set_copies_address_and_display_name()
    {
        using var original = new MailMessage();
        original.From = new MailAddress("from@example.com", "Sender Name");
        original.To.Add("to@example.com");

        using var clone = original.Clone();

        Assert.NotSame(original.From, clone.From);
        Assert.Equal("from@example.com", clone.From!.Address);
        Assert.Equal("Sender Name", clone.From.DisplayName);
    }



    [Fact]
    public void Clone_when_Sender_is_set_copies_Sender()
    {
        using var original = new MailMessage("from@example.com", "to@example.com");
        original.Sender = new MailAddress("sender@example.com", "Actual Sender");

        using var clone = original.Clone();

        Assert.NotNull(clone.Sender);
#pragma warning disable S8969 // `!` is redundant on TFMs where xunit's Assert.NotNull has [NotNull]; required on net462/netstandard2.0 where it doesn't.
        Assert.Equal("sender@example.com", clone.Sender!.Address);
#pragma warning restore S8969
        Assert.Equal("Actual Sender", clone.Sender.DisplayName);
    }



    // ---------- Address collections ----------

    [Fact]
    public void Clone_when_To_CC_BCC_have_addresses_copies_all()
    {
        using var original = new MailMessage("from@example.com", "to@example.com");
        original.CC.Add("cc@example.com");
        original.Bcc.Add("bcc@example.com");

        using var clone = original.Clone();

        var toAddress = Assert.Single(clone.To);
        Assert.Equal("to@example.com", toAddress.Address);
        var ccAddress = Assert.Single(clone.CC);
        Assert.Equal("cc@example.com", ccAddress.Address);
        var bccAddress = Assert.Single(clone.Bcc);
        Assert.Equal("bcc@example.com", bccAddress.Address);
    }



    [Fact]
    public void Clone_when_ReplyToList_has_addresses_copies_all()
    {
        using var original = new MailMessage("from@example.com", "to@example.com");
        original.ReplyToList.Add("reply@example.com");

        using var clone = original.Clone();

        var replyAddress = Assert.Single(clone.ReplyToList);
        Assert.Equal("reply@example.com", replyAddress.Address);
    }



    // ---------- Scalar properties ----------

    [Fact]
    public void Clone_when_Subject_Body_IsBodyHtml_set_copies_values()
    {
        using var original = new MailMessage("from@example.com", "to@example.com");
        original.Subject = "Subject";
        original.Body = "<h1>HTML</h1>";
        original.IsBodyHtml = true;

        using var clone = original.Clone();

        Assert.Equal("Subject", clone.Subject);
        Assert.Equal("<h1>HTML</h1>", clone.Body);
        Assert.True(clone.IsBodyHtml);
    }



    [Fact]
    public void Clone_when_Priority_and_DeliveryNotificationOptions_set_copies_values()
    {
        using var original = new MailMessage("from@example.com", "to@example.com");
        original.Priority = MailPriority.High;
        original.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

        using var clone = original.Clone();

        Assert.Equal(MailPriority.High, clone.Priority);
        Assert.Equal(DeliveryNotificationOptions.OnSuccess, clone.DeliveryNotificationOptions);
    }



    [Fact]
    public void Clone_when_BodyEncoding_SubjectEncoding_set_copies_values()
    {
        using var original = new MailMessage("from@example.com", "to@example.com");
        original.BodyEncoding = Encoding.UTF32;
        original.SubjectEncoding = Encoding.ASCII;

        using var clone = original.Clone();

        Assert.Equal(Encoding.UTF32, clone.BodyEncoding);
        Assert.Equal(Encoding.ASCII, clone.SubjectEncoding);
    }



    // ---------- Headers ----------

    [Fact]
    public void Clone_when_Headers_have_custom_values_copies_all_headers()
    {
        using var original = new MailMessage("from@example.com", "to@example.com");
        original.Headers.Add("X-Custom-1", "Value1");
        original.Headers.Add("X-Custom-2", "Value2");

        using var clone = original.Clone();

        Assert.Equal("Value1", clone.Headers["X-Custom-1"]);
        Assert.Equal("Value2", clone.Headers["X-Custom-2"]);
    }



    // ---------- Attachments ----------

    [Fact]
    public void Clone_when_Attachments_exist_copies_stream_content_independently()
    {
        var content = new byte[] { 1, 2, 3, 4, 5 };
        using var original = new MailMessage("from@example.com", "to@example.com");
        original.Attachments.Add(new Attachment(new MemoryStream(content), "test.bin"));

        using var clone = original.Clone();

        var clonedAttachment = Assert.Single(clone.Attachments);
        Assert.Equal("test.bin", clonedAttachment.Name);

        // Verify stream content is identical
        var clonedStream = clonedAttachment.ContentStream;
        using var ms = new MemoryStream();
        clonedStream.CopyTo(ms);
        var clonedBytes = ms.ToArray();
        Assert.Equal(content, clonedBytes);
    }



    [Fact]
    public void Clone_when_Attachment_stream_modified_original_unaffected()
    {
        var content = new byte[] { 1, 2, 3 };
        using var original = new MailMessage("from@example.com", "to@example.com");
        original.Attachments.Add(new Attachment(new MemoryStream(content), "test.bin"));

        using var clone = original.Clone();

        // Modify clone's stream
        var cloneStream = (MemoryStream)clone.Attachments[0].ContentStream;
        cloneStream.SetLength(0);

        // Original is unaffected
        Assert.Equal(3, original.Attachments[0].ContentStream.Length);
    }



    // ---------- AlternateViews ----------

    [Fact]
    public void Clone_when_AlternateViews_with_LinkedResources_exist_copies_all()
    {
        using var original = new MailMessage("from@example.com", "to@example.com");
        var view = AlternateView.CreateAlternateViewFromString
        (
            content: "<h1>Hello</h1>",
            contentEncoding: null,
            mediaType: "text/html"
        );
        var resource = new LinkedResource
        (
            new MemoryStream(new byte[] { 10, 20, 30 }),
            "image/png"
        );
        resource.ContentId = "img1";
        view.LinkedResources.Add(resource);
        original.AlternateViews.Add(view);

        using var clone = original.Clone();

        var clonedView = Assert.Single(clone.AlternateViews);
        var clonedResource = Assert.Single(clonedView.LinkedResources);
        Assert.Equal("img1", clonedResource.ContentId);
    }



    // ---------- Independence ----------

    [Fact]
    public void Clone_when_returned_message_disposed_original_still_usable()
    {
        using var original = new MailMessage("from@example.com", "to@example.com");
        original.Subject = "Still here";

        var clone = original.Clone();
        clone.Dispose();

        Assert.Equal("Still here", original.Subject);
    }



    [Fact]
    public void Clone_when_original_disposed_clone_still_usable()
    {
        MailMessage clone;
        using (var original = new MailMessage("from@example.com", "to@example.com"))
        {
            original.Subject = "Cloned";
            clone = original.Clone();
        }

        using (clone)
        {
            Assert.Equal("Cloned", clone.Subject);
        }
    }



    [Fact]
    public void Clone_when_attachment_stream_is_non_seekable_copies_content()
    {
        var payload = Encoding.ASCII.GetBytes("non-seekable attachment payload");

        using var original = new MailMessage("from@example.com", "to@example.com");
        original.Attachments.Add(new Attachment(new NonSeekableStream(payload), "data.bin"));

        using var clone = original.Clone();

        var attachment = Assert.Single(clone.Attachments);
        using var buffer = new MemoryStream();
        attachment.ContentStream.CopyTo(buffer);
        Assert.Equal(payload, buffer.ToArray());
    }



    private sealed class NonSeekableStream : Stream
    {
        private readonly MemoryStream _inner;

        public NonSeekableStream(byte[] data) => _inner = new MemoryStream(data);

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);

        public override void Flush() => _inner.Flush();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inner.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
