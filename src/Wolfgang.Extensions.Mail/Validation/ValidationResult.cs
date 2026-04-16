using System.Collections.Generic;
using System.Linq;

namespace Wolfgang.Extensions.Mail.Validation;


/// <summary>
/// Represents the result of validating a <see cref="System.Net.Mail.MailMessage"/>.
/// </summary>
public sealed class ValidationResult
{

    private readonly List<ValidationIssue> _issues;



    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class.
    /// </summary>
    /// <param name="issues">The list of validation issues found.</param>
    internal ValidationResult
    (
        List<ValidationIssue> issues
    )
    {
        _issues = issues ?? new List<ValidationIssue>();
    }



    /// <summary>
    /// Gets a value indicating whether the message passed validation.
    /// Returns <c>true</c> when there are no errors (warnings do not affect validity).
    /// </summary>
    public bool IsValid => !Errors.Any();



    /// <summary>
    /// Gets all validation issues with <see cref="ValidationSeverity.Error"/> severity.
    /// </summary>
    public IReadOnlyList<ValidationIssue> Errors =>
        _issues.Where(i => i.Severity == ValidationSeverity.Error).ToList().AsReadOnly();



    /// <summary>
    /// Gets all validation issues with <see cref="ValidationSeverity.Warning"/> severity.
    /// </summary>
    public IReadOnlyList<ValidationIssue> Warnings =>
        _issues.Where(i => i.Severity == ValidationSeverity.Warning).ToList().AsReadOnly();



    /// <summary>
    /// Gets all validation issues regardless of severity.
    /// </summary>
    public IReadOnlyList<ValidationIssue> AllIssues => _issues.AsReadOnly();
}
