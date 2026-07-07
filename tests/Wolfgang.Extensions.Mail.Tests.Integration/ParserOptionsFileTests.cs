using System.Net.Mail;
using Wolfgang.Extensions.Mail;
using Xunit;

namespace Wolfgang.Extensions.Mail.Tests.Integration;


/// <summary>
/// End-to-end coverage for the file-based parser-options overloads:
/// strict and diagnostic parsing of real .eml files on disk.
/// </summary>
public sealed class ParserOptionsFileTests : IDisposable
{

    private const string MalformedEml =
        "From: sender@example.com\r\nTo: good@example.com, @@@\r\nSubject: Test\r\n\r\nBody.\r\n";

    private readonly string _tempDirectory;



    public ParserOptionsFileTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"mailext-opt-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDirectory);
    }



    public void Dispose()
    {
        try
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
        catch (IOException)
        {
            // Best-effort cleanup.
        }
    }



    [Fact]
    public async Task ParseFileAsync_strict_when_file_has_malformed_address_throws()
    {
        var path = await WriteAsync(MalformedEml);

        await Assert.ThrowsAsync<EmlParseException>
        (
            () => EmlParser.ParseFileAsync(path, new EmlParserOptions { Strict = true })
        );
    }



    [Fact]
    public async Task ParseFileAsync_lenient_when_file_has_malformed_address_keeps_the_good_one()
    {
        var path = await WriteAsync(MalformedEml);

        using var message = await EmlParser.ParseFileAsync(path, new EmlParserOptions { Strict = false });

        Assert.Equal("good@example.com", message.To.Single().Address);
    }



    [Fact]
    public async Task ParseFile_strict_when_file_has_malformed_address_throws()
    {
        var path = await WriteAsync(MalformedEml);

        Assert.Throws<EmlParseException>
        (
            () => EmlParser.ParseFile(path, new EmlParserOptions { Strict = true })
        );
    }



    private async Task<string> WriteAsync
    (
        string content
    )
    {
        var path = Path.Combine(_tempDirectory, $"{Guid.NewGuid():N}.eml");
        await File.WriteAllTextAsync(path, content);
        return path;
    }
}
