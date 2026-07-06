# Copilot Coding Agent Instructions

## Repository Summary

This repository ships **Wolfgang.Extensions.Mail**, a NuGet library of extension methods and helpers for `System.Net.Mail`: a fluent `MailMessageBuilder`, an EML/MIME parser (`EmlParser`) and serializer (`ToMimeString`), message validation and deep cloning, `AttachmentFactory` with content-type inference, `InlineHtmlBuilder` for inline images, and collection conveniences.

**Repository Type**: Shipping .NET library (single package: `Wolfgang.Extensions.Mail`)
**Target Frameworks (src)**: net462; netstandard2.0; netstandard2.1; net8.0; net9.0; net10.0
**Test Frameworks**: unit tests span net462–net481 and net5.0–net10.0; integration tests target net8.0–net10.0
**Primary Language**: C# (`LangVersion` 14 — the codebase uses C# 14 extension members in `MailAddressExtensions`)

## Layout

- `src/Wolfgang.Extensions.Mail/` — the library (public API tracked in `PublicAPI.Shipped.txt`)
- `tests/Wolfgang.Extensions.Mail.Tests.Unit/` — unit suite
- `tests/Wolfgang.Extensions.Mail.Tests.Integration/` — end-to-end round-trip and file I/O suite (net8.0+)
- `benchmarks/Wolfgang.Extensions.Mail.Benchmarks/` — BenchmarkDotNet suites (parser, serialization, attachment factory); results chart to gh-pages `/dev/bench/`
- `docfx_project/` — API docs published to gh-pages

## Build and Validation

### Prerequisites
- .NET SDK 10.0+ (CI tests down-level TFMs; local net10.0 runs are usually sufficient for iteration)

### Commands
```powershell
dotnet restore
dotnet build --no-restore --configuration Release   # TreatWarningsAsErrors in Release
dotnet test --no-build --configuration Release      # or -f net10.0 for a quick pass
```

### Conventions that will fail your build if ignored
- **Release builds treat warnings as errors** (set in `Directory.Build.props`).
- **Sync I/O and blocking APIs are banned** (`BannedSymbols.txt` / RS0030): no `File.ReadAllText`, `Stream.Read/Write/CopyTo/Flush`, `Task.Wait`, `.Result`. Use async equivalents. Tests and benchmarks are exempt via `.editorconfig` scopes.
- **`PublicAPI.Shipped.txt` must match the public surface.** RS0017 (entry not found) breaks Release builds. The entries use the analyzer's dialect: `!` on non-null reference types, enum members as `Name = value -> Type`, `default(System.Threading.CancellationToken)` spelled out, and C# 14 extension members as `Type.extension(Receiver!).Member(...)`. To regenerate: `dotnet format analyzers src/Wolfgang.Extensions.Mail/Wolfgang.Extensions.Mail.csproj --diagnostics RS0016 --severity info` (severity `info` is required — a global `.editorconfig` rule downgrades analyzer diagnostics at `.cs` locations to suggestion).
- **`TargetFrameworks` stays on a single line** in every csproj — fleet-wide convention. This repo's `pr.yaml` tolerates multi-line values (it evaluates the property via `dotnet msbuild -getProperty`), but other fleet tooling greps for the single-line form, so keep it.
- **No absolute paths** in project files.
- Code style: Allman braces, file-scoped namespaces, 4-space indent, three blank lines between members, multi-line argument lists with the closing paren on its own line.
- Test naming: `MethodUnderTest_when_condition_expected_result`.

### Behavioral contracts to preserve
- `EmlParser` is **deliberately lenient**: malformed addresses are skipped, a malformed From leaves `MailMessage.From` null. Don't make it throw on bad input; pair with `Validate()` instead.
- `ToMimeString()` → `EmlParser.Parse()` round trips are **idempotent** (the parser trims the single wire-format CRLF that terminates the final body line). The integration suite asserts exact body equality.
- `AttachmentFactory`'s content-type registry is process-wide, thread-safe, last-write-wins. Tests must register **unique** extensions (see `UniqueExtension()` in `AttachmentFactoryTests`).

## CI

- `pr.yaml` — staged validation: Linux tests + 90% coverage gate, then Windows (.NET Framework matrix), then macOS; plus CodeQL, DevSkim, and gitleaks scans. Test projects are auto-discovered under `tests/`.
- `release.yaml` — fires on GitHub Release *published*; packs and pushes to NuGet, deploys docs.
- `benchmarks.yaml` — on push to main touching `src/`/`benchmarks/`; appends a data point to the gh-pages chart.
- `stryker.yaml` — mutation testing.
- Protected files (workflows, `Directory.Build.props`, `BannedSymbols.txt`, `.editorconfig`, `.globalconfig`) are guarded by the "Detect .NET Projects" check on PRs to `main`.
