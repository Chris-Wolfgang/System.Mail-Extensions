using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// A fluent builder for constructing <see cref="MailMessage"/> instances.
/// Simplifies the verbose ceremony of setting up mail messages.
/// </summary>
/// <example>
/// <code>
/// using var message = new MailMessageBuilder()
///     .From("sender@example.com", "Sender Name")
///     .To("alice@example.com", "bob@example.com")
///     .Subject("Monthly Report")
///     .HtmlBody("&lt;h1&gt;Report&lt;/h1&gt;")
///     .Attach("report.pdf")
///     .Build();
/// </code>
/// </example>
// ReSharper disable once UnusedType.Global
public sealed class MailMessageBuilder
{

    private MailAddress? _from;
    private MailAddress? _sender;
    private readonly List<MailAddress> _to = new List<MailAddress>();
    private readonly List<MailAddress> _cc = new List<MailAddress>();
    private readonly List<MailAddress> _bcc = new List<MailAddress>();
    private readonly List<MailAddress> _replyTo = new List<MailAddress>();
    private string? _subject;
    private string? _textBody;
    private string? _htmlBody;
    private MailPriority _priority = MailPriority.Normal;
    private readonly List<Attachment> _attachments = new List<Attachment>();
    private readonly Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.Ordinal);
    private Encoding? _bodyEncoding;
    private Encoding? _subjectEncoding;
    private DeliveryNotificationOptions _deliveryNotification = DeliveryNotificationOptions.None;



    /// <summary>
    /// Sets the From address.
    /// </summary>
    /// <param name="address">The sender email address.</param>
    /// <param name="displayName">Optional display name.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder From
    (
        string address,
        string? displayName = null
    )
    {
        if (address == null)
        {
            throw new ArgumentNullException(nameof(address));
        }

        _from = string.IsNullOrEmpty(displayName)
            ? new MailAddress(address)
            : new MailAddress(address, displayName);
        return this;
    }



    /// <summary>
    /// Sets the Sender address (the actual sender, if different from From).
    /// </summary>
    /// <param name="address">The sender email address.</param>
    /// <param name="displayName">Optional display name.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder SenderAddress
    (
        string address,
        string? displayName = null
    )
    {
        if (address == null)
        {
            throw new ArgumentNullException(nameof(address));
        }

        _sender = string.IsNullOrEmpty(displayName)
            ? new MailAddress(address)
            : new MailAddress(address, displayName);
        return this;
    }



    /// <summary>
    /// Adds one or more To recipients.
    /// </summary>
    /// <param name="addresses">The recipient email addresses.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="addresses"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder To
    (
        params string[] addresses
    )
    {
        if (addresses == null)
        {
            throw new ArgumentNullException(nameof(addresses));
        }

        foreach (var addr in addresses)
        {
            _to.Add(new MailAddress(addr));
        }

        return this;
    }



    /// <summary>
    /// Adds a To recipient with a display name.
    /// </summary>
    /// <param name="address">The recipient email address.</param>
    /// <param name="displayName">The display name.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="address"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder To
    (
        string address,
        string displayName
    )
    {
        if (address == null)
        {
            throw new ArgumentNullException(nameof(address));
        }

        _to.Add(new MailAddress(address, displayName));
        return this;
    }



    /// <summary>
    /// Adds one or more CC recipients.
    /// </summary>
    /// <param name="addresses">The CC email addresses.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="addresses"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder Cc
    (
        params string[] addresses
    )
    {
        if (addresses == null)
        {
            throw new ArgumentNullException(nameof(addresses));
        }

        foreach (var addr in addresses)
        {
            _cc.Add(new MailAddress(addr));
        }

        return this;
    }



    /// <summary>
    /// Adds one or more BCC recipients.
    /// </summary>
    /// <param name="addresses">The BCC email addresses.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="addresses"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder Bcc
    (
        params string[] addresses
    )
    {
        if (addresses == null)
        {
            throw new ArgumentNullException(nameof(addresses));
        }

        foreach (var addr in addresses)
        {
            _bcc.Add(new MailAddress(addr));
        }

        return this;
    }



    /// <summary>
    /// Adds one or more Reply-To addresses.
    /// </summary>
    /// <param name="addresses">The reply-to email addresses.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="addresses"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder ReplyTo
    (
        params string[] addresses
    )
    {
        if (addresses == null)
        {
            throw new ArgumentNullException(nameof(addresses));
        }

        foreach (var addr in addresses)
        {
            _replyTo.Add(new MailAddress(addr));
        }

        return this;
    }



    /// <summary>
    /// Sets the subject line.
    /// </summary>
    /// <param name="subject">The email subject.</param>
    /// <returns>This builder instance for chaining.</returns>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder Subject
    (
        string? subject
    )
    {
        _subject = subject;
        return this;
    }



    /// <summary>
    /// Sets the plain text body.
    /// </summary>
    /// <param name="body">The plain text body content.</param>
    /// <returns>This builder instance for chaining.</returns>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder PlainTextBody
    (
        string? body
    )
    {
        _textBody = body;
        return this;
    }



    /// <summary>
    /// Sets the HTML body.
    /// </summary>
    /// <param name="body">The HTML body content.</param>
    /// <returns>This builder instance for chaining.</returns>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder HtmlBody
    (
        string? body
    )
    {
        _htmlBody = body;
        return this;
    }



    /// <summary>
    /// Sets the mail priority.
    /// </summary>
    /// <param name="priority">The priority level.</param>
    /// <returns>This builder instance for chaining.</returns>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder Priority
    (
        MailPriority priority
    )
    {
        _priority = priority;
        return this;
    }



    /// <summary>
    /// Sets the delivery notification options.
    /// </summary>
    /// <param name="options">The notification options.</param>
    /// <returns>This builder instance for chaining.</returns>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder DeliveryNotification
    (
        DeliveryNotificationOptions options
    )
    {
        _deliveryNotification = options;
        return this;
    }



    /// <summary>
    /// Sets the body encoding.
    /// </summary>
    /// <param name="encoding">The encoding for the body.</param>
    /// <returns>This builder instance for chaining.</returns>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder BodyEncoding
    (
        Encoding encoding
    )
    {
        _bodyEncoding = encoding;
        return this;
    }



    /// <summary>
    /// Sets the subject encoding.
    /// </summary>
    /// <param name="encoding">The encoding for the subject.</param>
    /// <returns>This builder instance for chaining.</returns>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder SubjectEncoding
    (
        Encoding encoding
    )
    {
        _subjectEncoding = encoding;
        return this;
    }



    /// <summary>
    /// Adds a custom header.
    /// </summary>
    /// <param name="name">The header name.</param>
    /// <param name="value">The header value.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder Header
    (
        string name,
        string value
    )
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        _headers[name] = value;
        return this;
    }



    /// <summary>
    /// Attaches a file by path.
    /// </summary>
    /// <param name="filePath">The file path of the attachment.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder Attach
    (
        string filePath
    )
    {
        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        _attachments.Add(new Attachment(filePath));
        return this;
    }



    /// <summary>
    /// Attaches content from a stream.
    /// </summary>
    /// <param name="content">The stream containing the attachment content.</param>
    /// <param name="name">The display name for the attachment.</param>
    /// <param name="contentType">Optional MIME content type.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder Attach
    (
        Stream content,
        string name,
        string? contentType = null
    )
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        _attachments.Add
        (
            contentType != null
                ? new Attachment(content, name, contentType)
                : new Attachment(content, name)
        );
        return this;
    }



    /// <summary>
    /// Attaches content from a byte array.
    /// </summary>
    /// <param name="content">The attachment content.</param>
    /// <param name="name">The display name for the attachment.</param>
    /// <param name="contentType">Optional MIME content type.</param>
    /// <returns>This builder instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="content"/> is null.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
    // ReSharper disable once UnusedMember.Global
    public MailMessageBuilder Attach
    (
        byte[] content,
        string name,
        string? contentType = null
    )
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var stream = new MemoryStream(content, writable: false);

        _attachments.Add
        (
            contentType != null
                ? new Attachment(stream, name, contentType)
                : new Attachment(stream, name)
        );
        return this;
    }



    /// <summary>
    /// Builds a new <see cref="MailMessage"/> from the configured values.
    /// </summary>
    /// <returns>A fully constructed <see cref="MailMessage"/>.</returns>
    /// <exception cref="InvalidOperationException">From address has not been set.</exception>
    // ReSharper disable once UnusedMember.Global
    public MailMessage Build()
    {
        if (_from == null)
        {
            throw new InvalidOperationException
            (
                "From address is required. Call From() before Build()."
            );
        }

        var message = new MailMessage
        {
            From = _from,
            Subject = _subject,
            Priority = _priority,
            DeliveryNotificationOptions = _deliveryNotification
        };

        ApplyOptionalProperties(message);
        ApplyRecipients(message);
        ApplyBody(message);
        ApplyHeadersAndAttachments(message);

        return message;
    }



    private void ApplyOptionalProperties(MailMessage message)
    {
        if (_sender != null) { message.Sender = _sender; }
        if (_bodyEncoding != null) { message.BodyEncoding = _bodyEncoding; }
        if (_subjectEncoding != null) { message.SubjectEncoding = _subjectEncoding; }
    }



    private void ApplyRecipients(MailMessage message)
    {
        foreach (var addr in _to) { message.To.Add(addr); }
        foreach (var addr in _cc) { message.CC.Add(addr); }
        foreach (var addr in _bcc) { message.Bcc.Add(addr); }
        foreach (var addr in _replyTo) { message.ReplyToList.Add(addr); }
    }



    private void ApplyBody(MailMessage message)
    {
        if (_htmlBody != null && _textBody != null)
        {
            message.IsBodyHtml = false;
            message.Body = _textBody;

            var htmlView = AlternateView.CreateAlternateViewFromString
            (
                _htmlBody,
                _bodyEncoding ?? Encoding.UTF8,
                MediaTypeNames.Text.Html
            );
            message.AlternateViews.Add(htmlView);
        }
        else if (_htmlBody != null)
        {
            message.IsBodyHtml = true;
            message.Body = _htmlBody;
        }
        else
        {
            message.IsBodyHtml = false;
            message.Body = _textBody;
        }
    }



    private void ApplyHeadersAndAttachments(MailMessage message)
    {
        foreach (var kvp in _headers) { message.Headers[kvp.Key] = kvp.Value; }
        foreach (var attachment in _attachments) { message.Attachments.Add(attachment); }
    }
}
