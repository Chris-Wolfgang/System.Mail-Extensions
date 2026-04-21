using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// Static extension methods for the <see cref="MailAddress"/> class, including a
/// <c>TryParse</c> backfill for target frameworks that lack <c>MailAddress.TryCreate</c>.
/// </summary>
/// <remarks>
/// On .NET 5.0+, <c>TryParse</c> delegates to the built-in <c>MailAddress.TryCreate</c>.
/// On .NET Framework 4.6.2 and .NET Standard 2.0, it wraps the constructor in a try/catch.
/// </remarks>
// ReSharper disable once UnusedType.Global
public static class MailAddressExtensions
{

    extension(MailAddress)
    {

        /// <summary>
        /// Attempts to parse the specified string as a <see cref="MailAddress"/>.
        /// </summary>
        /// <param name="address">The email address string to parse.</param>
        /// <param name="result">
        /// When this method returns, contains the parsed <see cref="MailAddress"/> if parsing succeeded,
        /// or <c>null</c> if parsing failed.
        /// </param>
        /// <returns><c>true</c> if <paramref name="address"/> was parsed successfully; otherwise, <c>false</c>.</returns>
        /// <example>
        /// <code>
        /// if (MailAddress.TryParse("user@example.com", out var address))
        /// {
        ///     Console.WriteLine(address.DisplayName);
        /// }
        /// </code>
        /// </example>
        // ReSharper disable once UnusedMember.Global
        public static bool TryParse
        (
            string? address,
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            [NotNullWhen(true)]
#endif
            out MailAddress? result
        )
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                result = null;
                return false;
            }

#if NET5_0_OR_GREATER
            return MailAddress.TryCreate(address, out result);
#else
            try
            {
                result = new MailAddress(address);
                return true;
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentException)
            {
                result = null;
                return false;
            }
#endif
        }



        /// <summary>
        /// Attempts to parse the specified address and display name as a <see cref="MailAddress"/>.
        /// </summary>
        /// <param name="address">The email address string to parse.</param>
        /// <param name="displayName">The display name to associate with the address.</param>
        /// <param name="result">
        /// When this method returns, contains the parsed <see cref="MailAddress"/> if parsing succeeded,
        /// or <c>null</c> if parsing failed.
        /// </param>
        /// <returns><c>true</c> if <paramref name="address"/> was parsed successfully; otherwise, <c>false</c>.</returns>
        // ReSharper disable once UnusedMember.Global
        public static bool TryParse
        (
            string? address,
            string? displayName,
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
            [NotNullWhen(true)]
#endif
            out MailAddress? result
        )
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                result = null;
                return false;
            }

#if NET5_0_OR_GREATER
            return MailAddress.TryCreate(address, displayName, out result);
#else
            try
            {
                result = new MailAddress(address, displayName);
                return true;
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentException)
            {
                result = null;
                return false;
            }
#endif
        }
    }
}
