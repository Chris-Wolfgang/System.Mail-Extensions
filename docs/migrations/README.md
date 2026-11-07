# Migration guides

Per-major-version upgrade paths for consumers of `Wolfgang.Extensions.Mail`.

No major version has shipped yet (the package is `0.x`). This folder sets the convention now so
the first breaking major ships with a guide instead of having one retrofitted afterwards.

## When a migration guide is required

- **Every MAJOR release**: `0.x -> 1.0` when the `1.0` cut carries a breaking change, then
  `1.x -> 2.0`, and so on.
- **A MINOR that deprecates a large surface** through `[Obsolete]` without removing anything, so
  consumers can plan for the next major. A single obsoleted overload does not need one; the
  `[Obsolete]` message and the changelog are enough.

Package Validation ([ADR-0006](../adr/0006-package-validation-baseline.md)) fails the pack on an
unintended break. An intentional break is recorded in `CompatibilitySuppressions.xml`, and every
entry there must have a row in the guide's breaking-change inventory.

## When to write it

**During release prep, in the same pull request as the MAJOR version bump**, not after. If the
guide is not ready, the release is not ready. The guide's base branch is the one that carries the
bump (`main`, or the release-cycle integration branch when one is in use).

## File naming

- `TEMPLATE-major-version-migration.md`: the template. Copy it; never edit it during a release.
- `v<MAJOR>.md`: the guide, named by the destination major. `v2.md` describes `1.x -> 2.0`.

## Linking from the release notes

The guide MUST be linked from the GitHub Release notes of the MAJOR version, as the first line:

```markdown
**Upgrading from <PREV-MAJOR>.x?** See [the v<MAJOR> migration guide](https://github.com/Chris-Wolfgang/System.Mail-Extensions/blob/main/docs/migrations/v<MAJOR>.md).
```

Use the absolute URL: release notes are rendered outside the repository, where a relative link
does not resolve.

## Related

- [`docs/adr/`](../adr/index.md): Architecture Decision Records explain *why* the current design is
  what it is. A migration guide explains the *change* consumers experience across a major
  boundary. When a major reverses a decision recorded in an ADR, the superseding ADR and the
  guide link to each other.
- `CHANGELOG.md`: the per-release list of changes, assembled from `changelog/unreleased/`
  fragments. The guide is the narrative for breaking changes only.
