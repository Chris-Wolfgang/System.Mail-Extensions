# Introduction

Welcome to Wolfgang.Extensions.Mail!

## Overview

Extension methods and helpers for `System.Net.Mail`: a fluent message builder, an EML parser and serializer, message validation and cloning, attachment factories with content-type inference, inline-HTML image embedding, and collection conveniences.

## Key Features

- **`MailMessageBuilder`** — build a `MailMessage` fluently; `Build()` reports every missing part (From, recipients) in one `InvalidOperationException`.
- **`EmlParser`** — parse RFC 2822 / MIME content (multipart bodies, base64 and quoted-printable, attachments, RFC 2047 encoded words) from a string or a file. Lenient by default; `EmlParserOptions { Strict = true }` throws `EmlParseException` instead, and `ParseWithDiagnostics` returns the skipped constructs alongside the message.
- **`ToMimeString()`** — serialize a `MailMessage` to an EML document that round-trips through `EmlParser.Parse`. It is the one API that is not trim/AOT-safe (it reflects over the framework's internal MIME writer).
- **`Validate()`** — check a message against `ValidationOptions` (required subject/body, per-attachment and total size limits) and get a `ValidationResult` with errors and warnings.
- **`Clone()`** — deep-copy a message, including alternate views, linked resources and attachments with independent streams.
- **`AttachmentFactory`** — create attachments from bytes, base64 or streams with the content type inferred from the file extension; the extension registry is extensible and thread-safe.
- **`InlineHtmlBuilder`** — build an HTML `AlternateView` with embedded images wired up through Content-IDs.
- **`MailAddress.TryParse`** — a non-throwing address parser on every target framework (delegates to `MailAddress.TryCreate` on .NET 5+).
- **Collection helpers** — `AddRange` for `AttachmentCollection` and `MailAddressCollection`, `TotalSize` / `ExceedsLimit` for attachments, and `ToFormattedString` / `FormatMailAddress` for addresses.

## Getting Help

If you need help with Wolfgang.Extensions.Mail, please:

- Check the [Getting Started](getting-started.md) guide
- Review the [API Reference](../api/index.md)
- Visit the [GitHub repository](https://github.com/Chris-Wolfgang/System.Mail-Extensions)
- Open an issue on [GitHub Issues](https://github.com/Chris-Wolfgang/System.Mail-Extensions/issues)
