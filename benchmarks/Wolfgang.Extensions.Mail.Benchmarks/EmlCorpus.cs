using System;
using System.Text;

namespace Wolfgang.Extensions.Mail.Benchmarks;

/// <summary>
/// Builds deterministic EML documents of the shapes the parser benchmarks exercise.
/// Kept separate from the benchmark classes so the corpus can be smoke-tested.
/// </summary>
internal static class EmlCorpus
{

    private const string Boundary = "=_bench_boundary_0123456789";



    internal static string BuildPlainText
    (
        int bodySizeBytes
    )
    {
        var sb = new StringBuilder(bodySizeBytes + 256);

        AppendCommonHeaders(sb);
        sb.Append("Content-Type: text/plain; charset=utf-8\r\n");
        sb.Append("\r\n");
        AppendPlainBody(sb, bodySizeBytes);

        return sb.ToString();
    }



    internal static string BuildMultipartAlternative
    (
        int bodySizeBytes
    )
    {
        var half = bodySizeBytes / 2;
        var sb = new StringBuilder(bodySizeBytes + 512);

        AppendCommonHeaders(sb);
        sb.Append("Content-Type: multipart/alternative; boundary=\"").Append(Boundary).Append("\"\r\n");
        sb.Append("\r\n");

        sb.Append("--").Append(Boundary).Append("\r\n");
        sb.Append("Content-Type: text/plain; charset=utf-8\r\n");
        sb.Append("\r\n");
        AppendPlainBody(sb, half);
        sb.Append("\r\n");

        sb.Append("--").Append(Boundary).Append("\r\n");
        sb.Append("Content-Type: text/html; charset=utf-8\r\n");
        sb.Append("\r\n");
        sb.Append("<html><body><p>");
        AppendPlainBody(sb, half);
        sb.Append("</p></body></html>\r\n");

        sb.Append("--").Append(Boundary).Append("--\r\n");

        return sb.ToString();
    }



    internal static string BuildWithBase64Attachment
    (
        int attachmentSizeBytes
    )
    {
        var payload = new byte[attachmentSizeBytes];
        for (var i = 0; i < payload.Length; i++)
        {
            payload[i] = (byte)(i % 256);
        }

        var sb = new StringBuilder(attachmentSizeBytes * 2 + 512);

        AppendCommonHeaders(sb);
        sb.Append("Content-Type: multipart/mixed; boundary=\"").Append(Boundary).Append("\"\r\n");
        sb.Append("\r\n");

        sb.Append("--").Append(Boundary).Append("\r\n");
        sb.Append("Content-Type: text/plain; charset=utf-8\r\n");
        sb.Append("\r\n");
        sb.Append("See attached file.\r\n");

        sb.Append("--").Append(Boundary).Append("\r\n");
        sb.Append("Content-Type: application/octet-stream; name=\"data.bin\"\r\n");
        sb.Append("Content-Transfer-Encoding: base64\r\n");
        sb.Append("Content-Disposition: attachment; filename=\"data.bin\"\r\n");
        sb.Append("\r\n");
        sb.Append(Convert.ToBase64String(payload, Base64FormattingOptions.InsertLineBreaks));
        sb.Append("\r\n");

        sb.Append("--").Append(Boundary).Append("--\r\n");

        return sb.ToString();
    }



    internal static string BuildQuotedPrintable
    (
        int bodySizeBytes
    )
    {
        var sb = new StringBuilder(bodySizeBytes * 2 + 256);

        AppendCommonHeaders(sb);
        sb.Append("Content-Type: text/plain; charset=utf-8\r\n");
        sb.Append("Content-Transfer-Encoding: quoted-printable\r\n");
        sb.Append("\r\n");

        var written = 0;
        var lineLength = 0;
        while (written < bodySizeBytes)
        {
            if (lineLength >= 72)
            {
                sb.Append("=\r\n");
                lineLength = 0;
            }

            if (written % 10 == 9)
            {
                sb.Append("=C3=A9");
                lineLength += 6;
            }
            else
            {
                sb.Append('a');
                lineLength++;
            }

            written++;
        }

        sb.Append("\r\n");

        return sb.ToString();
    }



    private static void AppendCommonHeaders
    (
        StringBuilder sb
    )
    {
        sb.Append("From: sender@example.com\r\n");
        sb.Append("To: recipient@example.com\r\n");
        sb.Append("Subject: Benchmark message\r\n");
        sb.Append("MIME-Version: 1.0\r\n");
    }



    private static void AppendPlainBody
    (
        StringBuilder sb,
        int bodySizeBytes
    )
    {
        const int lineLength = 72;
        var remaining = bodySizeBytes;

        while (remaining > 0)
        {
            var chunk = Math.Min(lineLength, remaining);
            sb.Append('a', chunk);
            sb.Append("\r\n");
            remaining -= chunk;
        }
    }
}
