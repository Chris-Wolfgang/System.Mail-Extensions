# Getting Started

This guide will help you quickly get up and running with Wolfgang.Extensions.Mail.

## Prerequisites

- .NET Framework 4.6.2+ or .NET 6.0+ or .NET Standard 2.0 compatible runtime
- Visual Studio 2022, Rider, or Visual Studio Code

## Installation

### Via NuGet Package Manager

```bash
dotnet add package Wolfgang.Extensions.Mail
```

### Via Package Manager Console

```powershell
Install-Package Wolfgang.Extensions.Mail
```

## Quick Start

```csharp
using System.Net.Mail;
using Wolfgang.Extensions.Mail;

// Create a mail message
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
- Read the [Introduction](introduction.md) to learn more about Wolfgang.Extensions.Mail
- Check out example projects in the [GitHub repository](https://github.com/Chris-Wolfgang/System.Mail-Extensions)

## Common Issues

- Ensure file paths passed to `AttachmentCollection.AddRange` exist on disk, or a `FileNotFoundException` will be thrown.
- `MailAddressCollection.AddRange` with string addresses will throw `FormatException` for invalid email formats.

## Additional Resources

- [GitHub Repository](https://github.com/Chris-Wolfgang/System.Mail-Extensions)
- [Contributing Guidelines](https://github.com/Chris-Wolfgang/System.Mail-Extensions/blob/main/CONTRIBUTING.md)
- [Report an Issue](https://github.com/Chris-Wolfgang/System.Mail-Extensions/issues)
