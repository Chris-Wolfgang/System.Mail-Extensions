namespace Wolfgang.Extensions.Mail.Validation;


/// <summary>
/// Provides configuration options for <see cref="System.Net.Mail.MailMessage"/> validation.
/// </summary>
public sealed class ValidationOptions
{

    /// <summary>
    /// Gets or sets the maximum allowed size in bytes for a single attachment.
    /// When set, attachments exceeding this size will produce a warning.
    /// </summary>
    public long? MaxAttachmentSizeBytes { get; set; }



    /// <summary>
    /// Gets or sets the maximum allowed total size in bytes for all attachments combined.
    /// When set, a total size exceeding this value will produce a warning.
    /// </summary>
    public long? MaxTotalAttachmentSizeBytes { get; set; }



    /// <summary>
    /// Gets or sets a value indicating whether an empty or null subject should be treated
    /// as an error instead of a warning. Defaults to <c>false</c>.
    /// </summary>
    public bool RequireSubject { get; set; }



    /// <summary>
    /// Gets or sets a value indicating whether an empty or null body (with no alternate views)
    /// should be treated as an error instead of a warning. Defaults to <c>false</c>.
    /// </summary>
    public bool RequireBody { get; set; }
}
