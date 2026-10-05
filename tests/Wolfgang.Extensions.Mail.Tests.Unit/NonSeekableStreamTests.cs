using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Pins the test double the non-seekable-stream tests rely on: if it ever
/// became seekable or writable, those tests would stop exercising the
/// non-seekable path.
/// </summary>
public class NonSeekableStreamTests
{

    [Fact]
    public void Capabilities_when_constructed_report_read_only_and_not_seekable()
    {
        using var sut = new NonSeekableStream(new byte[] { 1 });

        Assert.True(sut.CanRead);
        Assert.False(sut.CanSeek);
        Assert.False(sut.CanWrite);
    }



    [Fact]
    public void Read_when_called_returns_the_data_in_order()
    {
        using var sut = new NonSeekableStream(new byte[] { 1, 2, 3 });
        var buffer = new byte[4];

        var read = sut.Read(buffer, 0, buffer.Length);

        Assert.Equal(3, read);
        Assert.Equal(new byte[] { 1, 2, 3, 0 }, buffer);
    }



    [Fact]
    public void Seeking_members_when_called_throw_NotSupportedException()
    {
        using var sut = new NonSeekableStream(new byte[] { 1 });

        Assert.Throws<NotSupportedException>(() => sut.Length);
        Assert.Throws<NotSupportedException>(() => sut.Position);
        Assert.Throws<NotSupportedException>(() => sut.Position = 0);
        Assert.Throws<NotSupportedException>(() => sut.Seek(0, SeekOrigin.Begin));
        Assert.Throws<NotSupportedException>(() => sut.SetLength(0));
    }



    [Fact]
    public void Write_when_called_throws_NotSupportedException()
    {
        using var sut = new NonSeekableStream(new byte[] { 1 });

        Assert.Throws<NotSupportedException>(() => sut.Write(new byte[] { 2 }, 0, 1));
    }



    [Fact]
    public void Flush_when_called_does_not_throw()
    {
        using var sut = new NonSeekableStream(new byte[] { 1 });

        var exception = Record.Exception(() => sut.Flush());

        Assert.Null(exception);
    }



    [Fact]
    public void Dispose_when_called_disposes_the_underlying_data()
    {
        var sut = new NonSeekableStream(new byte[] { 1 });

        sut.Dispose();

        Assert.Throws<ObjectDisposedException>(() => sut.Read(new byte[1], 0, 1));
    }
}
