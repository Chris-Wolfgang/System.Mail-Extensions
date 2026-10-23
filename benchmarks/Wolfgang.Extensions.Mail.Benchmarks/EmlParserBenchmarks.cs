using BenchmarkDotNet.Attributes;

namespace Wolfgang.Extensions.Mail.Benchmarks;

/// <summary>
/// Measures <see cref="EmlParser"/> throughput and allocations across the
/// message shapes that exercise its distinct code paths: plain text,
/// multipart/alternative, base64 attachments, and quoted-printable bodies.
/// </summary>
[MemoryDiagnoser]
public class EmlParserBenchmarks
{

    private string _plainTextEml = string.Empty;
    private string _multipartAlternativeEml = string.Empty;
    private string _attachmentEml = string.Empty;
    private string _quotedPrintableEml = string.Empty;



    // ReSharper disable once UnusedAutoPropertyAccessor.Global — [Params] is written by BenchmarkDotNet via reflection
    [Params(1_000, 100_000)]
    public int PayloadSizeBytes { get; set; }



    [GlobalSetup]
    public void Setup()
    {
        _plainTextEml = EmlCorpus.BuildPlainText(PayloadSizeBytes);
        _multipartAlternativeEml = EmlCorpus.BuildMultipartAlternative(PayloadSizeBytes);
        _attachmentEml = EmlCorpus.BuildWithBase64Attachment(PayloadSizeBytes);
        _quotedPrintableEml = EmlCorpus.BuildQuotedPrintable(PayloadSizeBytes);
    }



    [Benchmark(Baseline = true)]
    public int ParsePlainText()
    {
        // Disposed here so each iteration does not leak its attachment streams into the
        // measured allocations; the return value keeps the parse observable.
        using var message = EmlParser.Parse(_plainTextEml);
        return message.Attachments.Count + message.AlternateViews.Count;
    }



    [Benchmark]
    public int ParseMultipartAlternative()
    {
        // Disposed here so each iteration does not leak its attachment streams into the
        // measured allocations; the return value keeps the parse observable.
        using var message = EmlParser.Parse(_multipartAlternativeEml);
        return message.Attachments.Count + message.AlternateViews.Count;
    }



    [Benchmark]
    public int ParseWithBase64Attachment()
    {
        // Disposed here so each iteration does not leak its attachment streams into the
        // measured allocations; the return value keeps the parse observable.
        using var message = EmlParser.Parse(_attachmentEml);
        return message.Attachments.Count + message.AlternateViews.Count;
    }



    [Benchmark]
    public int ParseQuotedPrintable()
    {
        // Disposed here so each iteration does not leak its attachment streams into the
        // measured allocations; the return value keeps the parse observable.
        using var message = EmlParser.Parse(_quotedPrintableEml);
        return message.Attachments.Count + message.AlternateViews.Count;
    }
}
