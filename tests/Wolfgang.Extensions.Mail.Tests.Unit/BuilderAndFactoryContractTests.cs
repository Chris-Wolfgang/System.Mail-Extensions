using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Pins argument names, messages and option handling of the builders and the attachment
/// factory that the earlier suites exercised without asserting. Added for the Stryker
/// survivors listed in #280.
/// </summary>
public class BuilderAndFactoryContractTests
{

    // ---------- MailMessageBuilder ----------

    [Fact]
    public void SenderAddress_with_display_name_keeps_it()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .SenderAddress("sender@example.com", "The Sender")
            .To("to@example.com")
            .Build();

        Assert.Equal("The Sender", msg.Sender!.DisplayName);
    }



    [Fact]
    public void Header_when_name_is_null_names_the_parameter()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new MailMessageBuilder().Header(null!, "v"));

        Assert.Equal("name", ex.ParamName);
    }



    [Fact]
    public void Attach_file_when_path_is_null_names_the_parameter()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new MailMessageBuilder().Attach(null!));

        Assert.Equal("filePath", ex.ParamName);
    }



    [Fact]
    public void Attach_stream_when_content_is_null_names_the_parameter()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new MailMessageBuilder().Attach((Stream)null!, "a.bin"));

        Assert.Equal("content", ex.ParamName);
    }



    [Fact]
    public void Attach_bytes_when_content_is_null_names_the_parameter()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new MailMessageBuilder().Attach((byte[])null!, "a.bin"));

        Assert.Equal("content", ex.ParamName);
    }



    [Theory]
    [InlineData(null, "application/octet-stream")]
    [InlineData("application/pdf", "application/pdf")]
    public void Attach_stream_uses_the_given_content_type_or_the_default(string? contentType, string expected)
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .Attach(new MemoryStream(new byte[] { 1 }), "a.bin", contentType)
            .Build();

        Assert.Equal(expected, Assert.Single(msg.Attachments).ContentType.MediaType);
    }



    [Theory]
    [InlineData(null, "application/octet-stream")]
    [InlineData("application/pdf", "application/pdf")]
    public void Attach_bytes_uses_the_given_content_type_or_the_default(string? contentType, string expected)
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .Attach(new byte[] { 1 }, "a.bin", contentType)
            .Build();

        var attachment = Assert.Single(msg.Attachments);
        Assert.Equal(expected, attachment.ContentType.MediaType);
        Assert.False(attachment.ContentStream.CanWrite);
    }



    [Fact]
    public void Build_when_nothing_is_set_reports_both_missing_parts_in_one_message()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => new MailMessageBuilder().Build());

        Assert.Equal
        (
            "From address is required. Call From() before Build(). At least one recipient (To, Cc, or Bcc) is required. Call To(), Cc(), or Bcc() before Build().",
            ex.Message
        );
    }



    [Fact]
    public void Build_when_both_bodies_and_a_body_encoding_are_set_uses_it_for_the_html_view()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .PlainTextBody("text")
            .HtmlBody("<p>html</p>")
            .BodyEncoding(Encoding.Unicode)
            .Build();

        Assert.Equal("utf-16", Assert.Single(msg.AlternateViews).ContentType.CharSet);
    }



    [Fact]
    public void Build_when_no_body_is_set_leaves_an_empty_plain_body()
    {
        using var msg = new MailMessageBuilder()
            .From("from@example.com")
            .To("to@example.com")
            .Build();

        Assert.Equal(string.Empty, msg.Body);
        Assert.False(msg.IsBodyHtml);
    }



    // ---------- AttachmentFactory ----------

    [Theory]
    [InlineData("a.pdf", "application/pdf")]
    [InlineData("a.zip", "application/zip")]
    [InlineData("a.gz", "application/gzip")]
    [InlineData("a.json", "application/json")]
    [InlineData("a.xml", "application/xml")]
    [InlineData("a.doc", "application/msword")]
    [InlineData("a.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [InlineData("a.xls", "application/vnd.ms-excel")]
    [InlineData("a.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [InlineData("a.ppt", "application/vnd.ms-powerpoint")]
    [InlineData("a.pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation")]
    [InlineData("a.png", "image/png")]
    [InlineData("a.jpg", "image/jpeg")]
    [InlineData("a.jpeg", "image/jpeg")]
    [InlineData("a.gif", "image/gif")]
    [InlineData("a.svg", "image/svg+xml")]
    [InlineData("a.bmp", "image/bmp")]
    [InlineData("a.ico", "image/x-icon")]
    [InlineData("a.webp", "image/webp")]
    [InlineData("a.txt", "text/plain")]
    [InlineData("a.csv", "text/csv")]
    [InlineData("a.html", "text/html")]
    [InlineData("a.htm", "text/html")]
    [InlineData("a.css", "text/css")]
    [InlineData("a.js", "text/javascript")]
    [InlineData("a.mp3", "audio/mpeg")]
    [InlineData("a.wav", "audio/wav")]
    [InlineData("a.mp4", "video/mp4")]
    [InlineData("a.avi", "video/x-msvideo")]
    [InlineData("a.eml", "message/rfc822")]
    [InlineData("A.PDF", "application/pdf")]
    [InlineData("a.unknownext", "application/octet-stream")]
    public void InferContentType_maps_every_built_in_extension(string fileName, string expected)
    {
        Assert.Equal(expected, AttachmentFactory.InferContentType(fileName));
    }



    [Fact]
    public void RegisterContentType_when_extension_has_no_dot_explains_the_format()
    {
        var ex = Assert.Throws<ArgumentException>(() => AttachmentFactory.RegisterContentType("heic", "image/heic"));

        Assert.StartsWith("Extension must start with '.' (for example, \".heic\").", ex.Message, StringComparison.Ordinal);
    }



    [Fact]
    public void TryGetRegisteredContentType_when_extension_is_null_names_the_parameter()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => AttachmentFactory.TryGetRegisteredContentType(null!, out _));

        Assert.Equal("extension", ex.ParamName);
    }



    [Fact]
    public void Factory_methods_when_file_name_is_null_throw_even_with_an_explicit_content_type()
    {
        var bytes = Assert.Throws<ArgumentNullException>(() => AttachmentFactory.FromBytes(new byte[] { 1 }, null!, "text/plain"));
        var base64 = Assert.Throws<ArgumentNullException>(() => AttachmentFactory.FromBase64("AQ==", null!, "text/plain"));
        var stream = Assert.Throws<ArgumentNullException>(() => AttachmentFactory.FromStream(new MemoryStream(new byte[] { 1 }), null!, "text/plain"));

        Assert.Equal("fileName", bytes.ParamName);
        Assert.Equal("fileName", base64.ParamName);
        Assert.Equal("fileName", stream.ParamName);
    }



    [Fact]
    public void FromBytes_wraps_the_payload_read_only()
    {
        using var attachment = AttachmentFactory.FromBytes(new byte[] { 1, 2 }, "a.bin");

        Assert.False(attachment.ContentStream.CanWrite);
    }



    // ---------- AttachmentCollection.TotalSize ----------

    [Fact]
    public void TotalSize_when_the_sum_overflows_throws_OverflowException()
    {
        using var msg = new MailMessage();
        msg.Attachments.Add(new Attachment(new HugeLengthStream(), "a.bin"));
        msg.Attachments.Add(new Attachment(new HugeLengthStream(), "b.bin"));

        Assert.Throws<OverflowException>(() => msg.Attachments.TotalSize());
    }



    // ---------- InlineHtmlBuilder ----------

    [Fact]
    public void EmbedImage_when_arguments_are_null_names_the_parameter()
    {
        var builder = new InlineHtmlBuilder();

        Assert.Equal("filePath", Assert.Throws<ArgumentNullException>(() => builder.EmbedImage((string)null!)).ParamName);
        Assert.Equal("fileName", Assert.Throws<ArgumentNullException>(() => builder.EmbedImage(new MemoryStream(new byte[] { 1 }), null!, "image/png")).ParamName);
        Assert.Equal("imageBytes", Assert.Throws<ArgumentNullException>(() => builder.EmbedImage((byte[])null!, "a.png")).ParamName);
        Assert.Equal("fileName", Assert.Throws<ArgumentNullException>(() => builder.EmbedImage(new byte[] { 1 }, null!, "image/png")).ParamName);
    }



    [Fact]
    public void Build_without_Html_explains_what_is_missing()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => new InlineHtmlBuilder().Build());

        Assert.Equal("An HTML template must be set via Html() before calling Build().", ex.Message);
    }



    [Fact]
    public void EmbedImage_stream_uses_the_explicit_content_type_and_wires_the_content_id()
    {
        var builder = new InlineHtmlBuilder()
            .Html("<img src=\"cid:{0}\">")
            .EmbedImage(new MemoryStream(new byte[] { 1, 2, 3 }), "picture.png", "image/custom");

        using var view = builder.Build();
        using var reader = new StreamReader(view.ContentStream);
        var resource = Assert.Single(view.LinkedResources);

        Assert.Equal("image/custom", resource.ContentType.MediaType);
        Assert.Matches("^[0-9a-f]{32}$", resource.ContentId);
        Assert.Equal($"<img src=\"cid:{resource.ContentId}\">", reader.ReadToEnd());
    }



    [Fact]
    public void EmbedImage_bytes_and_file_wire_the_content_id_and_keep_the_payload_read_only()
    {
        var file = Path.Combine(Path.GetTempPath(), $"inline_{Guid.NewGuid():N}.png");
        File.WriteAllBytes(file, new byte[] { 1, 2 });
        try
        {
            var builder = new InlineHtmlBuilder()
                .Html("{0}|{1}")
                .EmbedImage(new byte[] { 3 }, "b.gif")
                .EmbedImage(file);

            using var view = builder.Build();
            using var reader = new StreamReader(view.ContentStream);

            Assert.Equal($"{view.LinkedResources[0].ContentId}|{view.LinkedResources[1].ContentId}", reader.ReadToEnd());
            Assert.False(view.LinkedResources[0].ContentStream.CanWrite);
            Assert.False(view.LinkedResources[1].ContentStream.CanWrite);
        }
        finally
        {
            File.Delete(file);
        }
    }



    /// <summary>A seekable stream that reports a length near <see cref="long.MaxValue"/>.</summary>
    private sealed class HugeLengthStream : MemoryStream
    {
        public override long Length => long.MaxValue - 1;
    }
}
