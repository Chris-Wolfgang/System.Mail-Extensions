using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;
#pragma warning disable CA1707
#pragma warning disable MA0074 // xUnit Assert.Contains triggers this for string overloads
#pragma warning disable S3878 // Array creation is intentional to disambiguate To(IEnumerable) overload

namespace Wolfgang.Extensions.Mail.Tests.Unit;

public class MailMessageBuilderTests
{
    // ---------- Build validation ----------

    [Fact]
    public void Build_when_From_not_set_throws_InvalidOperationException()
    {
        var builder = new MailMessageBuilder();

        Assert.Throws<InvalidOperationException>
        (
            () => builder.Build()
        );
    }



    // ---------- From ----------

    [Fact]
    public void From_when_address_is_null_throws_ArgumentNullException()
    {
        var builder = new MailMessageBuilder();

        Assert.Throws<ArgumentNullException>
        (
            () => builder.From(null!)
        );
    }



    [Fact]
    public void Build_when_From_set_creates_message_with_From()
    {
        using var msg = new MailMessageBuilder()
            .From("sender@example.com")
            .To("to@example.com")
            .Build();

        Assert.Equal("sender@example.com", msg.From!.Address);
    }



    [Fact]
    public void Build_when_From_has_display_name_preserves_it()
    {
        using var msg = new MailMessageBuilder()
            .From("sender@example.com", "Sender Name")
            .To("to@example.com")
            .Build();

        Assert.Equal("Sender Name", msg.From!.DisplayName);
    }



    // ---------- Recipients ----------

    [Fact]
    public void Build_when_To_set_creates_message_with_recipients()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To(new[] { "a@example.com", "b@example.com" })
            .Build();

        Assert.Equal(2, msg.To.Count);
    }



    [Fact]
    public void Build_when_Cc_set_creates_message_with_CC()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .Cc("cc@example.com")
            .Build();

        Assert.Single(msg.CC);
        Assert.Equal("cc@example.com", msg.CC[0].Address);
    }



    [Fact]
    public void Build_when_Bcc_set_creates_message_with_BCC()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .Bcc("bcc@example.com")
            .Build();

        Assert.Single(msg.Bcc);
    }



    [Fact]
    public void Build_when_ReplyTo_set_creates_message_with_ReplyToList()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .ReplyTo("reply@example.com")
            .Build();

        Assert.Single(msg.ReplyToList);
    }



    // ---------- Subject / Body ----------

    [Fact]
    public void Build_when_Subject_set_creates_message_with_subject()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .Subject("Test Subject")
            .Build();

        Assert.Equal("Test Subject", msg.Subject);
    }



    [Fact]
    public void Build_when_PlainTextBody_only_sets_body_as_plain_text()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .PlainTextBody("Hello")
            .Build();

        Assert.Equal("Hello", msg.Body);
        Assert.False(msg.IsBodyHtml);
    }



    [Fact]
    public void Build_when_HtmlBody_only_sets_body_as_html()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .HtmlBody("<h1>Hello</h1>")
            .Build();

        Assert.Equal("<h1>Hello</h1>", msg.Body);
        Assert.True(msg.IsBodyHtml);
    }



    [Fact]
    public void Build_when_both_bodies_set_uses_plain_text_body_and_html_alternate_view()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .PlainTextBody("Plain text")
            .HtmlBody("<h1>HTML</h1>")
            .Build();

        Assert.Equal("Plain text", msg.Body);
        Assert.False(msg.IsBodyHtml);
        Assert.Single(msg.AlternateViews);
        Assert.Contains("text/html", msg.AlternateViews[0].ContentType.MediaType);
    }



    // ---------- Priority / Headers ----------

    [Fact]
    public void Build_when_Priority_set_applies_priority()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .Priority(MailPriority.High)
            .Build();

        Assert.Equal(MailPriority.High, msg.Priority);
    }



    [Fact]
    public void Build_when_Header_set_applies_custom_header()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .Header("X-Custom", "value")
            .Build();

        Assert.Equal("value", msg.Headers["X-Custom"]);
    }



    // ---------- Attachments ----------

    [Fact]
    public void Build_when_Attach_stream_adds_attachment()
    {
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .Attach(stream, "data.bin")
            .Build();

        Assert.Single(msg.Attachments);
        Assert.Equal("data.bin", msg.Attachments[0].Name);
    }



    [Fact]
    public void Build_when_Attach_bytes_adds_attachment()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .Attach(new byte[] { 1, 2, 3 }, "report.pdf", "application/pdf")
            .Build();

        Assert.Single(msg.Attachments);
        Assert.Equal("report.pdf", msg.Attachments[0].Name);
    }



    // ---------- Sender ----------

    [Fact]
    public void Build_when_SenderAddress_set_applies_sender()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .SenderAddress("actual@example.com", "Actual Sender")
            .To("to@example.com")
            .Build();

        Assert.NotNull(msg.Sender);
        Assert.Equal("actual@example.com", msg.Sender!.Address);
    }



    // ---------- Encoding ----------

    [Fact]
    public void Build_when_encodings_set_applies_encodings()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .BodyEncoding(Encoding.UTF32)
            .SubjectEncoding(Encoding.ASCII)
            .Build();

        Assert.Equal(Encoding.UTF32, msg.BodyEncoding);
        Assert.Equal(Encoding.ASCII, msg.SubjectEncoding);
    }



    // ---------- Chaining ----------

    [Fact]
    public void Build_full_fluent_chain_produces_complete_message()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com", "From Name")
            .To("to@example.com")
            .Cc("cc@example.com")
            .Bcc("bcc@example.com")
            .ReplyTo("reply@example.com")
            .Subject("Full Test")
            .PlainTextBody("Plain")
            .HtmlBody("<b>HTML</b>")
            .Priority(MailPriority.High)
            .Header("X-Test", "yes")
            .Attach(new byte[] { 1 }, "file.bin")
            .Build();

        Assert.Equal("from@example.com", msg.From!.Address);
        Assert.Single(msg.To);
        Assert.Single(msg.CC);
        Assert.Single(msg.Bcc);
        Assert.Single(msg.ReplyToList);
        Assert.Equal("Full Test", msg.Subject);
        Assert.Equal("Plain", msg.Body);
        Assert.Single(msg.AlternateViews);
        Assert.Equal(MailPriority.High, msg.Priority);
        Assert.Equal("yes", msg.Headers["X-Test"]);
        Assert.Single(msg.Attachments);
    }
}
