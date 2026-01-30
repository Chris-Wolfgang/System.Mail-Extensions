using System;
using System.Collections.Generic;
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

        foreach (var address in attachments)
        {
            source.Add(address);
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

}