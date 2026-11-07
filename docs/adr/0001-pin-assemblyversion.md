# ADR-0001: Pin `AssemblyVersion` at `1.0.0.0`

- **Status**: Accepted
- **Date**: 2026-05-28

## Context

`AssemblyVersion` is part of an assembly's identity. If it changes on every release, a .NET
Framework consumer whose dependency graph contains two builds of this library, or a strong-named
caller compiled against an older one, needs a binding redirect for a change that broke nothing.
This library ships `net462` and `netstandard2.0`, so it is exposed to that behaviour in a way a
`net8.0`-only package is not.

The pin was dropped once during a template sync and restored in `b561d4e` ("C4 fix"), which is why
the decision is written down rather than left to a csproj comment.

## Decision

`<AssemblyVersion>` is pinned at `1.0.0.0` in `src/Wolfgang.Extensions.Mail/Wolfgang.Extensions.Mail.csproj`
and changes only on a deliberate breaking API change. `FileVersion` (derived from `<Version>` with
any pre-release suffix stripped) and `InformationalVersion` (from `<Version>` plus the SourceLink
commit) carry the real release version.

## Consequences

- Upgrading within the current major needs no binding redirect.
- The package is still `0.x` while the assembly says `1.0.0.0`. That is intentional: the pin is a
  binding-stability baseline, not a statement of API maturity. When the package reaches `1.0.0`
  the assembly version stays `1.0.0.0`; it moves to `2.0.0.0` with the first breaking major after
  that.
- Tooling that reports `AssemblyVersion` shows `1.0.0.0` for every release. Identify the exact
  build from `InformationalVersion` or `FileVersion`.
- The assembly is not strong-named today (`SignAssembly` is `False`), so the .NET Framework
  loader does not enforce the version for most consumers. The pin costs nothing and keeps the
  option of strong naming open without a version-churn problem.
- Changing it is a release-gating decision. Package Validation
  ([ADR-0006](0006-package-validation-baseline.md)) is what makes an accidental break visible
  before that question arises.
