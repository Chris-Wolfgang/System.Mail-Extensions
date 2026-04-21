using System;
using System.Collections.Concurrent;
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

    private static readonly ConcurrentDictionary<string, string> ContentTypeMap =
        new ConcurrentDictionary<string, string>
        (
            new[]
            {
                new KeyValuePair<string, string>(".pdf", "application/pdf"),
                new KeyValuePair<string, string>(".zip", "application/zip"),
                new KeyValuePair<string, string>(".gz", "application/gzip"),
                new KeyValuePair<string, string>(".json", "application/json"),
                new KeyValuePair<string, string>(".xml", "application/xml"),
                new KeyValuePair<string, string>(".doc", "application/msword"),
                new KeyValuePair<string, string>(".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
                new KeyValuePair<string, string>(".xls", "application/vnd.ms-excel"),
                new KeyValuePair<string, string>(".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"),
                new KeyValuePair<string, string>(".ppt", "application/vnd.ms-powerpoint"),
                new KeyValuePair<string, string>(".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"),
                new KeyValuePair<string, string>(".png", "image/png"),
                new KeyValuePair<string, string>(".jpg", "image/jpeg"),
                new KeyValuePair<string, string>(".jpeg", "image/jpeg"),
                new KeyValuePair<string, string>(".gif", "image/gif"),
                new KeyValuePair<string, string>(".svg", "image/svg+xml"),
                new KeyValuePair<string, string>(".bmp", "image/bmp"),
                new KeyValuePair<string, string>(".ico", "image/x-icon"),
                new KeyValuePair<string, string>(".webp", "image/webp"),
                new KeyValuePair<string, string>(".txt", "text/plain"),
                new KeyValuePair<string, string>(".csv", "text/csv"),
                new KeyValuePair<string, string>(".html", "text/html"),
                new KeyValuePair<string, string>(".htm", "text/html"),
                new KeyValuePair<string, string>(".css", "text/css"),
                new KeyValuePair<string, string>(".js", "text/javascript"),
                new KeyValuePair<string, string>(".mp3", "audio/mpeg"),
                new KeyValuePair<string, string>(".wav", "audio/wav"),
                new KeyValuePair<string, string>(".mp4", "video/mp4"),
                new KeyValuePair<string, string>(".avi", "video/x-msvideo"),
                new KeyValuePair<string, string>(".eml", "message/rfc822"),
            },
            StringComparer.OrdinalIgnoreCase
        );



    /// <summary>
    /// Registers or overrides a custom content type mapping for a file extension.
    /// Extension matching is case-insensitive. Registrations persist for the lifetime of the process.
    /// </summary>
    /// <param name="extension">The file extension (with leading dot, e.g. <c>".heic"</c>).</param>
    /// <param name="contentType">The MIME content type to associate with the extension.</param>
    /// <exception cref="ArgumentNullException"><paramref name="extension"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="contentType"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="extension"/> is empty or does not start with <c>"."</c>.</exception>
    /// <example>
    /// <code>
    /// AttachmentFactory.RegisterContentType(".heic", "image/heic");
    /// var attachment = AttachmentFactory.FromBytes(data, "photo.heic");
    /// // attachment.ContentType.MediaType == "image/heic"
    /// </code>
    /// </example>
    // ReSharper disable once UnusedMember.Global
    public static void RegisterContentType
    (
        string extension,
        string contentType
    )
    {
        if (extension == null)
        {
            throw new ArgumentNullException(nameof(extension));
        }

        if (contentType == null)
        {
            throw new ArgumentNullException(nameof(contentType));
        }

        if (extension.Length == 0 || extension[0] != '.')
        {
            throw new ArgumentException
            (
                "Extension must start with '.' (for example, \".heic\").",
                nameof(extension)
            );
        }

        ContentTypeMap[extension] = contentType;
    }



    /// <summary>
    /// Attempts to retrieve the registered content type for a file extension.
    /// </summary>
    /// <param name="extension">The file extension (with leading dot).</param>
    /// <param name="contentType">When this method returns, contains the registered content type if found; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if a content type was registered for the extension; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="extension"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public static bool TryGetRegisteredContentType
    (
        string extension,
        out string? contentType
    )
    {
        if (extension == null)
        {
            throw new ArgumentNullException(nameof(extension));
        }

        if (ContentTypeMap.TryGetValue(extension, out var value))
        {
            contentType = value;
            return true;
        }

        contentType = null;
        return false;
    }



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
