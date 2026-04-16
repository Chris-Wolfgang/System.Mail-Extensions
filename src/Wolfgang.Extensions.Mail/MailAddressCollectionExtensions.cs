using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// A collection of extension methods for the <see cref="MailAddressCollection"/> class.
/// </summary>
// ReSharper disable once UnusedType.Global
public static class MailAddressCollectionExtensions
{

    /// <summary>
    /// Adds a range of email addresses to the <see cref="MailAddressCollection"/>.
    /// </summary>
    /// <param name="source">The <see cref="MailAddressCollection"/> to add to</param>
    /// <param name="addresses">
    /// The list of email addresses to convert to <see cref="MailAddress"/>
    /// and add to the source</param>
    /// <exception cref="ArgumentNullException">source is null</exception>
    /// <exception cref="ArgumentNullException">addresses is null</exception>
    // ReSharper disable once MemberCanBePrivate.Global
    public static void AddRange
    (
        this MailAddressCollection source,
        IEnumerable<string> addresses
    )
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (addresses == null)
        {
            throw new ArgumentNullException(nameof(addresses));
        }

        foreach (var address in addresses)
        {
            source.Add(address);
        }
    }



    /// <summary>
    /// Adds a range of email addresses to the <see cref="MailAddressCollection"/>.
    /// </summary>
    /// <param name="source">The <see cref="MailAddressCollection"/> to add to</param>
    /// <param name="addresses">
    /// The list of email addresses to convert to <see cref="MailAddress"/>
    /// and add to the source</param>
    /// <exception cref="ArgumentNullException">source is null</exception>
    /// <exception cref="ArgumentNullException">addresses is null</exception>
    // ReSharper disable once UnusedMember.Global
    public static void AddRange
    (
        this MailAddressCollection source,
        params string[] addresses
    )
    // ReSharper disable once InvokeAsExtensionMember
        => AddRange(source, (IEnumerable<string>)addresses);



    /// <summary>
    /// Adds a range of email addresses to the <see cref="MailAddressCollection"/>.
    /// </summary>
    /// <param name="source">The <see cref="MailAddressCollection"/> to add to</param>
    /// <param name="addresses">The list of <see cref="MailAddress"/> to add to the source</param>
    /// <exception cref="ArgumentNullException">source is null</exception>
    /// <exception cref="ArgumentNullException">addresses is null</exception>
    // ReSharper disable once MemberCanBePrivate.Global
    public static void AddRange
    (
        this MailAddressCollection source,
        IEnumerable<MailAddress> addresses
    )
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (addresses == null)
        {
            throw new ArgumentNullException(nameof(addresses));
        }

        foreach (var address in addresses)
        {
            source.Add(address);
        }
    }



    /// <summary>
    /// Adds a range of email addresses to the <see cref="MailAddressCollection"/>.
    /// </summary>
    /// <param name="source">The <see cref="MailAddressCollection"/> to add to</param>
    /// <param name="addresses">The list of <see cref="MailAddress"/> to add to the source</param>
    /// <exception cref="ArgumentNullException">source is null</exception>
    /// <exception cref="ArgumentNullException">addresses is null</exception>
    // ReSharper disable once UnusedMember.Global
    public static void AddRange
    (
        this MailAddressCollection source,
        params MailAddress[] addresses
    )
    // ReSharper disable once InvokeAsExtensionMember
        => AddRange(source, (IEnumerable<MailAddress>)addresses);

}
