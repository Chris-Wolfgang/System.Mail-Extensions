# Wolfgang.Extensions.Mail

[![NuGet](https://img.shields.io/nuget/v/Wolfgang.Extensions.Mail.svg)](https://www.nuget.org/packages/Wolfgang.Extensions.Mail)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

Extension methods and helpers for `System.Net.Mail`: a fluent message builder, an EML parser and serializer, message validation and cloning, attachment factories with content-type inference, inline-HTML image embedding, and collection conveniences.

Full API documentation: <https://chris-wolfgang.github.io/System.Mail-Extensions/>

## Installation

```bash
dotnet add package Wolfgang.Extensions.Mail
```

## Features

### Build messages fluently — `MailMessageBuilder`

```csharp
using Wolfgang.Extensions.Mail;

using var message = new MailMessageBuilder()
    .From("sender@example.com", "Sender Name")
    .To("alice@example.com", "bob@example.com")
    .Cc("archive@example.com")
    .Subject("Monthly Report")
    .PlainTextBody("The report is attached.")
    .HtmlBody("<h1>Report</h1><p>The report is attached.</p>")
    .Attach("report.pdf")
    .Build();
```

`Build()` throws `InvalidOperationException` if the message has no From address or no recipient, reporting every missing part in one message. Also available: `Bcc`, `ReplyTo`, `SenderAddress`, `Priority`, `DeliveryNotification`, `BodyEncoding`, `SubjectEncoding`, `Header`, and `Attach` overloads for byte arrays and streams.

### Parse EML files — `EmlParser`

Parse RFC 2822 / MIME content — including multipart bodies, base64 and quoted-printable transfer encodings, attachments, and RFC 2047 encoded-word headers — into a `MailMessage`:

```csharp
using var fromString = EmlParser.Parse(emlContent);
using var fromFile   = EmlParser.ParseFile("message.eml");
using var fromFileAsync = await EmlParser.ParseFileAsync("message.eml", cancellationToken);
```

Parsing is deliberately **lenient**, because real-world EML files routinely contain malformed headers: an address that cannot be parsed is skipped, and a malformed `From` header leaves `message.From` null. Pair with `Validate()` to detect what a lenient parse dropped.

### Serialize to EML — `ToMimeString()`

```csharp
string eml = message.ToMimeString();
await File.WriteAllTextAsync("message.eml", eml);
```

Produces a complete RFC 2822 MIME document, round-trippable through `EmlParser.Parse`.

### Validate messages — `Validate()`

```csharp
var result = message.Validate(new ValidationOptions
{
    RequireSubject = true,
    RequireBody = true,
    MaxAttachmentSizeBytes = 10 * 1024 * 1024,
    MaxTotalAttachmentSizeBytes = 25 * 1024 * 1024
});

if (!result.IsValid)
{
    foreach (var issue in result.Errors)
    {
        Console.WriteLine($"{issue.PropertyName}: {issue.Message}");
    }
}
```

`ValidationResult` exposes `IsValid`, `Errors`, `Warnings`, and `AllIssues`. Calling `Validate()` without options checks the essentials (From address, at least one recipient).

### Deep-copy messages — `Clone()`

```csharp
using var clone = original.Clone();
clone.To.Add("extra@example.com");   // original is unchanged
```

Copies addresses, headers, bodies, alternate views, linked resources, and attachments (with independent streams).

### Create attachments — `AttachmentFactory`

```csharp
var fromBytes  = AttachmentFactory.FromBytes(pdfBytes, "report.pdf");
var fromBase64 = AttachmentFactory.FromBase64(base64Content, "photo.jpg");
var fromStream = AttachmentFactory.FromStream(stream, "data.csv");
```

Content types are inferred from the file extension. The registry is extensible:

```csharp
AttachmentFactory.RegisterContentType(".heic", "image/heic");
string contentType = AttachmentFactory.InferContentType("photo.heic");   // "image/heic"
bool known = AttachmentFactory.TryGetRegisteredContentType(".heic", out var registered);
```

### Inline HTML images — `InlineHtmlBuilder`

Build an HTML `AlternateView` with embedded images wired up via Content-IDs — `{0}`, `{1}`, … placeholders are filled in the order images are embedded:

```csharp
var view = new InlineHtmlBuilder()
    .Html("<h1>Report</h1><img src='cid:{0}' />")
    .EmbedImage("chart.png")
    .Build();

message.AlternateViews.Add(view);
```

`EmbedImage` overloads accept a file path, a byte array, or a stream.

### Parse addresses safely — `MailAddress.TryParse`

Extension members that backfill `MailAddress.TryCreate` on frameworks that lack it (on .NET 5+ they delegate to the built-in):

```csharp
if (MailAddress.TryParse("user@example.com", out var address))
{
    Console.WriteLine(address.Address);
}

MailAddress.TryParse("user@example.com", "Display Name", out var withName);
```

### Attachment collection helpers

```csharp
message.Attachments.AddRange(attachment1, attachment2);       // params, enumerable,
message.Attachments.AddRange("report.pdf", "data.csv");       // file paths, or
message.Attachments.AddRange(filePathList);                   // enumerable of paths

long totalBytes = message.Attachments.TotalSize();
bool tooBig = message.Attachments.ExceedsLimit(25 * 1024 * 1024);
```

### Address collection helpers

```csharp
message.To.AddRange("alice@example.com", "bob@example.com");
message.CC.AddRange(mailAddressList);

string formatted = message.To.ToFormattedString();
// "Alice Smith" <alice@example.com>; bob@example.com

string one = new MailAddress("alice@example.com", "Alice Smith").FormatMailAddress();
```

## Supported Frameworks

- .NET Framework 4.6.2+
- .NET Standard 2.0
- .NET Standard 2.1
- .NET 8.0
- .NET 9.0
- .NET 10.0

## License

This project is licensed under the [MIT License](LICENSE).
