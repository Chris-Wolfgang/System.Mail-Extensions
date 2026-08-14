// GC.GetAllocatedBytesForCurrentThread is not available on .NET Framework, so
// these allocation guards are scoped to .NET 5+. The allocation-free TotalSize
// refactor they protect is still exercised on every TFM by
// AttachmentCollectionExtensionsTests.
#if NET5_0_OR_GREATER
using System.Net.Mail;
using System.Text;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Regression guards on allocation-sensitive paths, measured with
/// <see cref="GC.GetAllocatedBytesForCurrentThread"/> (minimum delta over many
/// samples after warm-up, so background noise doesn't inflate the reading).
/// </summary>
/// <remarks>
/// Guarded contracts:
/// <list type="bullet">
/// <item><c>AttachmentCollection.TotalSize()</c> / <c>ExceedsLimit()</c> — allocation-free (index loop, no LINQ / enumerator).</item>
/// <item><c>AttachmentFactory.FromBytes()</c> — wraps the payload without copying, so allocation is independent of payload size.</item>
/// <item><c>EmlParser.Parse()</c> of a quoted-printable body — bounded allocation (the single-pass decoder, not the old per-character path).</item>
/// </list>
/// </remarks>
public class AllocationBudgetTests
{

    [Fact]
    public void TotalSize_is_allocation_free()
    {
        using var message = BuildMessageWithAttachments(4);

        // ReSharper disable once AccessToDisposedClosure — MeasureMinAllocations runs the lambda synchronously, before `message` leaves scope
        var allocated = MeasureMinAllocations(() => Consume(message.Attachments.TotalSize()));

        Assert.True(allocated == 0, $"TotalSize allocated {allocated:N0} bytes; expected 0 (a LINQ/enumerator regression allocates ~100+).");
    }



    [Fact]
    public void ExceedsLimit_is_allocation_free()
    {
        using var message = BuildMessageWithAttachments(4);

        // ReSharper disable once AccessToDisposedClosure — MeasureMinAllocations runs the lambda synchronously, before `message` leaves scope
        var allocated = MeasureMinAllocations(() => Consume(message.Attachments.ExceedsLimit(1)));

        Assert.True(allocated == 0, $"ExceedsLimit allocated {allocated:N0} bytes; expected 0.");
    }



    [Fact]
    public void FromBytes_does_not_copy_the_payload()
    {
        var payload = new byte[1024 * 1024];   // 1 MB

        var allocated = MeasureMinAllocations(() =>
        {
            var attachment = AttachmentFactory.FromBytes(payload, "data.bin");
            Consume(attachment.Name!.Length);
            attachment.Dispose();
        });

        // FromBytes wraps the array in a MemoryStream without copying, so the
        // per-call allocation is a few small objects — nowhere near 1 MB. A
        // regression that copies the payload would allocate ~1,048,576 bytes.
        Assert.True(allocated < 16 * 1024, $"FromBytes allocated {allocated:N0} bytes for a 1 MB payload; expected < 16 KB (the payload must not be copied).");
    }



    [Fact]
    public void QuotedPrintable_parse_stays_within_budget()
    {
        var eml = BuildQuotedPrintableEml(100_000);

        var allocated = MeasureMinAllocations(() =>
        {
            var message = EmlParser.Parse(eml);
            Consume(message.Body.Length);
            message.Dispose();
        });

        // The single-pass QP decoder allocates roughly one worst-case buffer
        // plus the decoded string (~1 MB for a 100 KB body). The old
        // per-character decoder allocated ~7.4 MB. Budget catches that regression.
        Assert.True(allocated < 3_000_000, $"Parsing a 100 KB quoted-printable body allocated {allocated:N0} bytes; expected < 3,000,000 (the single-pass decoder may have regressed).");
    }



    // ------------------------------------------------------------------

    private static long MeasureMinAllocations
    (
        Action operation,
        int warmup = 10,
        int samples = 30
    )
    {
        for (var i = 0; i < warmup; i++)
        {
            operation();
        }

        var min = long.MaxValue;
        for (var i = 0; i < samples; i++)
        {
            var before = GC.GetAllocatedBytesForCurrentThread();
            operation();
            var delta = GC.GetAllocatedBytesForCurrentThread() - before;
            if (delta < min)
            {
                min = delta;
            }
        }

        return min;
    }



    // Kept in a field so the JIT cannot elide the measured work. The field is
    // write-only by design — reading it back would be observable and let the
    // JIT prove the work was unused, defeating the point.
#pragma warning disable S4487 // Sonar: unread private field — intentional (JIT-elision guard).
    // ReSharper disable once NotAccessedField.Local
    private static long _sink;
#pragma warning restore S4487

    private static void Consume(long value) => _sink = value;

    private static void Consume(int value) => _sink = value;

    private static void Consume(bool value) => _sink = value ? 1 : 0;



    private static MailMessage BuildMessageWithAttachments
    (
        int count
    )
    {
        var message = new MailMessage("from@example.com", "to@example.com");
        for (var i = 0; i < count; i++)
        {
            message.Attachments.Add(AttachmentFactory.FromBytes(new byte[64 * (i + 1)], $"a{i}.bin"));
        }

        return message;
    }



    private static string BuildQuotedPrintableEml
    (
        int bodyChars
    )
    {
        var sb = new StringBuilder(bodyChars * 2 + 256);
        sb.Append("From: a@example.com\r\nTo: b@example.com\r\n");
        sb.Append("Content-Type: text/plain; charset=utf-8\r\n");
        sb.Append("Content-Transfer-Encoding: quoted-printable\r\n\r\n");

        var lineLength = 0;
        for (var i = 0; i < bodyChars; i++)
        {
            if (lineLength >= 72)
            {
                sb.Append("=\r\n");
                lineLength = 0;
            }

            if (i % 10 == 9)
            {
                sb.Append("=C3=A9");   // é — exercises the =XX escape path
                lineLength += 6;
            }
            else
            {
                sb.Append('a');
                lineLength++;
            }
        }

        sb.Append("\r\n");
        return sb.ToString();
    }
}

#endif
