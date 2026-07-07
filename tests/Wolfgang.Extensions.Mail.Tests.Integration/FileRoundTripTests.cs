using System.Net.Mail;
using Wolfgang.Extensions.Mail;
using Xunit;

namespace Wolfgang.Extensions.Mail.Tests.Integration;


/// <summary>
/// End-to-end file I/O: messages written to real .eml files on disk and read
/// back through <see cref="EmlParser.ParseFile(string)"/> /
/// <see cref="EmlParser.ParseFileAsync(string, System.Threading.CancellationToken)"/>.
/// </summary>
public sealed class FileRoundTripTests : IDisposable
{

    private readonly string _tempDirectory;



    public FileRoundTripTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"mailext-it-{Guid.NewGuid():N}");
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
            // Best-effort cleanup; leaked temp directories are harmless.
        }
    }



    [Fact]
    public async Task ParseFileAsync_when_file_contains_serialized_message_returns_equivalent_message()
    {
        using var original = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Subject = "File round trip",
            Body = "Body persisted to disk."
        };
        var path = await WriteEmlAsync(original);

        using var parsed = await EmlParser.ParseFileAsync(path);

        Assert.Equal("File round trip", parsed.Subject);
        Assert.Equal("sender@example.com", parsed.From!.Address);
    }



    [Fact]
    public async Task ParseFile_when_file_contains_serialized_message_returns_equivalent_message()
    {
        using var original = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Subject = "Sync file round trip",
            Body = "Body persisted to disk."
        };
        var path = await WriteEmlAsync(original);

        using var parsed = EmlParser.ParseFile(path);

        Assert.Equal("Sync file round trip", parsed.Subject);
        Assert.Equal("recipient@example.com", parsed.To.Single().Address);
    }



    [Fact]
    public async Task ParseFileAsync_when_attachment_is_one_megabyte_preserves_content()
    {
        var payload = new byte[1_048_576];
        for (var i = 0; i < payload.Length; i++)
        {
            payload[i] = (byte)(i % 256);
        }

        using var original = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Subject = "Large attachment",
            Body = "See attachment."
        };
        original.Attachments.Add(AttachmentFactory.FromBytes(payload, "large.bin"));
        var path = await WriteEmlAsync(original);

        using var parsed = await EmlParser.ParseFileAsync(path);

        var attachment = Assert.Single(parsed.Attachments);
        using var buffer = new MemoryStream();
        await attachment.ContentStream.CopyToAsync(buffer);
        Assert.Equal(payload, buffer.ToArray());
    }



    [Fact]
    public async Task ParseFileAsync_when_token_already_cancelled_throws_OperationCanceledException()
    {
        using var original = new MailMessage("sender@example.com", "recipient@example.com")
        {
            Subject = "Cancelled",
            Body = "Never read."
        };
        var path = await WriteEmlAsync(original);

        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>
        (
            () => EmlParser.ParseFileAsync(path, cts.Token)
        );
    }



    private async Task<string> WriteEmlAsync
    (
        MailMessage message
    )
    {
        var path = Path.Combine(_tempDirectory, $"{Guid.NewGuid():N}.eml");
        await File.WriteAllTextAsync(path, message.ToMimeString());
        return path;
    }
}
