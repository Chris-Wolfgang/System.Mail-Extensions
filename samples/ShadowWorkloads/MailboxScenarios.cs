using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Wolfgang.Extensions.Mail.Validation;

namespace Wolfgang.Extensions.Mail.ShadowWorkloads;

/// <summary>
/// Realistic consumer workloads, each one operation of a plausible application rather than one
/// API call: ingesting an inbox, sending a personalised campaign, archiving messages, importing an
/// address book, and a service handling several mailboxes at once. Each scenario is also a usage
/// example of the public API.
/// </summary>
/// <remarks>
/// shadow.yaml runs this class against the current commit and against the last published package
/// on the same runner, and compare.py fails the run when a scenario regresses beyond its
/// threshold. Only APIs present in the baseline release may be used.
/// </remarks>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 12)]
public class MailboxScenarios
{

    private IReadOnlyList<string> _inbox = Array.Empty<string>();
    private IReadOnlyList<string> _addressBook = Array.Empty<string>();
    private byte[] _logo = Array.Empty<byte>();
    private byte[] _report = Array.Empty<byte>();



    [GlobalSetup]
    public void Setup()
    {
        // Printed into the BenchmarkDotNet log so each run shows which build it measured: the
        // source build carries the commit, the baseline run the published version.
        var version = typeof(EmlParser).Assembly
            .GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), inherit: false)
            .OfType<System.Reflection.AssemblyInformationalVersionAttribute>()
            .Select(a => a.InformationalVersion)
            .FirstOrDefault();
        Console.WriteLine("// Measuring Wolfgang.Extensions.Mail " + version);

        _inbox = MailTraffic.Inbox(count: 200, seed: 20261030);
        _addressBook = MailTraffic.AddressBook(count: 2_000, seed: 20261031);
        _logo = MailTraffic.Bytes(8_000, seed: 1);
        _report = MailTraffic.Bytes(150_000, seed: 2);
    }



    /// <summary>
    /// A mail-processing service reads a batch of raw messages, keeps the diagnostics of the ones
    /// that were malformed, validates the rest, and totals the attachment volume.
    /// </summary>
    [Benchmark]
    public long InboxIngestion()
    {
        var options = new ValidationOptions { MaxAttachmentSizeBytes = 10_000_000 };
        long attachmentBytes = 0;
        var problems = 0;

        foreach (var eml in _inbox)
        {
            var parsed = EmlParser.ParseWithDiagnostics(eml);
            using var message = parsed.Message;
            problems += parsed.Issues.Count;

            var validation = message.Validate(options);
            problems += validation.Errors.Count;
            attachmentBytes += message.Attachments.TotalSize();
        }

        return attachmentBytes + problems;
    }



    /// <summary>
    /// A marketing system builds one personalised HTML message per recipient, with an embedded logo
    /// and a PDF attachment, and serializes each to MIME for a queue.
    /// </summary>
    [Benchmark]
    public long OutboundCampaign()
    {
        long totalLength = 0;

        for (var i = 0; i < 50; i++)
        {
            var html = new InlineHtmlBuilder()
                .Html("<html><body><img src=\"cid:{0}\"/><p>Hello " + MailTraffic.DisplayName(i) + ",</p></body></html>")
                .EmbedImage(_logo, "logo.png")
                .Build();

            using var message = new MailMessageBuilder()
                .From("newsletter@example.com", "Example News")
                .To($"user{i}@example.com")
                .ReplyTo("support@example.com")
                .Subject(MailTraffic.Subject(i))
                .SubjectEncoding(Encoding.UTF8)
                .BodyEncoding(Encoding.UTF8)
                .PlainTextBody("Plain version for " + MailTraffic.DisplayName(i))
                .Header("List-Unsubscribe", $"<mailto:unsubscribe+{i}@example.com>")
                .Priority(MailPriority.Normal)
                .Attach(_report, "report.pdf")
                .Build();
            message.AlternateViews.Add(html);

            totalLength += message.ToMimeString().Length;
        }

        return totalLength;
    }



    /// <summary>
    /// An archiver copies each message, serializes the copy, and checks it reads back with the same
    /// number of attachments before storing it.
    /// </summary>
    [Benchmark]
    public int ArchiveRoundTrip()
    {
        var verified = 0;

        foreach (var eml in _inbox.Take(60))
        {
            using var original = EmlParser.Parse(eml);
            if (original.From == null)
            {
                continue;
            }

            using var copy = original.Clone();
            using var restored = EmlParser.Parse(copy.ToMimeString());
            verified += restored.Attachments.Count == original.Attachments.Count ? 1 : 0;
        }

        return verified;
    }



    /// <summary>
    /// A CRM imports contacts from a CSV export: valid addresses go into distribution lists, which
    /// are then rendered for display.
    /// </summary>
    [Benchmark]
    public int AddressBookImport()
    {
        using var message = new MailMessage();
        var valid = new List<MailAddress>(_addressBook.Count);

        foreach (var entry in _addressBook)
        {
            if (MailAddress.TryParse(entry, out var address))
            {
                valid.Add(address);
            }
        }

        message.To.AddRange(valid.Take(valid.Count / 2));
        message.Bcc.AddRange(valid.Skip(valid.Count / 2).Select(a => a.Address));

        return message.To.ToFormattedString().Length + message.Bcc.ToFormattedString(", ").Length;
    }



    /// <summary>
    /// A service handles several mailboxes at once: each worker parses its own share of the inbox
    /// and builds attachments for replies.
    /// </summary>
    [Benchmark]
    public async Task<long> ConcurrentMailboxesAsync()
    {
        var logo = Convert.ToBase64String(_logo);

        var totals = await Task.WhenAll
        (
            Enumerable
                .Range(0, 4)
                .Select
                (
                    worker => Task.Run
                    (
                        () =>
                        {
                            long total = 0;
                            foreach (var eml in _inbox.Where((_, index) => index % 4 == worker))
                            {
                                using var message = EmlParser.Parse(eml);
                                using var reply = AttachmentFactory.FromBase64(logo, "reply.png", AttachmentFactory.InferContentType("reply.png"));
                                total += message.Attachments.TotalSize() + reply.ContentStream.Length;
                            }

                            return total;
                        }
                    )
                )
        ).ConfigureAwait(false);

        return totals.Sum();
    }
}
