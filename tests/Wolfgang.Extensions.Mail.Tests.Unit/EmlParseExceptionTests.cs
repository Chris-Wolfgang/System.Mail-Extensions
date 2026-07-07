using Wolfgang.Extensions.Mail;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Covers the public constructors and inheritance contract of
/// <see cref="EmlParseException"/>. Strict-mode parsing exercises only the
/// message constructor, so the parameterless and message+inner constructors are
/// verified directly here.
/// </summary>
public class EmlParseExceptionTests
{

    [Fact]
    public void Ctor_default_creates_a_FormatException_with_a_message()
    {
        var exception = new EmlParseException();

        Assert.IsAssignableFrom<FormatException>(exception);

        // The runtime supplies a default message for the parameterless ctor.
        Assert.False
        (
            string.IsNullOrEmpty(exception.Message)
        );
    }



    [Fact]
    public void Ctor_with_message_preserves_the_message()
    {
        const string message = "Malformed address in From header.";

        var exception = new EmlParseException(message);

        Assert.Equal
        (
            message,
            exception.Message
        );

        Assert.Null
        (
            exception.InnerException
        );
    }



    [Fact]
    public void Ctor_with_message_and_inner_preserves_both()
    {
        const string message = "Undecodable transfer encoding.";
        var inner = new InvalidOperationException("bad base64");

        var exception = new EmlParseException(message, inner);

        Assert.Equal
        (
            message,
            exception.Message
        );

        Assert.Same
        (
            inner,
            exception.InnerException
        );
    }
}
