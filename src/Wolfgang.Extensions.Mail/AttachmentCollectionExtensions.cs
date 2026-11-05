using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// A collection of extension methods for the <see cref="AttachmentCollection"/> class.
/// </summary>
public static class AttachmentCollectionExtensions
{

    /// <summary>
    /// Adds a range of <see cref="Attachment"/> objects to the <see cref="AttachmentCollection"/>.
    /// </summary>
    /// <param name="source">The collection to add to</param>
    /// <param name="attachments">The list of <see cref="Attachment"/> to add</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="attachments"/> is null.</exception>
    public static void AddRange
    (
        this AttachmentCollection source,
        params Attachment[] attachments
    )
        => source.AddRange((IEnumerable<Attachment>)attachments);



    /// <summary>
    /// Adds a range of <see cref="Attachment"/> objects to the <see cref="AttachmentCollection"/>.
    /// </summary>
    /// <param name="source">The collection to add to</param>
    /// <param name="attachments">The list of <see cref="Attachment"/> to add</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="attachments"/> is null.</exception>
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
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileNames"/> is null.</exception>
    /// <exception cref="System.IO.FileNotFoundException">A file in <paramref name="fileNames"/> does not exist.</exception>
    /// <exception cref="System.IO.DirectoryNotFoundException">The directory of a file in <paramref name="fileNames"/> does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller cannot read a file in <paramref name="fileNames"/>.</exception>
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
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileNames"/> is null.</exception>
    /// <exception cref="System.IO.FileNotFoundException">A file in <paramref name="fileNames"/> does not exist.</exception>
    /// <exception cref="System.IO.DirectoryNotFoundException">The directory of a file in <paramref name="fileNames"/> does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller cannot read a file in <paramref name="fileNames"/>.</exception>
    public static void AddRange
    (
        this AttachmentCollection source,
        params string[] fileNames
    )
        => source.AddRange((IEnumerable<string>)fileNames);



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
    public static long TotalSize
    (
        this AttachmentCollection source
    )
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        // Index-based loop instead of LINQ .Where().Sum() so the method is
        // allocation-free — no iterator, delegate, or boxed enumerator. Guarded
        // by AllocationBudgetTests. checked() preserves the overflow-throwing
        // behavior of the previous Enumerable.Sum(long) (Sum is checked) while
        // staying allocation-free.
        long total = 0;
        for (var i = 0; i < source.Count; i++)
        {
            var contentStream = source[i].ContentStream;
            if (contentStream.CanSeek)
            {
                total = checked(total + contentStream.Length);
            }
        }

        return total;
    }



    /// <summary>
    /// Returns a value indicating whether the total size of all attachments exceeds
    /// the specified maximum number of bytes.
    /// </summary>
    /// <param name="source">The <see cref="AttachmentCollection"/> to check.</param>
    /// <param name="maxBytes">The maximum allowed total size in bytes.</param>
    /// <returns><c>true</c> if the total attachment size exceeds <paramref name="maxBytes"/>; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    public static bool ExceedsLimit
    (
        this AttachmentCollection source,
        long maxBytes
    )
    {
        return source.TotalSize() > maxBytes;
    }
}
