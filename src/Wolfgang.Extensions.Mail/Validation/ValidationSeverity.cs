namespace Wolfgang.Extensions.Mail.Validation;


/// <summary>
/// Represents the severity level of a validation issue found in a <see cref="System.Net.Mail.MailMessage"/>.
/// </summary>
public enum ValidationSeverity
{
    /// <summary>
    /// A non-blocking issue that may indicate a problem but does not prevent sending.
    /// </summary>
    Warning,

    /// <summary>
    /// A blocking issue that will prevent the message from being sent successfully.
    /// </summary>
    Error
}
