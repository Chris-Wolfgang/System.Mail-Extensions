using System.IO;

namespace Wolfgang.Extensions.Mail;


/// <summary>
/// Copies a stream into a new <see cref="MemoryStream"/>: the one copy routine behind
/// <see cref="AttachmentFactory"/>, <see cref="InlineHtmlBuilder"/> and
/// <see cref="MailMessageExtensions"/>' <c>Clone</c>.
/// </summary>
internal static class StreamCopy
{

#pragma warning disable RS0030 // BannedSymbols: Stream.CopyTo is banned for async-first, but every
                               // caller copies into memory synchronously by design.
    /// <summary>
    /// Copies <paramref name="source"/> into a new <see cref="MemoryStream"/> positioned at 0.
    /// A seekable source is read from position 0 and its position is restored afterwards; a
    /// non-seekable source is read from where it is to its end.
    /// </summary>
    /// <param name="source">The stream to copy.</param>
    /// <returns>A new <see cref="MemoryStream"/> holding the copied bytes, positioned at 0.</returns>
    internal static MemoryStream ToMemoryStream
    (
        Stream source
    )
    {
        var destination = new MemoryStream();

        if (source.CanSeek)
        {
            var originalPosition = source.Position;
            source.Position = 0;
            source.CopyTo(destination);
            source.Position = originalPosition;
        }
        else
        {
            source.CopyTo(destination);
        }

        destination.Position = 0;
        return destination;
    }
#pragma warning restore RS0030
}
