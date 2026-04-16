using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// Provides factory methods for creating <see cref="Attachment"/> objects from common sources
/// with automatic content type inference from file extensions.
/// </summary>
// ReSharper disable once UnusedType.Global
public static class AttachmentFactory
{

    private static readonly Dictionary<string, string> ContentTypeMap = new Dictionary<string, string>
    (
        StringComparer.OrdinalIgnoreCase
    )
    {
        { ".pdf", "application/pdf" },
        { ".zip", "application/zip" },
        { ".gz", "application/gzip" },
        { ".json", "application/json" },
        { ".xml", "application/xml" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".ppt", "application/vnd.ms-powerpoint" },
        { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        { ".png", "image/png" },
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".gif", "image/gif" },
        { ".svg", "image/svg+xml" },
        { ".bmp", "image/bmp" },
        { ".ico", "image/x-icon" },
        { ".webp", "image/webp" },
        { ".txt", "text/plain" },
        { ".csv", "text/csv" },
        { ".html", "text/html" },
        { ".htm", "text/html" },
        { ".css", "text/css" },
        { ".js", "text/javascript" },
        { ".mp3", "audio/mpeg" },
        { ".wav", "audio/wav" },
        { ".mp4", "video/mp4" },
        { ".avi", "video/x-msvideo" },
        { ".eml", "message/rfc822" },
    };



    /// <summary>
    /// Creates an <see cref="Attachment"/> from a byte array with automatic content type inference.
    /// </summary>
    /// <param name="content">The attachment content as a byte array.</param>
    /// <param name="fileName">The file name for the attachment (used for content type inference and display).</param>
    /// <param name="contentType">Optional explicit content type. When <c>null</c>, inferred from <paramref name="fileName"/>.</param>
    /// <returns>A new <see cref="Attachment"/> backed by a <see cref="MemoryStream"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is null.</exception>
    /// <example>
    /// <code>
    /// byte[] pdfBytes = File.ReadAllBytes("report.pdf");
    /// var attachment = AttachmentFactory.FromBytes(pdfBytes, "report.pdf");
    /// </code>
    /// </example>
    // ReSharper disable once UnusedMember.Global
    public static Attachment FromBytes
    (
        byte[] content,
        string fileName,
        string? contentType = null
    )
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (fileName == null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        var stream = new MemoryStream(content, writable: false);
        var resolvedContentType = ResolveContentType(fileName, contentType);

        return new Attachment(stream, fileName, resolvedContentType);
    }



    /// <summary>
    /// Creates an <see cref="Attachment"/> from a base64-encoded string with automatic content type inference.
    /// </summary>
    /// <param name="base64Content">The attachment content as a base64-encoded string.</param>
    /// <param name="fileName">The file name for the attachment (used for content type inference and display).</param>
    /// <param name="contentType">Optional explicit content type. When <c>null</c>, inferred from <paramref name="fileName"/>.</param>
    /// <returns>A new <see cref="Attachment"/> backed by a <see cref="MemoryStream"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="base64Content"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is null.</exception>
    /// <exception cref="FormatException"><paramref name="base64Content"/> is not valid base64.</exception>
    // ReSharper disable once UnusedMember.Global
    public static Attachment FromBase64
    (
        string base64Content,
        string fileName,
        string? contentType = null
    )
    {
        if (base64Content == null)
        {
            throw new ArgumentNullException(nameof(base64Content));
        }

        if (fileName == null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        var bytes = Convert.FromBase64String(base64Content);
        return FromBytes(bytes, fileName, contentType);
    }



    /// <summary>
    /// Creates an <see cref="Attachment"/> from a <see cref="Stream"/> with automatic content type inference.
    /// The stream content is copied to a new <see cref="MemoryStream"/>; the original stream is not modified.
    /// </summary>
    /// <param name="content">The stream containing the attachment content.</param>
    /// <param name="fileName">The file name for the attachment (used for content type inference and display).</param>
    /// <param name="contentType">Optional explicit content type. When <c>null</c>, inferred from <paramref name="fileName"/>.</param>
    /// <returns>A new <see cref="Attachment"/> backed by a <see cref="MemoryStream"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public static Attachment FromStream
    (
        Stream content,
        string fileName,
        string? contentType = null
    )
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (fileName == null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        var destination = new MemoryStream();

#pragma warning disable RS0030 // In-memory stream copy; sync is appropriate
        if (content.CanSeek)
        {
            var originalPosition = content.Position;
            content.Position = 0;
            content.CopyTo(destination);
            content.Position = originalPosition;
        }
        else
        {
            content.CopyTo(destination);
        }
#pragma warning restore RS0030

        destination.Position = 0;
        var resolvedContentType = ResolveContentType(fileName, contentType);

        return new Attachment(destination, fileName, resolvedContentType);
    }



    /// <summary>
    /// Infers the MIME content type from a file name extension.
    /// Returns <c>application/octet-stream</c> for unknown extensions.
    /// </summary>
    /// <param name="fileName">The file name to infer the content type from.</param>
    /// <returns>The inferred MIME content type string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is null.</exception>
    // ReSharper disable once MemberCanBePrivate.Global
    public static string InferContentType
    (
        string fileName
    )
    {
        if (fileName == null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        var extension = Path.GetExtension(fileName);

        if (!string.IsNullOrEmpty(extension) && ContentTypeMap.TryGetValue(extension, out var mapped))
        {
            return mapped;
        }

        return MediaTypeNames.Application.Octet;
    }



    private static string ResolveContentType
    (
        string fileName,
        string? explicitContentType
    )
    {
        return explicitContentType ?? InferContentType(fileName);
    }
}
