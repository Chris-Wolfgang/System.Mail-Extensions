using System;
#if NET462
using System.Runtime.Serialization;
#endif

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// Thrown by <see cref="EmlParser"/> when a malformed construct is encountered
/// while parsing in strict mode (see <see cref="EmlParserOptions.Strict"/>).
/// </summary>
/// <remarks>
/// Derives from <see cref="FormatException"/> so existing callers that catch
/// <see cref="FormatException"/> around parse calls continue to work.
/// </remarks>
#if NET462
[Serializable]
#endif
public sealed class EmlParseException : FormatException
{

    /// <summary>
    /// Initializes a new instance of the <see cref="EmlParseException"/> class.
    /// </summary>
    public EmlParseException()
    {
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="EmlParseException"/> class
    /// with a message describing the malformed construct.
    /// </summary>
    /// <param name="message">A message describing the parse failure.</param>
    public EmlParseException
    (
        string message
    )
        : base(message)
    {
    }



    /// <summary>
    /// Initializes a new instance of the <see cref="EmlParseException"/> class
    /// with a message and the underlying exception that caused the failure.
    /// </summary>
    /// <param name="message">A message describing the parse failure.</param>
    /// <param name="innerException">The exception that caused this failure.</param>
    public EmlParseException
    (
        string message,
        Exception innerException
    )
        : base(message, innerException)
    {
    }



#if NET462
    private EmlParseException
    (
        SerializationInfo info,
        StreamingContext context
    )
        : base(info, context)
    {
    }
#endif
}
