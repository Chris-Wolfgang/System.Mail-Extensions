# Getting Started

This guide will help you quickly get up and running with Wolfgang.Extensions.Mail.

## Prerequisites

- A runtime supported by one of the package's targets: .NET Framework 4.6.2+, .NET Standard 2.0 / 2.1 (which covers .NET Core 2.0+ and .NET 5-7), or .NET 8.0 / 9.0 / 10.0
- Visual Studio 2022, Rider, or Visual Studio Code

## Installation

### Via the .NET CLI

```bash
dotnet add package Wolfgang.Extensions.Mail
```

### Via Package Manager Console

```powershell
Install-Package Wolfgang.Extensions.Mail
```

## Quick Start

### Build, validate and save a message

```csharp
using System;
using System.IO;
using System.Net.Mail;
using Wolfgang.Extensions.Mail;

using var message = new MailMessageBuilder()
    .From("sender@example.com", "Sender Name")
    .To("alice@example.com")
    .Cc("bob@example.com", "carol@example.com")
    .Subject("Monthly Report")
    .PlainTextBody("The report is attached.")
    .Attach("report.pdf")
    .Build();

var result = message.Validate();
if (!result.IsValid)
{
    foreach (var issue in result.Errors)
    {
        Console.WriteLine($"{issue.PropertyName}: {issue.Message}");
    }
}

await File.WriteAllTextAsync("message.eml", message.ToMimeString());
```

### Parse an EML file

```csharp
using Wolfgang.Extensions.Mail;

using var parsed = await EmlParser.ParseFileAsync("message.eml", cancellationToken);
Console.WriteLine(parsed.Subject);
```

### Collection helpers on an existing message

```csharp
using System.Collections.Generic;
using System.Net.Mail;
using Wolfgang.Extensions.Mail;

using var message = new MailMessage();
message.From = new MailAddress("sender@example.com");

// Add multiple recipients at once
message.To.AddRange("alice@example.com", "bob@example.com");

// Add CC addresses from a list
var ccList = new List<string> { "manager@example.com", "team@example.com" };
message.CC.AddRange(ccList);

// Add multiple attachments by file path
message.Attachments.AddRange("report.pdf", "data.csv");
```

## Next Steps

- Explore the [API Reference](../api/index.md) for detailed documentation
- Read the [Introduction](introduction.md) for the full feature list
- The [README](https://github.com/Chris-Wolfgang/System.Mail-Extensions#readme) has a short example for every feature

## Common Issues

- `MailMessageBuilder.Build()` throws `InvalidOperationException` when the message has no From address or no recipient.
- File paths passed to `AttachmentCollection.AddRange` or `MailMessageBuilder.Attach` must exist, or a `FileNotFoundException` is thrown.
- `MailAddressCollection.AddRange` with string addresses throws `FormatException` for an invalid address; use `MailAddress.TryParse` to check first.
- `EmlParser` is lenient: a malformed address is skipped rather than reported. Use `EmlParser.ParseWithDiagnostics` to see what was skipped, or `new EmlParserOptions { Strict = true }` to throw instead.

## Additional Resources

- [GitHub Repository](https://github.com/Chris-Wolfgang/System.Mail-Extensions)
- [Contributing Guidelines](https://github.com/Chris-Wolfgang/System.Mail-Extensions/blob/main/CONTRIBUTING.md)
- [Report an Issue](https://github.com/Chris-Wolfgang/System.Mail-Extensions/issues)
