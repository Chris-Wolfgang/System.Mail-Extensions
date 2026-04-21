using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// A builder for creating HTML <see cref="AlternateView"/> instances with inline embedded images.
/// Manages <see cref="LinkedResource"/> creation and Content-ID wiring automatically.
/// </summary>
/// <example>
/// <code>
/// var builder = new InlineHtmlBuilder()
///     .Html("&lt;h1&gt;Report&lt;/h1&gt;&lt;img src='cid:{0}' /&gt;")
///     .EmbedImage("chart.png");
///
/// message.AlternateViews.Add(builder.Build());
/// </code>
/// </example>
// ReSharper disable once UnusedType.Global
public sealed class InlineHtmlBuilder
{

    private string? _htmlTemplate;
    private readonly List<LinkedResource> _resources = new List<LinkedResource>();
    private readonly List<string> _contentIds = new List<string>();



    /// <summary>
    /// Sets the HTML template. Use <c>{0}</c>, <c>{1}</c>, etc. as placeholders for
    /// embedded image Content-IDs, which are filled in order of <see cref="EmbedImage(string)"/> calls.
    /// </summary>
    /// <param name="htmlTemplate">The HTML template string with <c>{n}</c> placeholders for inline images.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="htmlTemplate"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public InlineHtmlBuilder Html
    (
        string htmlTemplate
    )
    {
        if (htmlTemplate == null)
        {
            throw new ArgumentNullException(nameof(htmlTemplate));
        }

        _htmlTemplate = htmlTemplate;
        return this;
    }



    /// <summary>
    /// Embeds an image from a file path as a <see cref="LinkedResource"/>.
    /// The content type is inferred from the file extension.
    /// </summary>
    /// <param name="filePath">The path to the image file.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public InlineHtmlBuilder EmbedImage
    (
        string filePath
    )
    {
        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        var contentType = InferImageContentType(Path.GetExtension(filePath));
        var contentId = GenerateContentId();

#pragma warning disable RS0030 // File I/O — reading image file into memory for embedding
        var bytes = File.ReadAllBytes(filePath);
#pragma warning restore RS0030

        var stream = new MemoryStream(bytes, writable: false);
        var resource = new LinkedResource(stream, contentType)
        {
            ContentId = contentId
        };

        _resources.Add(resource);
        _contentIds.Add(contentId);
        return this;
    }



    /// <summary>
    /// Embeds an image from a <see cref="Stream"/> as a <see cref="LinkedResource"/>.
    /// The stream content is copied; the original stream is not modified.
    /// </summary>
    /// <param name="imageStream">The stream containing the image data.</param>
    /// <param name="fileName">A file name used for content type inference (e.g., <c>"logo.png"</c>).</param>
    /// <param name="contentType">Optional explicit content type. When <c>null</c>, inferred from <paramref name="fileName"/>.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="imageStream"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public InlineHtmlBuilder EmbedImage
    (
        Stream imageStream,
        string fileName,
        string? contentType = null
    )
    {
        if (imageStream == null)
        {
            throw new ArgumentNullException(nameof(imageStream));
        }

        if (fileName == null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        var resolvedContentType = contentType ?? InferImageContentType(Path.GetExtension(fileName));
        var contentId = GenerateContentId();

        var destination = new MemoryStream();

#pragma warning disable RS0030 // In-memory stream copy
        if (imageStream.CanSeek)
        {
            var originalPosition = imageStream.Position;
            imageStream.Position = 0;
            imageStream.CopyTo(destination);
            imageStream.Position = originalPosition;
        }
        else
        {
            imageStream.CopyTo(destination);
        }
#pragma warning restore RS0030

        destination.Position = 0;

        var resource = new LinkedResource(destination, resolvedContentType)
        {
            ContentId = contentId
        };

        _resources.Add(resource);
        _contentIds.Add(contentId);
        return this;
    }



    /// <summary>
    /// Embeds an image from a byte array as a <see cref="LinkedResource"/>.
    /// </summary>
    /// <param name="imageBytes">The image content as a byte array.</param>
    /// <param name="fileName">A file name used for content type inference (e.g., <c>"chart.png"</c>).</param>
    /// <param name="contentType">Optional explicit content type. When <c>null</c>, inferred from <paramref name="fileName"/>.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="imageBytes"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public InlineHtmlBuilder EmbedImage
    (
        byte[] imageBytes,
        string fileName,
        string? contentType = null
    )
    {
        if (imageBytes == null)
        {
            throw new ArgumentNullException(nameof(imageBytes));
        }

        if (fileName == null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        var resolvedContentType = contentType ?? InferImageContentType(Path.GetExtension(fileName));
        var contentId = GenerateContentId();
        var stream = new MemoryStream(imageBytes, writable: false);

        var resource = new LinkedResource(stream, resolvedContentType)
        {
            ContentId = contentId
        };

        _resources.Add(resource);
        _contentIds.Add(contentId);
        return this;
    }



    /// <summary>
    /// Builds the <see cref="AlternateView"/> with all embedded images wired up.
    /// The HTML template placeholders (<c>{0}</c>, <c>{1}</c>, etc.) are replaced with
    /// the generated Content-IDs.
    /// </summary>
    /// <returns>An <see cref="AlternateView"/> containing the HTML and all linked resources.</returns>
    /// <exception cref="InvalidOperationException">No HTML template has been set via <see cref="Html"/>.</exception>
    // ReSharper disable once UnusedMember.Global
    public AlternateView Build()
    {
        if (_htmlTemplate == null)
        {
            throw new InvalidOperationException
            (
                "An HTML template must be set via Html() before calling Build()."
            );
        }

        var html = string.Format(System.Globalization.CultureInfo.InvariantCulture, _htmlTemplate, _contentIds.ToArray());

        var view = AlternateView.CreateAlternateViewFromString
        (
            html,
            null,
            MediaTypeNames.Text.Html
        );

        foreach (var resource in _resources)
        {
            view.LinkedResources.Add(resource);
        }

        return view;
    }



    private static string GenerateContentId()
    {
        return Guid.NewGuid().ToString("N");
    }



    private static string InferImageContentType
    (
        string extension
    )
    {
        switch (extension.ToLowerInvariant())
        {
            case ".png": return "image/png";
            case ".jpg":
            case ".jpeg": return "image/jpeg";
            case ".gif": return "image/gif";
            case ".bmp": return "image/bmp";
            case ".svg": return "image/svg+xml";
            case ".webp": return "image/webp";
            case ".ico": return "image/x-icon";
            default: return MediaTypeNames.Application.Octet;
        }
    }
}
