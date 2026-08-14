# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.3.1] - 2026-08-14

### Fixed
- Code-scanning noise floor: drove open InspectCode findings from 400 to 0 through a mix of real fixes (redundant usings deleted, redundant casts and qualifiers removed, `Cast<object>()` for `string.Format` args in `InlineHtmlBuilder`, `_textBody ?? string.Empty` in `MailMessageBuilder`, dropped defensive `?? new List<>()` on a non-nullable constructor parameter in `ValidationResult`, `using`-statement resource-initialization splits in the integration tests, conditional-compilation guards on the `using System;` / `System.Diagnostics.CodeAnalysis` directives in the `MailAddressExtensions` / `EmlParser` multi-TFM paths) plus narrowly-scoped analyzer opt-outs for a handful of true false positives (`RS0016` for the `extension(MailAddress)` C# 14 members where InspectCode's bundled `PublicApiAnalyzers` version doesn't understand the extension-syntax entries in `PublicAPI.Shipped.txt`; `S8969` / `RedundantSuppressNullableWarningExpression` on `Assert.NotNull` follow-up sites where the `!` is required on net462 / netstandard2.0 but redundant on modern TFMs). `RS0016` / `RS0037` are silenced at test-project and benchmark-project scope in the respective `.editorconfig` files (the `PublicApiAnalyzers` premise — tracking a library's public API — doesn't apply to test or benchmark projects).
- zizmor findings: driven from 5 to 0. Four `template-injection` alerts in `codeql.yaml` fixed by moving step-outcome inlines into an `env:` block (the values reach the pwsh script through the data channel, not template expansion). One `superfluous-actions` alert on `release.yaml` documented as an accepted design choice in `.github/zizmor.yml` — `softprops/action-gh-release` is the fleet-canonical glob-matched multi-file uploader.
- Line-ending drift: `docfx_project/docfx.json` was committed to git with CRLF while `.gitattributes` declares `* text=auto eol=lf`, leaving the file permanently dirty on Windows worktrees. Renormalized to LF via `git add --renormalize`.

## [0.3.0] - 2026-07-07

### Changed
- `AttachmentCollection.TotalSize()` (and therefore `ExceedsLimit()`) is now allocation-free — an index-based loop replaces the LINQ `Where`/`Sum`, removing the iterator, delegate, and boxed-enumerator allocations.

### Added
- Trim / Native-AOT compatibility: the library is marked `IsAotCompatible` and its one reflection-based API, `ToMimeString`, is annotated `[RequiresUnreferencedCode]` / `[RequiresDynamicCode]` so trimmed / `PublishAot` consumers get a compile-time warning instead of a publish-time failure. A Native AOT smoke consumer runs in CI to verify the rest of the surface survives trimming and AOT.
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

[Unreleased]: https://github.com/Chris-Wolfgang/System.Mail-Extensions/compare/v0.3.1...HEAD
[0.3.1]: https://github.com/Chris-Wolfgang/System.Mail-Extensions/compare/v0.3.0...v0.3.1
[0.3.0]: https://github.com/Chris-Wolfgang/System.Mail-Extensions/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/Chris-Wolfgang/System.Mail-Extensions/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/Chris-Wolfgang/System.Mail-Extensions/releases/tag/v0.1.0
