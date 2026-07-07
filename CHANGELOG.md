# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- `EmlParserOptions` with a `Strict` mode: `EmlParser.Parse`/`ParseFile`/`ParseFileAsync` overloads that throw `EmlParseException` on the first malformed construct (address, undecodable transfer encoding, or malformed RFC 2047 encoded word) instead of skipping it.
- `EmlParser.ParseWithDiagnostics`, returning a `ParseResult` that carries the best-effort message plus the list of skipped constructs as `ValidationIssue`s.

## [0.2.0] - 2026-07-06

### Added
- BenchmarkDotNet baseline project covering the EML parser, MIME serialization, and attachment factory, with results published to an interactive chart on the docs site.
- Integration test suite covering `ToMimeString` → `EmlParser.Parse` round trips, on-disk `.eml` file I/O, and `MailMessageBuilder` / `InlineHtmlBuilder` composition.
- NuGet package search tags and a package icon.

### Changed
- Quoted-printable decoding rewritten as a single pass, cutting allocations by roughly 7.7x on large bodies (a 100 KB body drops from ~7.4 MB to ~1 MB allocated). Parser regular expressions are now static compiled instances.
- `ToMimeString` serializes with a single buffer copy instead of two.
- `MailMessage.Clone` copies `ContentType` by property rather than round-tripping it through a string.
- `MailMessageBuilder.Build` now reports every missing required field (From address and recipients) in a single exception message instead of surfacing them one rebuild at a time.
- README rewritten to document the full public API surface; repository contributor and AI-assistant guidance corrected to describe this library.

### Removed
- Removed an example project that had been carried over unchanged from another library, along with unused `using` directives and dead helpers in the test projects.

### Fixed
- `ToMimeString` → `EmlParser.Parse` round trips are now idempotent. Previously each cycle appended a trailing blank line to the body; the parser now trims the single wire-format line break that terminates the final body line. **Behavior change:** a body parsed back from `ToMimeString` output no longer carries an extra trailing CRLF.
- Restored an explicit, pinned `AssemblyVersion` so .NET Framework consumers do not need new binding redirects on every patch release (`FileVersion` and the informational version continue to carry the release version).
- Documented `EmlParser`'s lenient parsing contract: malformed addresses are skipped and a malformed `From` header leaves `MailMessage.From` null — pair with `Validate()` to detect what a lenient parse dropped.

## [0.1.0] - 2026-05-02

### Added
- Initial release: `MailMessageBuilder`, `EmlParser` (parse and serialize EML/MIME), `MailMessageExtensions` (`Validate`, `Clone`, `ToMimeString`), `AttachmentFactory`, `InlineHtmlBuilder`, `MailAddress.TryParse`, and attachment / address collection helpers.

[Unreleased]: https://github.com/Chris-Wolfgang/System.Mail-Extensions/compare/v0.2.0...HEAD
[0.2.0]: https://github.com/Chris-Wolfgang/System.Mail-Extensions/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/Chris-Wolfgang/System.Mail-Extensions/releases/tag/v0.1.0
