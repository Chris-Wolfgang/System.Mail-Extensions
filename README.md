# Wolfgang.Extensions.Mail

[![NuGet](https://img.shields.io/nuget/v/Wolfgang.Extensions.Mail.svg)](https://www.nuget.org/packages/Wolfgang.Extensions.Mail)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A collection of extension methods for working with email and mail-related functionality in `System.Net.Mail`.

## Installation

```bash
dotnet add package Wolfgang.Extensions.Mail
```

## Features

### AttachmentCollection Extensions

Add multiple attachments to a `MailMessage` in a single call:

```csharp
using Wolfgang.Extensions.Mail;

using var message = new MailMessage();

// Add attachments by object
message.Attachments.AddRange(attachment1, attachment2);

// Add attachments from an enumerable
message.Attachments.AddRange(attachmentList);

// Add attachments by file path
message.Attachments.AddRange("report.pdf", "data.csv");

// Add attachments from an enumerable of file paths
message.Attachments.AddRange(filePathList);
```

### MailAddressCollection Extensions

Add multiple email addresses to To, CC, or BCC collections:

```csharp
using Wolfgang.Extensions.Mail;

using var message = new MailMessage();

// Add addresses from strings
message.To.AddRange("alice@example.com", "bob@example.com");

// Add addresses from an enumerable of strings
message.To.AddRange(addressList);

// Add MailAddress objects
message.CC.AddRange(new MailAddress("alice@example.com"), new MailAddress("bob@example.com"));

// Add from an enumerable of MailAddress objects
message.Bcc.AddRange(mailAddressList);
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
