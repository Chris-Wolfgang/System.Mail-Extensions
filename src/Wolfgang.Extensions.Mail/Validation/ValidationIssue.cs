using System;

namespace Wolfgang.Extensions.Mail.Validation;


/// <summary>
/// Represents a single validation issue found when validating a <see cref="System.Net.Mail.MailMessage"/>.
/// </summary>
public sealed class ValidationIssue
{

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationIssue"/> class.
    /// </summary>
    /// <param name="severity">The severity of the issue.</param>
    /// <param name="message">A human-readable description of the issue.</param>
    /// <param name="propertyName">The name of the property that caused the issue, or <c>null</c> if not applicable.</param>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is null.</exception>
    public ValidationIssue
    (
        ValidationSeverity severity,
        string message,
        string? propertyName = null
    )
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        Severity = severity;
        Message = message;
        PropertyName = propertyName;
    }



    /// <summary>
    /// Gets the severity of this validation issue.
    /// </summary>
    public ValidationSeverity Severity { get; }



    /// <summary>
    /// Gets the human-readable description of this validation issue.
    /// </summary>
    public string Message { get; }



    /// <summary>
    /// Gets the name of the property that caused this issue, or <c>null</c> if not applicable.
    /// </summary>
    public string? PropertyName { get; }



    /// <summary>
    /// Returns a string representation of this validation issue.
    /// </summary>
    /// <returns>A formatted string containing the severity, property name, and message.</returns>
    public override string ToString()
    {
        return PropertyName != null
            ? $"{Severity}: [{PropertyName}] {Message}"
            : $"{Severity}: {Message}";
    }
}
