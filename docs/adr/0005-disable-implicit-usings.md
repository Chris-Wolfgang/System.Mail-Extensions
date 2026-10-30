# ADR-0005: Implicit usings are off on every target framework

- **Status**: Accepted
- **Date**: 2026-07-06

## Context

The source project targets `net462;netstandard2.0;netstandard2.1;net8.0;net9.0;net10.0`. It once
enabled `ImplicitUsings` on the modern TFMs only. The same file then needed `using System.IO;` on
`net462` but had it flagged as redundant on `net10.0`, so ReSharper / IDE0005 reported findings
that could not be fixed for every TFM at once.

## Decision

`ImplicitUsings` stays disabled (the SDK default) for every target framework, and every file
lists its usings explicitly (`7211e4b`). The reasoning is recorded next to the setting in the
csproj.

## Consequences

- Every file compiles identically on every TFM, and redundant-using analysis is consistent.
- Files carry a few more `using` lines than a single-TFM project would.
- Turning implicit usings back on for some TFMs reintroduces the inconsistency; if it is ever
  wanted it has to be all-or-nothing.
