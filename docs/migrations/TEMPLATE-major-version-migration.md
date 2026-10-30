# Wolfgang.Extensions.Mail v<NEW-MAJOR> migration guide

> **Upgrading from v<PREV-MAJOR>.x?** This is the place. Between minor or patch versions within
> v<NEW-MAJOR>, see [CHANGELOG.md](../../CHANGELOG.md) instead: MINOR and PATCH releases carry no
> breaking changes.

- **Release**: `v<NEW-MAJOR>.0.0` (YYYY-MM-DD)
- **Previous major**: `v<PREV-MAJOR>.x`
- **Release notes**: [link to the GitHub Release]

## Breaking-change inventory

One row per breaking change. If it is not in this table it is not breaking, and consumers who do
not hit any row can upgrade without code changes. Every entry in `CompatibilitySuppressions.xml`
for this release must appear here.

| # | Kind | Before (v<PREV-MAJOR>) | After (v<NEW-MAJOR>) | Fix path |
|---|---|---|---|---|
| 1 | Removed API | `MailMessageBuilder.Foo(...)` | `MailMessageBuilder.Bar(...)` | Section 1 |
| 2 | Renamed API | `EmlParser.OldName(...)` | `EmlParser.NewName(...)` | Section 2 |
| 3 | Signature change | `X.Baz(string s)` | `X.Baz(MailAddress a)` | Section 3 |
| 4 | Behaviour change | `X.Qux()` returned `null` on bad input | `X.Qux()` throws `EmlParseException` | Section 4 |
| 5 | Target framework removed | shipped `net462` | minimum is `netstandard2.0` | Section 5 |

Delete rows that do not apply. Do not merge several breaks into one row.

## Per-change migration

### 1. `MailMessageBuilder.Foo(...)` removed - use `MailMessageBuilder.Bar(...)`

**Why**: [one paragraph; link the ADR if one records the decision]

**Before**:

```csharp
var message = new MailMessageBuilder()
    .Foo("a@example.com")
    .Build();
```

**After**:

```csharp
var message = new MailMessageBuilder()
    .Bar(new MailAddress("a@example.com"))
    .Build();
```

**Notes**: [subtleties, for example defaults that preserve the old behaviour]

### 2. ...

## Deprecation timeline (looking back)

APIs this major removes that were `[Obsolete]` earlier, so consumers who skipped versions see the
warnings they missed.

| Version | What happened |
|---|---|
| v<PREV-MAJOR>.Y | `Foo` marked `[Obsolete("Use Bar instead")]` (warning) |
| v<NEW-MAJOR>.0 | Removed |

If this major breaks something with **no** prior deprecation, say so here and explain why.

## Deprecation timeline (looking forward)

APIs this major marks `[Obsolete]` but keeps, so consumers know what the next major removes.

| API | Status in v<NEW-MAJOR> | Planned removal |
|---|---|---|
| `X.Baz(string)` | `[Obsolete(...)]` warning | v<NEW-MAJOR + 1>.0 |

## Compatibility notes

- **Target frameworks**: which TFMs v<NEW-MAJOR> ships versus v<PREV-MAJOR>; call out drops and
  additions.
- **Package identity**: `Wolfgang.Extensions.Mail`, unchanged unless the package was renamed.
- **AssemblyVersion**: state whether it moved (see
  [ADR-0001](../adr/0001-pin-assemblyversion.md)). If it did, .NET Framework consumers with
  strong-named callers need a one-time binding redirect at this boundary.
- **Trim / Native AOT**: state any change to the APIs annotated `[RequiresUnreferencedCode]` /
  `[RequiresDynamicCode]` (today only `ToMimeString`; see
  [ADR-0002](../adr/0002-tomimestring-reuses-the-internal-mailwriter.md)).
- **Public API diff**: the removed lines of `src/Wolfgang.Extensions.Mail/PublicAPI.Shipped.txt`
  between the two tags (`git diff v<PREV>..v<NEW> -- src/Wolfgang.Extensions.Mail/PublicAPI.Shipped.txt`).

## FAQ

**I am on v<PREV-MAJOR>.x and hit none of the rows above. Do I need to do anything?**

Update the package reference and run your tests. If nothing breaks, you are done.

**Can I stay on v<PREV-MAJOR>?**

Security fixes are released for the latest published version only (see
[SECURITY.md](../../SECURITY.md)), so plan the upgrade rather than pinning the old major.

**How do I report a migration problem this guide does not cover?**

Open an issue at https://github.com/Chris-Wolfgang/System.Mail-Extensions/issues/new with enough
code to reproduce it, and say which version you are upgrading from.
