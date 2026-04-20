using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wolfgang.Extensions.Mail.Validation;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// A collection of extension methods for the <see cref="MailMessage"/> class.
/// </summary>
// ReSharper disable once UnusedType.Global
public static class MailMessageExtensions
{

    // ==========================================================================
    // Validate
    // ==========================================================================

    /// <summary>
    /// Validates the <see cref="MailMessage"/> and returns a <see cref="ValidationResult"/>
    /// containing any errors or warnings found. This method is pure and does not modify the message.
    /// </summary>
    /// <param name="source">The <see cref="MailMessage"/> to validate.</param>
    /// <returns>A <see cref="ValidationResult"/> indicating whether the message is valid.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// using var message = new MailMessage();
    /// var result = message.Validate();
    /// if (!result.IsValid)
    /// {
    ///     foreach (var error in result.Errors)
    ///         Console.WriteLine(error);
    /// }
    /// </code>
    /// </example>
    // ReSharper disable once UnusedMember.Global
    public static ValidationResult Validate
    (
        this MailMessage source
    )
    {
        return Validate(source, null);
    }



    /// <summary>
    /// Validates the <see cref="MailMessage"/> using the specified options and returns a
    /// <see cref="ValidationResult"/> containing any errors or warnings found.
    /// This method is pure and does not modify the message.
    /// </summary>
    /// <param name="source">The <see cref="MailMessage"/> to validate.</param>
    /// <param name="options">Optional validation configuration. Pass <c>null</c> for default behavior.</param>
    /// <returns>A <see cref="ValidationResult"/> indicating whether the message is valid.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    // ReSharper disable once MemberCanBePrivate.Global
    public static ValidationResult Validate
    (
        this MailMessage source,
        ValidationOptions? options
    )
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var issues = new List<ValidationIssue>();

        ValidateFrom(source, issues);
        ValidateRecipients(source, issues);
        ValidateSubject(source, options, issues);
        ValidateBody(source, options, issues);

        if (options?.MaxAttachmentSizeBytes != null || options?.MaxTotalAttachmentSizeBytes != null)
        {
            ValidateAttachmentSizes(source, options!, issues);
        }

        return new ValidationResult(issues);
    }



    // ==========================================================================
    // Clone
    // ==========================================================================

    /// <summary>
    /// Creates a deep copy of the <see cref="MailMessage"/>. All properties, address collections,
    /// attachments, alternate views, and linked resources are independently copied.
    /// The returned message can be modified and disposed independently of the original.
    /// </summary>
    /// <param name="source">The <see cref="MailMessage"/> to clone.</param>
    /// <returns>A new <see cref="MailMessage"/> that is an independent deep copy of <paramref name="source"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// using var original = new MailMessage("from@example.com", "to@example.com");
    /// original.Subject = "Hello";
    /// using var clone = original.Clone();
    /// clone.To.Add("extra@example.com");
    /// // original.To is unchanged
    /// </code>
    /// </example>
    // ReSharper disable once UnusedMember.Global
    public static MailMessage Clone
    (
        this MailMessage source
    )
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        var clone = new MailMessage();

        CopyScalarProperties(source, clone);
        CopyAddresses(source, clone);
        CopyHeaders(source, clone);
        CopyAttachments(source, clone);
        CopyAlternateViews(source, clone);

        return clone;
    }



    // ==========================================================================
    // ToMimeString
    // ==========================================================================

    /// <summary>
    /// Serializes the <see cref="MailMessage"/> to a raw MIME/EML format string.
    /// The output conforms to RFC 2822 and can be saved as a <c>.eml</c> file.
    /// </summary>
    /// <param name="source">The <see cref="MailMessage"/> to serialize.</param>
    /// <returns>A string containing the complete MIME representation of the message.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="MailMessage.From"/> property is not set, or the runtime does not support
    /// the internal MIME serialization API.
    /// </exception>
    /// <example>
    /// <code>
    /// using var message = new MailMessage("from@example.com", "to@example.com");
    /// message.Subject = "Test";
    /// message.Body = "Hello, World!";
    /// string eml = message.ToMimeString();
    /// </code>
    /// </example>
    // ReSharper disable once UnusedMember.Global
    public static string ToMimeString
    (
        this MailMessage source
    )
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (source.From == null)
        {
            throw new InvalidOperationException
            (
                "The From property must be set before serializing to MIME format."
            );
        }

        using var stream = new MemoryStream();
        SerializeToMimeStream(source, stream);
        return Encoding.UTF8.GetString(stream.ToArray());
    }



    /// <summary>
    /// Serializes the <see cref="MailMessage"/> to a raw MIME/EML format and saves it to a file.
    /// </summary>
    /// <param name="source">The <see cref="MailMessage"/> to serialize.</param>
    /// <param name="filePath">The file path to write the EML content to.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="MailMessage.From"/> property is not set.
    /// </exception>
    // ReSharper disable once UnusedMember.Global
    public static Task SaveToEmlAsync
    (
        this MailMessage source,
        string filePath,
        CancellationToken cancellationToken = default
    )
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        var mimeContent = source.ToMimeString();

