using System.Collections.Generic;
using System.Linq;

namespace Wolfgang.Extensions.Mail.Validation;


/// <summary>
/// Represents the result of validating a <see cref="System.Net.Mail.MailMessage"/>.
/// </summary>
public sealed class ValidationResult
{

    private readonly IReadOnlyList<ValidationIssue> _errors;
    private readonly IReadOnlyList<ValidationIssue> _warnings;
    private readonly IReadOnlyList<ValidationIssue> _allIssues;



    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationResult"/> class.
    /// </summary>
    /// <param name="issues">The list of validation issues found.</param>
    internal ValidationResult
    (
        List<ValidationIssue> issues
    )
    {
        var allIssues = issues ?? new List<ValidationIssue>();
        _allIssues = allIssues.AsReadOnly();
        _errors = allIssues.Where(i => i.Severity == ValidationSeverity.Error).ToList().AsReadOnly();
        _warnings = allIssues.Where(i => i.Severity == ValidationSeverity.Warning).ToList().AsReadOnly();
    }



    /// <summary>
    /// Gets a value indicating whether the message passed validation.
    /// Returns <c>true</c> when there are no errors (warnings do not affect validity).
    /// </summary>
    public bool IsValid => _errors.Count == 0;



    /// <summary>
    /// Gets all validation issues with <see cref="ValidationSeverity.Error"/> severity.
    /// </summary>
    public IReadOnlyList<ValidationIssue> Errors => _errors;



    /// <summary>
    /// Gets all validation issues with <see cref="ValidationSeverity.Warning"/> severity.
    /// </summary>
    public IReadOnlyList<ValidationIssue> Warnings => _warnings;



    /// <summary>
    /// Gets all validation issues regardless of severity.
    /// </summary>
    public IReadOnlyList<ValidationIssue> AllIssues => _allIssues;
}
