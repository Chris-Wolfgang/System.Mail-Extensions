namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// A read-only, forward-only stream over a byte array, for tests that need a
/// <see cref="Stream"/> whose <see cref="Stream.CanSeek"/> is <see langword="false"/>.
/// </summary>
internal sealed class NonSeekableStream : Stream
{

    private readonly MemoryStream _inner;



    public NonSeekableStream(byte[] data) => _inner = new MemoryStream(data);



    public override bool CanRead => true;



    public override bool CanSeek => false;



    public override bool CanWrite => false;



    public override long Length => throw new NotSupportedException();



    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }



    public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);



    public override void Flush() => _inner.Flush();



    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();



    public override void SetLength(long value) => throw new NotSupportedException();



    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();



    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _inner.Dispose();
        }

        base.Dispose(disposing);
    }
}
