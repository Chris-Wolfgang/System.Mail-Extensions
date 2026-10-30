using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Wolfgang.Extensions.Mail.Tests.Concurrency;

/// <summary>
/// Concurrency stress tests over the library's shared static state, under real thread-pool
/// contention: the process-wide content-type registry in <see cref="AttachmentFactory"/>, the
/// static compiled regexes and option defaults in <see cref="EmlParser"/>, and the reflection
/// <see cref="MailMessageExtensions.ToMimeString"/> uses to reach System.Net.Mail's internal
/// writer.
/// </summary>
/// <remarks>
/// <para>
/// Coyote (systematic schedule exploration) was considered and not used, for the same reason as in
/// Try-Pattern and IAsyncEnumerable-Extensions: the library has no async state machine of its own
/// and no lock-based coordination for a schedule explorer to permute; its only shared mutable
/// state is a <see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey,TValue}"/>. A
/// stress suite runs everywhere <c>dotnet test</c> does and exercises exactly that state.
/// </para>
/// <para>
/// <see cref="MailMessage"/> itself is not thread-safe (its attachment streams have one position),
/// so every test gives each caller its own message; sharing one message across threads is a
/// caller error, not something the library promises to support.
/// </para>
/// <para>
/// Each test records every outcome and checks them as a whole, so no line runs only on failure
/// (the assembly is held to the coverage gate like the other test projects).
/// </para>
/// </remarks>
public sealed class ConcurrencyStressTests
{

    private static readonly int StressIterations = ReadStressIterations();

    private static readonly int ConcurrentCallers = Math.Clamp(Environment.ProcessorCount * 2, 4, 16);



    private static int ReadStressIterations()
    {
        return int.TryParse(Environment.GetEnvironmentVariable("STRESS_ITERATIONS"), out var parsed) && parsed > 0
            ? parsed
            : 50;
    }



    private static Task<T[]> RunConcurrently<T>(Func<int, T> work)
    {
        return Task.WhenAll
        (
            Enumerable
                .Range(0, ConcurrentCallers)
                .Select(id => Task.Run(() => work(id)))
        );
    }



    [Fact]
    public async Task RegisterContentType_and_InferContentType_when_called_concurrently_see_every_registration()
    {
        for (var round = 0; round < StressIterations; round++)
        {
            var prefix = string.Create(CultureInfo.InvariantCulture, $".r{round}x");

            // Half the callers register a new extension each; the other half read a built-in one
            // and the extension registered by their partner, which may or may not have landed yet.
            var results = await RunConcurrently
            (
                id =>
                {
                    var extension = prefix + (id / 2).ToString(CultureInfo.InvariantCulture);
                    var contentType = "application/x-stress-" + (id / 2).ToString(CultureInfo.InvariantCulture);
                    if (id % 2 == 0)
                    {
                        AttachmentFactory.RegisterContentType(extension, contentType);
                    }

                    var builtIn = AttachmentFactory.InferContentType("report.pdf");
                    var custom = AttachmentFactory.InferContentType("file" + extension);
                    return (Extension: extension, ContentType: contentType, BuiltIn: builtIn, Custom: custom);
                }
            );

            Assert.All(results, r => Assert.Equal("application/pdf", r.BuiltIn));
            Assert.All(results, r => Assert.Contains(r.Custom, new[] { r.ContentType, "application/octet-stream" }));
            Assert.All
            (
                results,
                r => Assert.Equal(r.ContentType, AttachmentFactory.InferContentType("after" + r.Extension))
            );
        }
    }



