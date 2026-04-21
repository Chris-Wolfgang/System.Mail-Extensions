using System.IO;
using System.Net.Mail;
using Wolfgang.Extensions.Mail.Validation;
using Xunit;
using Assert = Xunit.Assert;
// ReSharper disable InvokeAsExtensionMember
#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;

public class MailMessageExtensions_Validate_Tests
{
    // ---------- Null guard ----------

    [Fact]
    public void Validate_when_source_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => MailMessageExtensions.Validate(null!)
        );
        Assert.Equal("source", ex.ParamName);
    }



    // ---------- From ----------

    [Fact]
    public void Validate_when_From_is_null_returns_error()
    {
        using var msg = new MailMessage();
        msg.To.Add("to@example.com");

        var result = msg.Validate();

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "From");
    }



    // ---------- Recipients ----------

    [Fact]
    public void Validate_when_no_recipients_returns_error()
    {
        using var msg = new MailMessage();
        msg.From = new MailAddress("from@example.com");

        var result = msg.Validate();

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "To");
    }



    [Fact]
    public void Validate_when_To_has_address_returns_no_recipient_error()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Test";
        msg.Body = "Body";

        var result = msg.Validate();

        Assert.True(result.IsValid);
    }



    [Fact]
    public void Validate_when_CC_only_has_address_returns_no_recipient_error()
    {
        using var msg = new MailMessage();
        msg.From = new MailAddress("from@example.com");
        msg.CC.Add("cc@example.com");
        msg.Subject = "Test";
        msg.Body = "Body";

        var result = msg.Validate();

        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "To");
    }



    [Fact]
    public void Validate_when_BCC_only_has_address_returns_no_recipient_error()
    {
        using var msg = new MailMessage();
        msg.From = new MailAddress("from@example.com");
        msg.Bcc.Add("bcc@example.com");
        msg.Subject = "Test";
        msg.Body = "Body";

        var result = msg.Validate();

        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "To");
    }



    // ---------- Subject ----------

    [Fact]
    public void Validate_when_Subject_is_null_returns_warning()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Body = "Body";

        var result = msg.Validate();

        Assert.Contains(result.Warnings, w => w.PropertyName == "Subject");
    }



    [Fact]
    public void Validate_when_Subject_is_empty_returns_warning()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "";
        msg.Body = "Body";

        var result = msg.Validate();

        Assert.Contains(result.Warnings, w => w.PropertyName == "Subject");
    }



    // ---------- Body ----------

    [Fact]
    public void Validate_when_Body_is_null_and_no_AlternateViews_returns_warning()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Test";

        var result = msg.Validate();

        Assert.Contains(result.Warnings, w => w.PropertyName == "Body");
    }



    [Fact]
    public void Validate_when_Body_is_null_but_AlternateViews_exist_returns_no_body_warning()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Test";
        var view = AlternateView.CreateAlternateViewFromString
        (
            "<h1>Hello</h1>",
            null,
            "text/html"
        );
        msg.AlternateViews.Add(view);

        var result = msg.Validate();

        Assert.DoesNotContain(result.Warnings, w => w.PropertyName == "Body");
    }



    // ---------- Fully valid ----------

    [Fact]
    public void Validate_when_all_fields_valid_returns_IsValid_true()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Test";
        msg.Body = "Body";

        var result = msg.Validate();

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
        Assert.Empty(result.Warnings);
    }



    // ---------- Multiple errors ----------

    [Fact]
    public void Validate_when_multiple_errors_returns_all_errors()
    {
        using var msg = new MailMessage();
        // No From, no recipients

        var result = msg.Validate();

        Assert.True(result.Errors.Count >= 2);
    }



    // ---------- ValidationOptions ----------

    [Fact]
    public void Validate_with_options_RequireSubject_true_and_empty_subject_returns_error()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Body = "Body";
        var options = new ValidationOptions { RequireSubject = true };

        var result = msg.Validate(options);

        Assert.Contains(result.Errors, e => e.PropertyName == "Subject");
    }



    [Fact]
    public void Validate_with_options_RequireBody_true_and_empty_body_returns_error()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Test";
        var options = new ValidationOptions { RequireBody = true };

        var result = msg.Validate(options);

        Assert.Contains(result.Errors, e => e.PropertyName == "Body");
    }



    [Fact]
    public void Validate_with_options_MaxAttachmentSizeBytes_exceeded_returns_warning()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Test";
        msg.Body = "Body";
        var largeData = new MemoryStream(new byte[1024]);
        msg.Attachments.Add(new Attachment(largeData, "large.bin"));

        var options = new ValidationOptions { MaxAttachmentSizeBytes = 512 };

        var result = msg.Validate(options);

        Assert.Contains(result.Warnings, w => w.PropertyName == "Attachments");
    }



    [Fact]
    public void Validate_with_options_MaxTotalAttachmentSizeBytes_exceeded_returns_warning()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Test";
        msg.Body = "Body";
        msg.Attachments.Add(new Attachment(new MemoryStream(new byte[512]), "a.bin"));
        msg.Attachments.Add(new Attachment(new MemoryStream(new byte[512]), "b.bin"));

        var options = new ValidationOptions { MaxTotalAttachmentSizeBytes = 800 };

        var result = msg.Validate(options);

        Assert.Contains(result.Warnings, w => w.PropertyName == "Attachments" && w.Message.Contains("Total"));
    }



    // ---------- Purity ----------

    [Fact]
    public void Validate_does_not_modify_source_message()
    {
        using var msg = new MailMessage("from@example.com", "to@example.com");
        msg.Subject = "Original";
        msg.Body = "Body";

        msg.Validate();

        Assert.Equal("Original", msg.Subject);
        Assert.Equal("Body", msg.Body);
        Assert.Equal("from@example.com", msg.From!.Address);
    }



    // ---------- ValidationIssue.ToString ----------

    [Fact]
    public void ValidationIssue_ToString_when_PropertyName_set_includes_property()
    {
        var issue = new ValidationIssue(ValidationSeverity.Error, "Test message", "TestProp");

        Assert.Equal("Error: [TestProp] Test message", issue.ToString());
    }



    [Fact]
    public void ValidationIssue_ToString_when_PropertyName_null_excludes_brackets()
    {
        var issue = new ValidationIssue(ValidationSeverity.Warning, "General warning");

        Assert.Equal("Warning: General warning", issue.ToString());
    }
}