#if NETSTANDARD2_0 || NET462
        cancellationToken.ThrowIfCancellationRequested();
#pragma warning disable RS0030 // File I/O — sync write on older TFMs that lack WriteAllTextAsync
        File.WriteAllText(filePath, mimeContent, Encoding.UTF8);
#pragma warning restore RS0030
        return Task.CompletedTask;
#else
        return File.WriteAllTextAsync(filePath, mimeContent, Encoding.UTF8, cancellationToken);
#endif
    }



    // ==========================================================================
    // Validate helpers
    // ==========================================================================

    private static void ValidateFrom
    (
        MailMessage source,
        List<ValidationIssue> issues
    )
    {
        if (source.From == null)
        {
            issues.Add
            (
                new ValidationIssue(ValidationSeverity.Error, "From address is required.", "From")
            );
        }
    }



    private static void ValidateRecipients
    (
        MailMessage source,
        List<ValidationIssue> issues
    )
    {
        if (source.To.Count == 0 && source.CC.Count == 0 && source.Bcc.Count == 0)
        {
            issues.Add
            (
                new ValidationIssue(ValidationSeverity.Error, "At least one recipient (To, CC, or BCC) is required.", "To")
            );
        }
    }



    private static void ValidateSubject
    (
        MailMessage source,
        ValidationOptions? options,
        List<ValidationIssue> issues
    )
    {
        if (string.IsNullOrWhiteSpace(source.Subject))
        {
            var severity = options?.RequireSubject == true
                ? ValidationSeverity.Error
                : ValidationSeverity.Warning;

            issues.Add
            (
                new ValidationIssue(severity, "Subject is empty or missing.", "Subject")
            );
        }
    }



    private static void ValidateBody
    (
        MailMessage source,
        ValidationOptions? options,
        List<ValidationIssue> issues
    )
    {
        if (string.IsNullOrWhiteSpace(source.Body) && source.AlternateViews.Count == 0)
        {
            var severity = options?.RequireBody == true
                ? ValidationSeverity.Error
                : ValidationSeverity.Warning;

            issues.Add
            (
                new ValidationIssue(severity, "Body is empty and no alternate views are defined.", "Body")
            );
        }
    }



    private static void ValidateAttachmentSizes
    (
        MailMessage source,
        ValidationOptions options,
        List<ValidationIssue> issues
    )
    {
        long totalSize = 0;

        foreach (var attachment in source.Attachments)
        {
            if (attachment.ContentStream.CanSeek)
            {
                var size = attachment.ContentStream.Length;
                totalSize += size;

                if (options.MaxAttachmentSizeBytes != null && size > options.MaxAttachmentSizeBytes.Value)
                {
                    issues.Add
                    (
                        new ValidationIssue
                        (
                            ValidationSeverity.Warning,
                            $"Attachment '{attachment.Name}' ({size:N0} bytes) exceeds the maximum allowed size of {options.MaxAttachmentSizeBytes.Value:N0} bytes.",
                            "Attachments"
                        )
                    );
                }
            }
        }

        if (options.MaxTotalAttachmentSizeBytes != null && totalSize > options.MaxTotalAttachmentSizeBytes.Value)
        {
            issues.Add
            (
                new ValidationIssue
                (
                    ValidationSeverity.Warning,
                    $"Total attachment size ({totalSize:N0} bytes) exceeds the maximum allowed total of {options.MaxTotalAttachmentSizeBytes.Value:N0} bytes.",
                    "Attachments"
                )
            );
        }
    }



    // ==========================================================================
    // Clone helpers
    // ==========================================================================

    private static void CopyScalarProperties
    (
        MailMessage source,
        MailMessage clone
    )
    {
        clone.Subject = source.Subject;
        clone.Body = source.Body;
        clone.IsBodyHtml = source.IsBodyHtml;
        clone.Priority = source.Priority;
        clone.DeliveryNotificationOptions = source.DeliveryNotificationOptions;
        clone.BodyEncoding = source.BodyEncoding;
        clone.SubjectEncoding = source.SubjectEncoding;
        clone.HeadersEncoding = source.HeadersEncoding;
        clone.BodyTransferEncoding = source.BodyTransferEncoding;
    }



    private static void CopyAddresses
    (
        MailMessage source,
        MailMessage clone
    )
    {
        if (source.From != null)
        {
            clone.From = CloneMailAddress(source.From)!;
        }

        if (source.Sender != null)
        {
            clone.Sender = CloneMailAddress(source.Sender)!;
        }

        CopyAddressCollection(source.To, clone.To);
        CopyAddressCollection(source.CC, clone.CC);
        CopyAddressCollection(source.Bcc, clone.Bcc);
        CopyAddressCollection(source.ReplyToList, clone.ReplyToList);
    }



    private static void CopyHeaders
    (
        MailMessage source,
        MailMessage clone
    )
    {
        foreach (string key in source.Headers)
        {
            var values = source.Headers.GetValues(key);
            if (values != null)
            {
                foreach (var value in values)
                {
                    clone.Headers.Add(key, value);
                }
            }
        }
    }



    private static void CopyAttachments
    (
        MailMessage source,
        MailMessage clone
    )
    {
        foreach (var attachment in source.Attachments)
        {
            clone.Attachments.Add(CloneAttachment(attachment));
        }
    }



    private static void CopyAlternateViews
    (
        MailMessage source,
        MailMessage clone
    )
    {
        foreach (var view in source.AlternateViews)
        {
            clone.AlternateViews.Add(CloneAlternateView(view));
        }
    }



    // ==========================================================================
    // ToMimeString helpers
    // ==========================================================================

