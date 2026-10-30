# ADR-0003: `EmlParser` is a lenient, dependency-free parser with opt-in strict mode

- **Status**: Accepted
- **Date**: 2026-07-06

## Context

Real-world `.eml` files are routinely malformed: unparseable addresses, a broken `From`, headers
that violate RFC 5322, unknown charsets. A parser that throws on the first defect is unusable for
the main use case (reading mail somebody else produced), while a parser that silently drops data
hides problems from callers who need to know.

The library also ships to `net462` and `netstandard2.0` with **no runtime package dependencies**,
and bringing in a full MIME library (MimeKit) for parsing would change that.

## Decision

- `EmlParser` is hand-written over `System.Net.Mail` types and the BCL only; the package keeps
  zero runtime dependencies.
- The default (`Parse`, `ParseFile`, `ParseFileAsync`) is **lenient**: an address that cannot be
  parsed is skipped, a malformed `From` leaves `MailMessage.From` null, and a body that cannot be
  decoded keeps its original text. Callers detect what was dropped with
  `MailMessageExtensions.Validate`. The leniency is documented in the type's remarks (`7f0ba10`).
- Callers that need certainty opt in: `EmlParserOptions` (strict mode throws
  `EmlParseException`) and `ParseWithDiagnostics` (returns the message plus the problems found)
  were added in `79b7ccd` (#185).

## Consequences

- Most real files parse; callers who need certainty pay for it explicitly.
- The parser covers the subset of MIME that maps onto `MailMessage` (alternate views,
  attachments, encoded words, base64 and quoted-printable). Gaps are tracked as issues (for
  example #303: charset handling, duplicate headers, inline images) rather than closed by adding
  a dependency.
- Parser bugs are ours to fix; the mutation-testing and allocation-budget tests exist partly
  because there is no upstream parser to lean on.
- Anything that walks untrusted input (regexes, decoders) has to be defensive on its own; see
  [ADR-0004](0004-static-compiled-regexes-with-timeouts.md).