    [Fact]
    public async Task Parse_when_called_concurrently_each_caller_gets_its_own_message()
    {
        for (var round = 0; round < StressIterations; round++)
        {
            var results = await RunConcurrently
            (
                id =>
                {
                    var eml =
                        $"From: \"Sender {id}\" <s{id}@example.com>\r\n" +
                        $"To: r{id}@example.com\r\n" +
                        $"Subject: =?utf-8?Q?Gr=C3=BC=C3=9Fe_{id}?=\r\n" +
                        "Content-Type: multipart/mixed; boundary=\"b\"\r\n\r\n" +
                        "--b\r\nContent-Type: text/plain\r\n\r\n" +
                        $"Body {id}\r\n" +
                        "--b\r\nContent-Type: text/csv; name=\"d.csv\"\r\nContent-Transfer-Encoding: base64\r\n\r\n" +
                        Convert.ToBase64String(Encoding.ASCII.GetBytes($"id,{id}")) + "\r\n--b--\r\n";

                    using var message = EmlParser.Parse(eml);
                    return (Id: id, From: message.From!.Address, message.Subject, message.Body, Attachment: message.Attachments[0].ContentStream.Length);
                }
            );

            Assert.All
            (
                results,
                r =>
                {
                    Assert.Equal($"s{r.Id}@example.com", r.From);
                    Assert.Equal($"Grüße {r.Id}", r.Subject);
                    Assert.Equal($"Body {r.Id}", r.Body);
                    Assert.Equal($"id,{r.Id}".Length, r.Attachment);
                }
            );
        }
    }



    [Fact]
    public async Task ToMimeString_when_called_concurrently_each_caller_gets_its_own_message()
    {
        for (var round = 0; round < StressIterations; round++)
        {
            var results = await RunConcurrently
            (
                id =>
                {
                    using var message = new MailMessageBuilder()
                        .From($"s{id}@example.com")
                        .To($"r{id}@example.com")
                        .Subject($"Subject {id}")
                        .PlainTextBody($"Body {id}")
                        .Attach(Encoding.ASCII.GetBytes($"payload {id}"), $"file{id}.txt")
                        .Build();

                    using var parsed = EmlParser.Parse(message.ToMimeString());
                    return (Id: id, parsed.Subject, parsed.Body, Attachment: parsed.Attachments[0].Name);
                }
            );

            Assert.All
            (
                results,
                r =>
                {
                    Assert.Equal($"Subject {r.Id}", r.Subject);
                    Assert.Equal($"Body {r.Id}", r.Body);
                    Assert.Equal($"file{r.Id}.txt", r.Attachment);
                }
            );
        }
    }



    [Fact]
    public async Task Clone_and_Validate_when_called_concurrently_on_separate_messages_are_independent()
    {
        for (var round = 0; round < StressIterations; round++)
        {
            var results = await RunConcurrently
            (
                id =>
                {
                    using var source = new MailMessageBuilder()
                        .From($"s{id}@example.com")
                        .To($"r{id}@example.com")
                        .Subject(id % 3 == 0 ? null : $"Subject {id}")
                        .PlainTextBody($"Body {id}")
                        .Attach(new byte[id + 1], $"file{id}.bin")
                        .Build();

                    using var clone = source.Clone();
                    var validation = clone.Validate(new Validation.ValidationOptions { RequireSubject = true });
                    return (Id: id, clone.Subject, Size: clone.Attachments.TotalSize(), validation.IsValid);
                }
            );

            Assert.All
            (
                results,
                r =>
                {
                    Assert.Equal(r.Id % 3 == 0 ? string.Empty : $"Subject {r.Id}", r.Subject ?? string.Empty);
                    Assert.Equal(r.Id + 1, r.Size);
                    Assert.Equal(r.Id % 3 != 0, r.IsValid);
                }
            );
        }
    }



    [Fact]
    public async Task ParseWithDiagnostics_when_called_concurrently_keeps_issues_per_call()
    {
        for (var round = 0; round < StressIterations; round++)
        {
            var results = await RunConcurrently
            (
                id =>
                {
                    // Even callers send a malformed From, odd callers a valid one: the issue list
                    // must belong to the call that produced it.
                    var from = id % 2 == 0 ? $"broken {id}" : $"ok{id}@example.com";
                    var result = EmlParser.ParseWithDiagnostics($"From: {from}\r\nTo: t@example.com\r\n\r\nBody");
                    using var message = result.Message;
                    return (Id: id, Issues: result.Issues.Select(i => i.Message).ToArray());
                }
            );

            Assert.All
            (
                results,
                r => Assert.Equal
                (
                    r.Id % 2 == 0 ? new[] { $"Malformed From address: 'broken {r.Id}'." } : Array.Empty<string>(),
                    r.Issues
                )
            );
        }
    }
}
