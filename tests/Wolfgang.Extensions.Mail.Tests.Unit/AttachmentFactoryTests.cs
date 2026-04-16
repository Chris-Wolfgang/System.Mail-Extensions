using System;
using System.IO;
using System.Net.Mail;
using Xunit;
using Assert = Xunit.Assert;
#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;

public class AttachmentFactoryTests
{
    // ---------- FromBytes ----------

    [Fact]
    public void FromBytes_when_content_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => AttachmentFactory.FromBytes(null!, "test.bin")
        );
        Assert.Equal("content", ex.ParamName);
    }



    [Fact]
    public void FromBytes_when_fileName_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => AttachmentFactory.FromBytes(new byte[] { 1 }, null!)
        );
        Assert.Equal("fileName", ex.ParamName);
    }



    [Fact]
    public void FromBytes_when_valid_creates_attachment_with_inferred_content_type()
    {
        using var attachment = AttachmentFactory.FromBytes
        (
            new byte[] { 1, 2, 3 },
            "report.pdf"
        );

        Assert.Equal("report.pdf", attachment.Name);
        Assert.Equal("application/pdf", attachment.ContentType.MediaType);
        Assert.Equal(3, attachment.ContentStream.Length);
    }



    [Fact]
    public void FromBytes_when_explicit_content_type_uses_override()
    {
        using var attachment = AttachmentFactory.FromBytes
        (
            new byte[] { 1 },
            "data.bin",
            "application/custom"
        );

        Assert.Equal("application/custom", attachment.ContentType.MediaType);
    }



    // ---------- FromBase64 ----------

    [Fact]
    public void FromBase64_when_base64Content_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => AttachmentFactory.FromBase64(null!, "test.bin")
        );
        Assert.Equal("base64Content", ex.ParamName);
    }



    [Fact]
    public void FromBase64_when_invalid_base64_throws_FormatException()
    {
        Assert.Throws<FormatException>
        (
            () => AttachmentFactory.FromBase64("not-valid-base64!!!", "test.bin")
        );
    }



    [Fact]
    public void FromBase64_when_valid_creates_attachment_with_decoded_content()
    {
        var original = new byte[] { 10, 20, 30 };
        var base64 = Convert.ToBase64String(original);

        using var attachment = AttachmentFactory.FromBase64(base64, "image.png");

        Assert.Equal("image.png", attachment.Name);
        Assert.Equal("image/png", attachment.ContentType.MediaType);
        Assert.Equal(3, attachment.ContentStream.Length);
    }



    // ---------- FromStream ----------

    [Fact]
    public void FromStream_when_content_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => AttachmentFactory.FromStream(null!, "test.bin")
        );
        Assert.Equal("content", ex.ParamName);
    }



    [Fact]
    public void FromStream_when_fileName_is_null_throws_ArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>
        (
            () => AttachmentFactory.FromStream(new MemoryStream(), null!)
        );
        Assert.Equal("fileName", ex.ParamName);
    }



    [Fact]
    public void FromStream_when_valid_copies_stream_content()
    {
        var original = new byte[] { 1, 2, 3, 4, 5 };
        using var stream = new MemoryStream(original);

        using var attachment = AttachmentFactory.FromStream(stream, "data.xlsx");

        Assert.Equal("data.xlsx", attachment.Name);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", attachment.ContentType.MediaType);
        Assert.Equal(5, attachment.ContentStream.Length);
    }



    [Fact]
    public void FromStream_does_not_modify_original_stream_position()
    {
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        stream.Position = 2;

        using var attachment = AttachmentFactory.FromStream(stream, "test.bin");

        Assert.Equal(2, stream.Position);
    }



    // ---------- InferContentType ----------

    [Theory]
    [InlineData("file.pdf", "application/pdf")]
    [InlineData("file.PDF", "application/pdf")]
    [InlineData("file.png", "image/png")]
    [InlineData("file.jpg", "image/jpeg")]
    [InlineData("file.csv", "text/csv")]
    [InlineData("file.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [InlineData("file.unknown", "application/octet-stream")]
    [InlineData("file", "application/octet-stream")]
    public void InferContentType_when_given_filename_returns_expected_type
    (
        string fileName,
        string expectedType
    )
    {
        Assert.Equal(expectedType, AttachmentFactory.InferContentType(fileName));
    }



    [Fact]
    public void InferContentType_when_null_throws_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>
        (
            () => AttachmentFactory.InferContentType(null!)
        );
    }
}
