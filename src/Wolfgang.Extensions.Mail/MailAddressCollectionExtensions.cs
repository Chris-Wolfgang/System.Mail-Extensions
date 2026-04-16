using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

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



    /// <summary>
    /// Returns a formatted string representation of all addresses in the
    /// <see cref="MailAddressCollection"/>, using RFC 5322 display name quoting.
    /// </summary>
    /// <param name="source">The <see cref="MailAddressCollection"/> to format.</param>
    /// <param name="separator">The separator between addresses. Defaults to <c>"; "</c>.</param>
    /// <returns>A formatted string of all addresses.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <example>
    /// <code>
    /// using var message = new MailMessage();
    /// message.To.Add(new MailAddress("alice@example.com", "Alice Smith"));
    /// message.To.Add(new MailAddress("bob@example.com"));
    /// string formatted = message.To.ToFormattedString();
    /// // "Alice Smith" &lt;alice@example.com&gt;; bob@example.com
    /// </code>
    /// </example>
    // ReSharper disable once UnusedMember.Global
    public static string ToFormattedString
    (
        this MailAddressCollection source,
        string separator = "; "
    )
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return string.Join
        (
            separator,
            source.Select(FormatMailAddress)
        );
    }



    /// <summary>
    /// Returns a formatted RFC 5322 string representation of a single <see cref="MailAddress"/>.
    /// </summary>
    /// <param name="address">The <see cref="MailAddress"/> to format.</param>
    /// <returns>A formatted string such as <c>"Display Name" &lt;email@example.com&gt;</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
    // ReSharper disable once MemberCanBePrivate.Global
    public static string FormatMailAddress
    (
        this MailAddress address
    )
    {
        if (address == null)
        {
            throw new ArgumentNullException(nameof(address));
        }

        if (string.IsNullOrEmpty(address.DisplayName))
        {
            return address.Address;
        }

        return $"\"{address.DisplayName}\" <{address.Address}>";
    }

}
