using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Wolfgang.Extensions.Mail.Benchmarks;

/// <summary>
/// Measures <see cref="MailMessageExtensions.ToMimeString"/> and
/// <see cref="MailMessageExtensions.Clone"/> on a message with an HTML
/// alternate view and an optional attachment. Repeated serialization of the
/// same message is safe: the framework re-reads attachment streams from the
/// start on every call (verified — output length is stable across calls).
/// </summary>
[MemoryDiagnoser]
public class MimeSerializationBenchmarks : IDisposable
{

    private MailMessage _message = null!;



    [Params(0, 64)]
    public int AttachmentSizeKb { get; set; }



    [GlobalSetup]
    public void Setup()
    {
        _message = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Subject = "Benchmark message",
            Body = "Plain text body for serialization benchmarks."
        };

        _message.AlternateViews.Add
        (
            AlternateView.CreateAlternateViewFromString
            (
                "<html><body><p>HTML body for serialization benchmarks.</p></body></html>",
                Encoding.UTF8,
                "text/html"
            )
        );

        if (AttachmentSizeKb > 0)
        {
            var payload = new byte[AttachmentSizeKb * 1024];
            for (var i = 0; i < payload.Length; i++)
            {
                payload[i] = (byte)(i % 256);
            }

            _message.Attachments.Add(AttachmentFactory.FromBytes(payload, "data.bin"));
        }
    }



    [GlobalCleanup]
    public void Cleanup()
    {
        Dispose();
    }



    [Benchmark]
    public string ToMimeString()
    {
        return _message.ToMimeString();
    }



    [Benchmark]
    public MailMessage Clone()
    {
        return _message.Clone();
    }



    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }



    protected virtual void Dispose
    (
        bool disposing
    )
    {
        if (disposing)
        {
            _message?.Dispose();
        }
    }
}
