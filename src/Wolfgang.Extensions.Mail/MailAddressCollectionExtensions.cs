using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// A collection of extension methods for the <see cref="MailAddressCollection"/> class.
/// </summary>
public static class MailAddressCollectionExtensions
{

    /// <summary>
    /// Adds a range of email addresses to the <see cref="MailAddressCollection"/>.
    /// </summary>
    /// <param name="source">The <see cref="MailAddressCollection"/> to add to</param>
    /// <param name="addresses">
    /// The list of email addresses to convert to <see cref="MailAddress"/>
    /// and add to the source</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="addresses"/> is null.</exception>
    /// <exception cref="FormatException">An entry in <paramref name="addresses"/> is not a valid email address.</exception>
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
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="addresses"/> is null.</exception>
    /// <exception cref="FormatException">An entry in <paramref name="addresses"/> is not a valid email address.</exception>
    public static void AddRange
    (
        this MailAddressCollection source,
        params string[] addresses
    )
        => source.AddRange((IEnumerable<string>)addresses);



    /// <summary>
    /// Adds a range of email addresses to the <see cref="MailAddressCollection"/>.
    /// </summary>
    /// <param name="source">The <see cref="MailAddressCollection"/> to add to</param>
    /// <param name="addresses">The list of <see cref="MailAddress"/> to add to the source</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="addresses"/> is null.</exception>
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
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="addresses"/> is null.</exception>
    public static void AddRange
    (
        this MailAddressCollection source,
        params MailAddress[] addresses
    )
        => source.AddRange((IEnumerable<MailAddress>)addresses);



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

        var escapedName = address.DisplayName
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
        return $"\"{escapedName}\" <{address.Address}>";
    }
}
