using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Wolfgang.Extensions.Mail.Validation;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Pins the messages, limits and clone fidelity of <see cref="MailMessageExtensions"/> that the
/// earlier suites exercised without asserting. Added for the Stryker survivors listed in #280.
/// </summary>
public class MailMessageExtensions_Contract_Tests
{

    // ---------- ToMimeString ----------

    [Fact]
    public void ToMimeString_when_From_is_missing_explains_it()
    {
        using var msg = new MailMessage();
        msg.To.Add("to@example.com");

        var ex = Assert.Throws<InvalidOperationException>(() => msg.ToMimeString());

        Assert.Equal("The From property must be set before serializing to MIME format.", ex.Message);
    }



    [Fact]
    public void ToMimeString_encodes_a_non_ascii_subject_as_an_encoded_word()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com")
        {
            Subject = "Café",
            Body = "Body"
        };

        var mime = msg.ToMimeString();

        Assert.DoesNotContain("Café", mime, StringComparison.Ordinal);
        Assert.Contains("Subject: =?utf-8?", mime, StringComparison.OrdinalIgnoreCase);
    }



    // ---------- Validate ----------

    [Fact]
    public void Validate_when_message_is_empty_reports_each_problem_with_its_text()
    {
        using var msg = new MailMessage();

        var result = msg.Validate();

        Assert.Equal
        (
            new[] { "From address is required.", "At least one recipient (To, CC, or BCC) is required." },
            new[] { result.Errors[0].Message, result.Errors[1].Message }
        );
        Assert.Contains(result.Warnings, w => string.Equals(w.Message, "Subject is empty or missing.", StringComparison.Ordinal) && string.Equals(w.PropertyName, "Subject", StringComparison.Ordinal));
        Assert.Contains(result.Warnings, w => string.Equals(w.Message, "Body is empty and no alternate views are defined.", StringComparison.Ordinal) && string.Equals(w.PropertyName, "Body", StringComparison.Ordinal));
    }



    [Fact]
    public void Validate_when_attachment_size_equals_the_limit_reports_nothing()
    {
        using var msg = MessageWithAttachment(100);

        var result = msg.Validate(new ValidationOptions { MaxAttachmentSizeBytes = 100, MaxTotalAttachmentSizeBytes = 100 });

        Assert.Empty(result.Warnings);
    }



    [Fact]
    public void Validate_when_attachment_exceeds_the_limit_reports_name_size_and_limit()
    {
        using var msg = MessageWithAttachment(1500);

        var result = msg.Validate(new ValidationOptions { MaxAttachmentSizeBytes = 1000 });

        var warning = Assert.Single(result.Warnings);
        Assert.Equal
        (
            $"Attachment 'data.bin' ({1500:N0} bytes) exceeds the maximum allowed size of {1000:N0} bytes.",
            warning.Message
        );
        Assert.Equal("Attachments", warning.PropertyName);
    }



    // ---------- Clone fidelity ----------

    [Fact]
    public void Clone_keeps_recipient_display_names()
    {
        using var msg = new MailMessage(new MailAddress("from@example.com", "From Name"), new MailAddress("to@example.com", "To Name"));
        msg.CC.Add(new MailAddress("cc@example.com"));

        using var clone = msg.Clone();

        Assert.Equal("From Name", clone.From!.DisplayName);
        Assert.Equal("To Name", clone.To[0].DisplayName);
        Assert.Equal(string.Empty, clone.CC[0].DisplayName);
    }



    [Fact]
    public void Clone_keeps_attachment_metadata()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        var attachment = new Attachment(new MemoryStream(new byte[] { 1, 2, 3 }), new ContentType("text/plain; charset=utf-8"))
        {
            Name = "notes.txt",
            TransferEncoding = TransferEncoding.QuotedPrintable,
            ContentId = "cid-1"
        };
        attachment.ContentDisposition!.Inline = true;
        attachment.ContentDisposition.FileName = "notes.txt";
        attachment.ContentDisposition.Size = 3;
        msg.Attachments.Add(attachment);

        using var clone = msg.Clone();
        var copied = Assert.Single(clone.Attachments);

        Assert.Equal("notes.txt", copied.Name);
        Assert.Equal(TransferEncoding.QuotedPrintable, copied.TransferEncoding);
        Assert.Equal("cid-1", copied.ContentId);
        Assert.Equal("utf-8", copied.ContentType.CharSet);
        Assert.True(copied.ContentDisposition!.Inline);
        Assert.Equal("notes.txt", copied.ContentDisposition.FileName);
        Assert.Equal(3, copied.ContentDisposition.Size);
    }



    [Fact]
    public void Clone_keeps_alternate_view_and_linked_resource_metadata()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        var view = AlternateView.CreateAlternateViewFromString("<img src=\"cid:img1\">", Encoding.UTF8, MediaTypeNames.Text.Html);
        view.TransferEncoding = TransferEncoding.Base64;
        view.BaseUri = new Uri("https://example.com/");
        view.ContentId = "view-1";
        var resource = new LinkedResource(new MemoryStream(new byte[] { 9 }), "image/png")
        {
            ContentId = "img1",
            TransferEncoding = TransferEncoding.Base64,
            ContentLink = new Uri("https://example.com/img.png")
        };
        view.LinkedResources.Add(resource);
        msg.AlternateViews.Add(view);

        using var clone = msg.Clone();
        var copiedView = Assert.Single(clone.AlternateViews);
        var copiedResource = Assert.Single(copiedView.LinkedResources);

        Assert.Equal(TransferEncoding.Base64, copiedView.TransferEncoding);
        Assert.Equal(new Uri("https://example.com/"), copiedView.BaseUri);
        Assert.Equal("view-1", copiedView.ContentId);
        Assert.Equal("utf-8", copiedView.ContentType.CharSet);
        Assert.Equal("img1", copiedResource.ContentId);
        Assert.Equal(TransferEncoding.Base64, copiedResource.TransferEncoding);
        Assert.Equal(new Uri("https://example.com/img.png"), copiedResource.ContentLink);
    }



    [Fact]
    public void Clone_copies_a_generated_attachment_content_id()
    {
        // Attachment generates a Content-ID on first read; the clone must carry the same one,
        // or an HTML body that references it through cid: would break.
        using var msg = MessageWithAttachment(3);
        var original = msg.Attachments[0].ContentId;

        using var clone = msg.Clone();

        Assert.Equal(original, Assert.Single(clone.Attachments).ContentId);
    }



    private static MailMessage MessageWithAttachment(int size)
    {
        var msg = new MailMessage("from@example.com", "to@example.com")
        {
            Subject = "S",
            Body = "Body"
        };
        msg.Attachments.Add(new Attachment(new MemoryStream(new byte[size]), "data.bin"));
        return msg;
    }
}
