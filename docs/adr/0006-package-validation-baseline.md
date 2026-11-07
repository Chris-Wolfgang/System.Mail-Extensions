# ADR-0006: Gate every pack on binary compatibility with the last published version

- **Status**: Accepted
- **Date**: 2026-07-07

## Context

The library ships six target frameworks and is still `0.x`, so it is easy to remove or change a
public member, drop a TFM, or change a default value without noticing. `PublicAPI.*.txt` (the
Public API analyzers) catches source-level surface changes, but not every binary break and not a
dropped TFM.

## Decision

`EnablePackageValidation` is on, with `PackageValidationBaselineVersion` set to the last released
version (`0.3.1` today) (`3baba4f`, #134). During `dotnet pack` the SDK downloads that package and
validates every TFM's assembly against it. Intentional breaks in a major release are recorded in
`CompatibilitySuppressions.xml` (regenerate with `-p:GenerateCompatibilitySuppressionFile=true`).

## Consequences

- An accidental break fails the pack, locally and in `release.yaml`, before anything is published.
- The baseline has to be bumped after each release, once the new version is on nuget.org; a pack
  run before the CDN has indexed it fails to download the baseline.
- API changes are expressed as additions plus `[Obsolete]` where possible rather than removals.
- Together with [ADR-0001](0001-pin-assemblyversion.md) this makes "nothing broke" a checked claim
  rather than a hope.
