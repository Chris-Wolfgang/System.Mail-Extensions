using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// A collection of extension methods for the <see cref="AttachmentCollection"/> class.
/// </summary>
// ReSharper disable once UnusedType.Global
public static class AttachmentCollectionExtensions
{

    /// <summary>
    /// Adds a range of <see cref="Attachment"/> objects to the <see cref="AttachmentCollection"/>.
    /// </summary>
    /// <param name="source">The collection to add to</param>
    /// <param name="attachments">The list of <see cref="Attachment"/> to add</param>
    /// <exception cref="ArgumentNullException">source is null</exception>
    /// <exception cref="ArgumentNullException">attachments is null</exception>
    // ReSharper disable once UnusedMember.Global
    public static void AddRange
    (
        this AttachmentCollection source,
        params Attachment[] attachments
    )
    // ReSharper disable once InvokeAsExtensionMember
        => AddRange(source, (IEnumerable<Attachment>)attachments);



    /// <summary>
    /// Adds a range of <see cref="Attachment"/> objects to the <see cref="AttachmentCollection"/>.
    /// </summary>
    /// <param name="source">The collection to add to</param>
    /// <param name="attachments">The list of <see cref="Attachment"/> to add</param>
    /// <exception cref="ArgumentNullException">source is null</exception>
    /// <exception cref="ArgumentNullException">attachments is null</exception>
    // ReSharper disable once MemberCanBePrivate.Global
    // ReSharper disable once ConvertToExtensionBlock
    public static void AddRange
    (
        this AttachmentCollection source,
        IEnumerable<Attachment> attachments
    )
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (attachments == null)
        {
            throw new ArgumentNullException(nameof(attachments));
        }

        foreach (var attachment in attachments)
        {
            source.Add(attachment);
        }
    }



    /// <summary>
    /// Adds a range of <see cref="Attachment"/> objects to the <see cref="AttachmentCollection"/>.
    /// </summary>
    /// <param name="source">The collection to add to</param>
    /// <param name="fileNames">The list of <see cref="string"/> containing file names to add</param>
    /// <exception cref="ArgumentNullException">source is null</exception>
    /// <exception cref="ArgumentNullException">fileNames is null</exception>
    // ReSharper disable once MemberCanBePrivate.Global
    public static void AddRange
    (
        this AttachmentCollection source,
        IEnumerable<string> fileNames
    )
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (fileNames == null)
        {
            throw new ArgumentNullException(nameof(fileNames));
        }

        foreach (var fileName in fileNames)
        {
            source.Add(new Attachment(fileName));
        }
    }



    /// <summary>
    /// Adds a range of <see cref="Attachment"/> objects to the <see cref="AttachmentCollection"/>.
    /// </summary>
    /// <param name="source">The collection to add to</param>
    /// <param name="fileNames">The list of <see cref="string"/> containing file names to add</param>
    /// <exception cref="ArgumentNullException">source is null</exception>
    /// <exception cref="ArgumentNullException">fileNames is null</exception>
    // ReSharper disable once UnusedMember.Global
    public static void AddRange
    (
        this AttachmentCollection source,
        params string[] fileNames
    )
    // ReSharper disable once InvokeAsExtensionMember
        => AddRange(source, (IEnumerable<string>)fileNames);



    /// <summary>
    /// Returns the total size in bytes of all attachments in the <see cref="AttachmentCollection"/>
    /// whose underlying streams support seeking. Non-seekable streams are excluded from the total.
    /// </summary>
    /// <param name="source">The <see cref="AttachmentCollection"/> to measure.</param>
    /// <returns>The total size in bytes of all seekable attachment streams.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// using var message = new MailMessage();
    /// message.Attachments.AddRange("report.pdf", "data.csv");
    /// long totalBytes = message.Attachments.TotalSize();
    /// </code>
    /// </example>
    // ReSharper disable once UnusedMember.Global
    public static long TotalSize
    (
        this AttachmentCollection source
    )
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return source
            .Where(a => a.ContentStream.CanSeek)
            .Sum(a => a.ContentStream.Length);
    }



    /// <summary>
    /// Returns a value indicating whether the total size of all attachments exceeds
    /// the specified maximum number of bytes.
    /// </summary>
    /// <param name="source">The <see cref="AttachmentCollection"/> to check.</param>
    /// <param name="maxBytes">The maximum allowed total size in bytes.</param>
    /// <returns><c>true</c> if the total attachment size exceeds <paramref name="maxBytes"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public static bool ExceedsLimit
    (
        this AttachmentCollection source,
        long maxBytes
    )
    {
        return source.TotalSize() > maxBytes;
    }

}