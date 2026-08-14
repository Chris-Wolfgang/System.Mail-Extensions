using System;
using System.Net.Mail;
using BenchmarkDotNet.Attributes;

namespace Wolfgang.Extensions.Mail.Benchmarks;

/// <summary>
/// Measures <see cref="AttachmentFactory"/> creation paths, which scale with
/// payload size (base64 decode plus <see cref="System.IO.MemoryStream"/> copy).
/// </summary>
[MemoryDiagnoser]
public class AttachmentFactoryBenchmarks
{

    private byte[] _payload = Array.Empty<byte>();
    private string _base64 = string.Empty;



    // ReSharper disable once UnusedAutoPropertyAccessor.Global — [Params] is written by BenchmarkDotNet via reflection
    [Params(1_024, 1_048_576)]
    public int PayloadSizeBytes { get; set; }



    [GlobalSetup]
    public void Setup()
    {
        _payload = new byte[PayloadSizeBytes];
        for (var i = 0; i < _payload.Length; i++)
        {
            _payload[i] = (byte)(i % 256);
        }

        _base64 = Convert.ToBase64String(_payload);
    }



    [Benchmark(Baseline = true)]
    public Attachment FromBytes()
    {
        return AttachmentFactory.FromBytes(_payload, "data.bin");
    }



    [Benchmark]
    public Attachment FromBase64()
    {
        return AttachmentFactory.FromBase64(_base64, "data.bin");
    }
}
