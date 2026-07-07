using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Mail;
using Wolfgang.Extensions.Mail.Validation;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// The result of a diagnostic parse: the parsed <see cref="MailMessage"/>
/// together with the list of malformed constructs the lenient parser skipped.
/// </summary>
/// <remarks>
/// Returned by
/// <see cref="EmlParser.ParseWithDiagnostics(string, EmlParserOptions?)"/>.
/// Each entry in <see cref="Issues"/> is a <see cref="ValidationIssue"/>
/// describing something the parser could not read (a malformed address, an
/// undecodable body part, a malformed encoded word).
/// </remarks>
public sealed class ParseResult
{

    private readonly IReadOnlyList<ValidationIssue> _issues;



    internal ParseResult
    (
        MailMessage message,
        IReadOnlyList<ValidationIssue> issues
    )
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));

        if (issues == null)
        {
            throw new ArgumentNullException(nameof(issues));
        }

        // Wrap in a genuinely read-only view so a caller can't down-cast the
        // exposed IReadOnlyList back to the underlying List and mutate it.
        _issues = new ReadOnlyCollection<ValidationIssue>
        (
            issues as IList<ValidationIssue> ?? new List<ValidationIssue>(issues)
        );
    }



    /// <summary>
    /// The parsed message. Populated on a best-effort basis: constructs that
    /// could not be read are reported in <see cref="Issues"/> and omitted from
    /// the message.
    /// </summary>
    public MailMessage Message { get; }



    /// <summary>
    /// The malformed constructs the parser skipped, in the order they were
    /// encountered. Empty when the content parsed cleanly.
    /// </summary>
    public IReadOnlyList<ValidationIssue> Issues => _issues;



    /// <summary>
    /// <c>true</c> when the parser skipped at least one malformed construct.
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public bool HasIssues => _issues.Count > 0;
}
