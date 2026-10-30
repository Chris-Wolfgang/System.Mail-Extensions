using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Wolfgang.Extensions.Mail.ShadowWorkloads;

/// <summary>
/// Deterministic synthetic mail traffic with the shape of a real mailbox: mostly small plain-text
/// messages, some HTML newsletters with images, a few large attachments, non-ASCII names and
/// subjects, folded headers, and the occasional malformed message. The same seed always produces
/// the same traffic, so runs against two library versions measure identical input.
/// </summary>
internal static class MailTraffic
{

    private static readonly string[] Names =
    {
        "Alice Example", "Bob Builder", "Zoë Smith", "Jürgen Müller", "Last, First", "Ops <on call>",
        "Renée Dubois", "Søren Ørsted", "山田 太郎", "O'Brien",
    };



    private static readonly string[] Subjects =
    {
        "Quarterly report", "Re: lunch?", "Grüße aus Köln", "Invoice 2026-10", "Your order has shipped",
        "Überprüfung der Änderungen", "Meeting notes", "[ALERT] disk space low", "Fwd: photos", "週報",
    };



    /// <summary>Raw .eml documents for an ingestion run.</summary>
    internal static IReadOnlyList<string> Inbox(int count, int seed)
    {
        var random = new Random(seed);
        var messages = new List<string>(count);

        for (var i = 0; i < count; i++)
        {
            var roll = random.Next(100);
            messages.Add
            (
                roll switch
                {
                    < 60 => PlainText(random, i),
                    < 85 => HtmlWithInlineImage(random, i),
                    < 97 => WithAttachment(random, i, attachmentBytes: random.Next(2_000, 200_000)),
                    _ => Malformed(random, i),
                }
            );
        }

        return messages;
    }



    /// <summary>Addresses as they arrive from a CSV import: mostly valid, some not.</summary>
    internal static IReadOnlyList<string> AddressBook(int count, int seed)
    {
        var random = new Random(seed);
        var addresses = new List<string>(count);

        for (var i = 0; i < count; i++)
        {
            var name = Names[random.Next(Names.Length)];
            addresses.Add
            (
                random.Next(10) switch
                {
                    0 => $"not an address {i}",
                    1 => $"\"{name}\" <user{i}@example.com>",
                    2 => $"user{i}@example.com",
                    _ => $"user{i}.{random.Next(1000)}@mail{random.Next(20)}.example.org",
                }
            );
        }

        return addresses;
    }



    internal static string DisplayName(int i) => Names[i % Names.Length];



    internal static string Subject(int i) => Subjects[i % Subjects.Length];



    internal static byte[] Bytes(int length, int seed)
    {
        var bytes = new byte[length];
        new Random(seed).NextBytes(bytes);
        return bytes;
    }



    private static string Headers(Random random, int i, string contentType)
    {
        var name = Names[random.Next(Names.Length)];
        var subject = Subjects[random.Next(Subjects.Length)];
        var encodedSubject = "=?utf-8?B?" + Convert.ToBase64String(Encoding.UTF8.GetBytes(subject)) + "?=";
        var encodedName = "=?utf-8?B?" + Convert.ToBase64String(Encoding.UTF8.GetBytes(name)) + "?=";

        return string.Create
        (
            CultureInfo.InvariantCulture,
            $"From: {encodedName} <sender{i}@example.com>\r\n" +
            $"To: user{i}@example.com, \"Team {i % 7}\" <team{i % 7}@example.com>\r\n" +
            $"Cc: archive@example.com\r\n" +
            $"Subject: {encodedSubject}\r\n" +
            $"Date: Fri, 30 Oct 2026 10:{i % 60:00}:00 -0400\r\n" +
            $"Message-ID: <{i}.{random.Next()}@example.com>\r\n" +
            $"X-Mailer: ShadowWorkloads\r\n" +
            $"MIME-Version: 1.0\r\n" +
            $"Content-Type: {contentType}\r\n"
        );
    }



    private static string PlainText(Random random, int i)
    {
        var body = new StringBuilder();
        var lines = random.Next(3, 40);
        for (var line = 0; line < lines; line++)
        {
            body.Append("Line ").Append(line.ToString(CultureInfo.InvariantCulture)).Append(" of a message body, with some text.\r\n");
        }

        return Headers(random, i, "text/plain; charset=utf-8") +
            "Content-Transfer-Encoding: quoted-printable\r\n\r\n" +
            body;
    }



    private static string HtmlWithInlineImage(Random random, int i)
    {
        var image = Convert.ToBase64String(Bytes(random.Next(1_000, 20_000), i), Base64FormattingOptions.InsertLineBreaks);

        return Headers(random, i, "multipart/related; boundary=\"rel\"") +
            "\r\n--rel\r\nContent-Type: multipart/alternative; boundary=\"alt\"\r\n\r\n" +
            "--alt\r\nContent-Type: text/plain; charset=utf-8\r\n\r\nPlain version of the newsletter.\r\n" +
            "--alt\r\nContent-Type: text/html; charset=utf-8\r\n\r\n<html><body><h1>News</h1><img src=\"cid:logo\"></body></html>\r\n" +
            "--alt--\r\n" +
            "--rel\r\nContent-Type: image/png; name=\"logo.png\"\r\nContent-ID: <logo>\r\nContent-Transfer-Encoding: base64\r\n\r\n" +
            image + "\r\n--rel--\r\n";
    }



    private static string WithAttachment(Random random, int i, int attachmentBytes)
    {
        var attachment = Convert.ToBase64String(Bytes(attachmentBytes, i), Base64FormattingOptions.InsertLineBreaks);

        return Headers(random, i, "multipart/mixed; boundary=\"mix\"") +
            "\r\n--mix\r\nContent-Type: text/plain; charset=utf-8\r\n\r\nSee attached.\r\n" +
            "--mix\r\nContent-Type: application/pdf; name=\"report.pdf\"\r\n" +
            "Content-Disposition: attachment; filename=\"report.pdf\"\r\nContent-Transfer-Encoding: base64\r\n\r\n" +
            attachment + "\r\n--mix--\r\n";
    }



    // No empty custom header here: the 0.3.1 baseline throws on one (#338), which would make the
    // baseline run fail instead of measuring it.
    private static string Malformed(Random random, int i)
    {
        return string.Create
        (
            CultureInfo.InvariantCulture,
            $"From: broken sender {i}\r\nTo: also, not; valid\r\nSubject: =?bogus?Q?x?=\r\n\r\nBody {random.Next()}\r\n"
        );
    }
}
