using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Xunit;
using Assert = Xunit.Assert;
#pragma warning disable CA1707
#pragma warning disable MA0074 // xUnit Assert.Contains/DoesNotContain triggers this for string overloads

namespace Wolfgang.Extensions.Mail.Tests.Unit;

public class InlineHtmlBuilderTests
{
    // ---------- Html ----------

    [Fact]
    public void Html_when_null_throws_ArgumentNullException()
    {
        var builder = new InlineHtmlBuilder();

        Assert.Throws<ArgumentNullException>
        (
            () => builder.Html(null!)
        );
    }



    // ---------- Build ----------

    [Fact]
    public void Build_when_no_html_set_throws_InvalidOperationException()
    {
        var builder = new InlineHtmlBuilder();

        Assert.Throws<InvalidOperationException>
        (
            () => builder.Build()
        );
    }



    [Fact]
    public void Build_when_html_only_returns_alternate_view_with_no_resources()
    {
        var builder = new InlineHtmlBuilder()
            .Html("<h1>Hello</h1>");

        using var view = builder.Build();

        Assert.Empty(view.LinkedResources);
        Assert.Contains("text/html", view.ContentType.MediaType);
    }



    // ---------- EmbedImage(byte[], string) ----------

    [Fact]
    public void EmbedImage_bytes_when_null_bytes_throws_ArgumentNullException()
    {
        var builder = new InlineHtmlBuilder();

        Assert.Throws<ArgumentNullException>
        (
            () => builder.EmbedImage((byte[])null!, "img.png")
        );
    }



    [Fact]
    public void EmbedImage_bytes_when_null_fileName_throws_ArgumentNullException()
    {
        var builder = new InlineHtmlBuilder();

        Assert.Throws<ArgumentNullException>
        (
            () => builder.EmbedImage(new byte[] { 1 }, null!)
        );
    }



    [Fact]
    public void EmbedImage_bytes_creates_linked_resource_with_content_id()
    {
        var builder = new InlineHtmlBuilder()
            .Html("<img src='cid:{0}' />")
            .EmbedImage(new byte[] { 1, 2, 3 }, "chart.png");

        using var view = builder.Build();

        Assert.Single(view.LinkedResources);
        Assert.False(string.IsNullOrEmpty(view.LinkedResources[0].ContentId));
        Assert.Equal("image/png", view.LinkedResources[0].ContentType.MediaType);
    }



    // ---------- EmbedImage(Stream, string) ----------

    [Fact]
    public void EmbedImage_stream_when_null_stream_throws_ArgumentNullException()
    {
        var builder = new InlineHtmlBuilder();

        Assert.Throws<ArgumentNullException>
        (
            () => builder.EmbedImage((Stream)null!, "img.png")
        );
    }



    [Fact]
    public void EmbedImage_stream_copies_content_and_preserves_original_position()
    {
        using var stream = new MemoryStream(new byte[] { 10, 20, 30 });
        stream.Position = 2;

        var builder = new InlineHtmlBuilder()
            .Html("<img src='cid:{0}' />")
            .EmbedImage(stream, "logo.jpg");

        Assert.Equal(2, stream.Position);

        using var view = builder.Build();

        Assert.Single(view.LinkedResources);
        Assert.Equal("image/jpeg", view.LinkedResources[0].ContentType.MediaType);
    }



    // ---------- Multiple images ----------

    [Fact]
    public void Build_when_multiple_images_replaces_all_placeholders()
    {
        var builder = new InlineHtmlBuilder()
            .Html("<img src='cid:{0}' /><img src='cid:{1}' />")
            .EmbedImage(new byte[] { 1 }, "a.png")
            .EmbedImage(new byte[] { 2 }, "b.jpg");

        using var view = builder.Build();

        Assert.Equal(2, view.LinkedResources.Count);

        // Verify the HTML contains the content IDs (not the placeholders)
        using var reader = new StreamReader(view.ContentStream);
        var html = reader.ReadToEnd();
        Assert.DoesNotContain("{0}", html);
        Assert.DoesNotContain("{1}", html);
        Assert.Contains(view.LinkedResources[0].ContentId, html);
        Assert.Contains(view.LinkedResources[1].ContentId, html);
    }



    // ---------- Content type inference ----------

    [Fact]
    public void EmbedImage_bytes_when_explicit_content_type_uses_override()
    {
        var builder = new InlineHtmlBuilder()
            .Html("<img src='cid:{0}' />")
            .EmbedImage(new byte[] { 1 }, "img.dat", "image/custom");

        using var view = builder.Build();

        Assert.Equal("image/custom", view.LinkedResources[0].ContentType.MediaType);
    }
}