#pragma warning disable S3011 // Reflection on non-public members is intentional for MIME serialization
    private static void SerializeToMimeStream
    (
        MailMessage source,
        MemoryStream stream
    )
    {
        var mailWriterType = typeof(SmtpClient).Assembly.GetType("System.Net.Mail.MailWriter");
        if (mailWriterType == null)
        {
            throw new InvalidOperationException
            (
                "Unable to locate the internal MailWriter type. " +
                "This runtime version may not support MIME serialization via reflection."
            );
        }

        var mailWriter = CreateMailWriter(mailWriterType, stream);

        try
        {
            InvokeMailMessageSend(source, mailWriter);
        }
        finally
        {
            var closeMethod = mailWriterType.GetMethod
            (
                "Close",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            closeMethod?.Invoke(mailWriter, Array.Empty<object>());
        }
    }
#pragma warning restore S3011



#pragma warning disable S3011  // Reflection on non-public members is intentional for MIME serialization
#pragma warning disable VSTHRD002 // Synchronous wait is intentional — SendAsync with SyncReadWriteAdapter completes synchronously
    private static void InvokeMailMessageSend
    (
        MailMessage source,
        object mailWriter
    )
    {
        // Try synchronous Send first (.NET Framework / .NET Core / .NET 5-9)
        var sendMethod = typeof(MailMessage).GetMethod
        (
            "Send",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        if (sendMethod != null)
        {
            sendMethod.Invoke(source, new[] { mailWriter, true, false });
            return;
        }

        // Fall back to SendAsync<TIOAdapter> (.NET 10+)
        var sendAsyncMethod = typeof(MailMessage).GetMethod
        (
            "SendAsync",
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        if (sendAsyncMethod == null)
        {
            throw new InvalidOperationException
            (
                "Unable to locate the internal MailMessage.Send or SendAsync method. " +
                "This runtime version may not support MIME serialization via reflection."
            );
        }

        var syncAdapterType = typeof(SmtpClient).Assembly.GetType
        (
            "System.Net.SyncReadWriteAdapter"
        );

        if (syncAdapterType == null)
        {
            throw new InvalidOperationException
            (
                "Unable to locate the internal SyncReadWriteAdapter type. " +
                "This runtime version may not support MIME serialization via reflection."
            );
        }

        var closedMethod = sendAsyncMethod.MakeGenericMethod(syncAdapterType);
        var result = closedMethod.Invoke
        (
            source,
            new[] { mailWriter, true, false, (object)CancellationToken.None }
        );

        if (result is Task task)
        {
            task.GetAwaiter().GetResult();
        }
    }
#pragma warning restore VSTHRD002
#pragma warning restore S3011



    // ==========================================================================
    // Shared helpers
    // ==========================================================================

    private static MailAddress? CloneMailAddress
    (
        MailAddress? address
    )
    {
        if (address == null)
        {
            return null;
        }

        return string.IsNullOrEmpty(address.DisplayName)
            ? new MailAddress(address.Address)
            : new MailAddress(address.Address, address.DisplayName);
    }



    private static void CopyAddressCollection
    (
        MailAddressCollection source,
        MailAddressCollection destination
    )
    {
        foreach (var address in source)
        {
            destination.Add
            (
                string.IsNullOrEmpty(address.DisplayName)
                    ? new MailAddress(address.Address)
                    : new MailAddress(address.Address, address.DisplayName)
            );
        }
    }



    private static Attachment CloneAttachment
    (
        Attachment source
    )
    {
        var clonedStream = CloneStream(source.ContentStream);

        var clone = new Attachment(clonedStream, new ContentType(source.ContentType.ToString()))
        {
            Name = source.Name,
            TransferEncoding = source.TransferEncoding
        };

        if (!string.IsNullOrEmpty(source.ContentId))
        {
            clone.ContentId = source.ContentId;
        }

        // Copy ContentDisposition properties
        if (source.ContentDisposition != null && clone.ContentDisposition != null)
        {
            clone.ContentDisposition.FileName = source.ContentDisposition.FileName;
            clone.ContentDisposition.Inline = source.ContentDisposition.Inline;
            clone.ContentDisposition.CreationDate = source.ContentDisposition.CreationDate;
            clone.ContentDisposition.ModificationDate = source.ContentDisposition.ModificationDate;
            clone.ContentDisposition.ReadDate = source.ContentDisposition.ReadDate;
            clone.ContentDisposition.Size = source.ContentDisposition.Size;
        }

        return clone;
    }



    private static AlternateView CloneAlternateView
    (
        AlternateView source
    )
    {
        var clonedStream = CloneStream(source.ContentStream);

        var clone = new AlternateView(clonedStream, new ContentType(source.ContentType.ToString()))
        {
            TransferEncoding = source.TransferEncoding,
            BaseUri = source.BaseUri
        };

        if (!string.IsNullOrEmpty(source.ContentId))
        {
            clone.ContentId = source.ContentId;
        }

        foreach (var linkedResource in source.LinkedResources)
        {
            clone.LinkedResources.Add(CloneLinkedResource(linkedResource));
        }

        return clone;
    }



    private static LinkedResource CloneLinkedResource
    (
        LinkedResource source
    )
    {
        var clonedStream = CloneStream(source.ContentStream);

        var clone = new LinkedResource(clonedStream, new ContentType(source.ContentType.ToString()))
        {
            TransferEncoding = source.TransferEncoding,
            ContentLink = source.ContentLink
        };

        if (!string.IsNullOrEmpty(source.ContentId))
        {
            clone.ContentId = source.ContentId;
        }

        return clone;
    }



#pragma warning disable RS0030 // BannedSymbols: Stream.CopyTo is banned for async-first,
                               // but these are in-memory streams where sync copy is appropriate.
    private static MemoryStream CloneStream
    (
        Stream source
    )
    {
        var destination = new MemoryStream();

        if (source.CanSeek)
        {
            var originalPosition = source.Position;
            source.Position = 0;
            source.CopyTo(destination);
            source.Position = originalPosition;
        }
        else
        {
            source.CopyTo(destination);
        }

        destination.Position = 0;
        return destination;
    }
#pragma warning restore RS0030



#pragma warning disable S3011 // Reflection on non-public members is intentional for MIME serialization
    private static object CreateMailWriter
    (
        Type mailWriterType,
        Stream stream
    )
    {
        // Try 2-parameter constructor first (net6.0+): MailWriter(Stream, bool)
        var ctor = mailWriterType.GetConstructor
        (
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
            null,
            new[] { typeof(Stream), typeof(bool) },
            null
        );

        if (ctor != null)
        {
            return ctor.Invoke(new object[] { stream, false });
        }

        // Fall back to 1-parameter constructor (net462): MailWriter(Stream)
        ctor = mailWriterType.GetConstructor
        (
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
            null,
            new[] { typeof(Stream) },
            null
        );

        if (ctor != null)
        {
            return ctor.Invoke(new object[] { stream });
        }

        throw new InvalidOperationException
        (
            "Unable to create MailWriter instance. " +
            "No supported constructor found for this runtime version."
        );
    }
#pragma warning restore S3011
}
