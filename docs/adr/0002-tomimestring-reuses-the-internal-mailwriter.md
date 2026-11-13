# ADR-0002: `ToMimeString` reuses System.Net.Mail's internal `MailWriter` through reflection

- **Status**: Accepted
- **Date**: 2026-04-15

## Context

`MailMessage` has no public API that turns a message into RFC 5322 / MIME text. The only code in
the BCL that does it is the internal `System.Net.Mail.MailWriter`, driven by the non-public
`MailMessage.Send` (or `SendAsync` on runtimes that removed the synchronous one), which is what
`SmtpClient` uses for its pickup-directory delivery method.

The alternatives were:

- **Write our own MIME serializer.** Header folding, encoded words, transfer encodings,
  multipart boundaries, alternate views, linked resources and attachment dispositions would all
  have to match what `SmtpClient` actually sends. Every divergence is a bug report.
- **Take a dependency on MimeKit.** Correct and fast, but it pulls a large dependency into a
  library whose point is small extensions over `System.Net.Mail` (see
  [ADR-0003](0003-lenient-dependency-free-eml-parser.md)).
- **`SmtpClient` with `SpecifiedPickupDirectory`.** Public, but writes a file to disk per
  message, needs a temporary directory, and cannot target a `MemoryStream`.

## Decision

`ToMimeString` reaches `MailWriter` and `MailMessage.Send` / `SendAsync` through reflection
(`SerializeToMimeStream`, `CreateMailWriter` and `InvokeMailMessageSend` in
`MailMessageExtensions.cs`), probing for each signature the supported runtimes use (the
two-parameter `MailWriter(Stream, bool)` constructor on .NET 6+, the older one before it). It
writes into a `MemoryStream` behind a wrapper that absorbs `MailWriter`'s close. If no known
shape is found it throws `InvalidOperationException` rather than guessing.

## Consequences

- The output is what `System.Net.Mail` itself produces on that runtime, so it round-trips with
  `SmtpClient` and with `EmlParser`.
- The method depends on private implementation details of the BCL. A runtime update can rename
  or remove them; the test suite runs `ToMimeString` on every target framework to catch that, and
  the failure mode is a clear `InvalidOperationException`, not corrupt output.
- It is not trim- or Native-AOT-safe. `ToMimeString` carries `[RequiresUnreferencedCode]`
  (.NET 5+) and `[RequiresDynamicCode]` (.NET 7+), so trimmed and AOT consumers get a build-time
  warning at the call site. The rest of the library is `IsAotCompatible` on `net8.0`+.
- Sonar's S3011 (reflection on non-public members) is suppressed locally at the reflective
  helpers only, each with the reason.
- Whatever `Send` writes besides the message (for example the pickup-directory envelope headers)
  has to be controlled through the arguments passed to it; that is a review point whenever this
  code changes.
- If .NET ever ships a public MIME writer, this ADR should be superseded by one that switches to
  it.
