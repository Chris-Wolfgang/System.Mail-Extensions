# License audit

Dependabot tracks dependency *versions* and advisories; it does not look at *licenses*. This
repository audits the license of every package in the source project's transitive NuGet closure,
so an incompatible license (GPL, AGPL, SSPL, a business-source license, or an unknown one) cannot
reach the MIT-licensed package unnoticed.

## How it runs

| Piece | Where | What it does |
|---|---|---|
| Gate | [`.github/workflows/license-audit.yaml`](../.github/workflows/license-audit.yaml) | On every pull request, every push to `main`, weekly (Tuesdays 06:15 UTC) and on demand: restores each `src/` project and runs `nuget-license` (pinned in `.config/dotnet-tools.json`) over the full transitive closure. Any license outside the allow-list fails the job. |
| Policy files | [`.github/license-audit/`](../.github/license-audit/) | `allowed-licenses.json` (the allow-list), `ignored-packages.json`, `package-overrides.json`, `url-license-mappings.json`. On a pull request the workflow uses the copies from `main`, never the PR's own, and the protected-file guard (`protected-files.yaml`) covers them, so a PR cannot loosen the policy it is audited with. |
| Attribution | [`scripts/third-party-notices.ps1`](../scripts/third-party-notices.ps1), run by `release.yaml` before pack | Renders `THIRD-PARTY-NOTICES.md` from the same closure and policy; `Directory.Build.props` packs it at the package root, so every release ships the attribution for what it was built with. `license-audit.yaml` uploads the same file as an artifact on each run so reviewers can see what a dependency change does to it. |

## Allow-list

`allowed-licenses.json` accepts these SPDX identifiers, all permissive and compatible with
redistribution under MIT:

`MIT`, `Apache-2.0`, `BSD-2-Clause`, `BSD-3-Clause`, `MS-PL`, `0BSD`, `ISC`, `BSL-1.0`, `MPL-2.0`,
`MS-.NET-Library`.

`MPL-2.0` is file-level copyleft: using an MPL package unmodified is fine, but modifying its files
means publishing those modifications. `MS-.NET-Library` is the license of older Microsoft
`System.*` packages (here `Microsoft.NETCore.Platforms` 1.1.0, pulled in by `netstandard2.0`).

## Adding a package outside the allow-list

Anything else needs an explicit decision recorded in the pull request that introduces it:

1. The PR body names the package, its license, why it is needed, and why the license is
   acceptable for an MIT package (or that it is build-time only and never distributed).
2. The policy change goes in **its own pull request** (the policy files are protected and are read
   from `main`): add the package to `package-overrides.json` with a `_comment` stating the
   verified license and a pinned `Version`, or to `ignored-packages.json` if it is build-time
   only. Pinning the version means a license change on a later bump is checked again.
3. The maintainer's approval of that PR is the recorded decision.

Today's exceptions: `SonarAnalyzer.CSharp` is ignored (an analyzer: `PrivateAssets="all"`, never
distributed with the package). `package-overrides.json` carries a `SQLitePCLRaw.lib.e_sqlite3`
entry inherited from the repository template; nothing in this repository depends on it, so it is
inert.

## Baseline (main @ 0b54993, 2026-10-30)

**The published package has no runtime dependencies.** Every `PackageReference` of
`src/Wolfgang.Extensions.Mail` is `PrivateAssets="all"` (analyzers and SourceLink), and the nuspec
of the published `0.3.1` lists an empty dependency group for all six target frameworks.

The audit nevertheless walks the whole build-time closure. `dotnet nuget-license -t` (4.0.18) with
the policy files above, run locally on `main` @ `0b54993`, exit 0:

| Package | Version | License | Why it is in the closure |
|---|---|---|---|
| AsyncFixer | 2.1.0 | Apache-2.0 | analyzer |
| Meziantou.Analyzer | 3.0.303 | MIT | analyzer |
| Microsoft.Build.Tasks.Git | 10.0.402 | MIT | SourceLink |
| Microsoft.CodeAnalysis.BannedApiAnalyzers | 5.6.0 | MIT | analyzer |
| Microsoft.CodeAnalysis.PublicApiAnalyzers | 5.6.0 | MIT | analyzer |
| Microsoft.NET.ILLink.Tasks | 8.0.30, 9.0.19, 10.0.11 | MIT | trim/AOT analyzers on `net8.0`+ (`IsAotCompatible`) |
| Microsoft.NETCore.Platforms | 1.1.0 | MS-.NET-Library | `netstandard2.0` |
| Microsoft.SourceLink.Common | 10.0.402 | MIT | SourceLink |
| Microsoft.SourceLink.GitHub | 10.0.402 | MIT | SourceLink |
| Microsoft.VisualStudio.Threading.Analyzers | 18.12.1 | MIT | analyzer |
| NETStandard.Library | 2.0.3 | MIT | `netstandard2.0` |
| Roslynator.Analyzers | 5.0.1 | Apache-2.0 | analyzer |
| System.Buffers | 4.6.1 | MIT | build-time closure |
| System.IO.Hashing | 10.0.13 | MIT | build-time closure |
| System.Memory | 4.6.3 | MIT | build-time closure |
| System.Numerics.Vectors | 4.6.1 | MIT | build-time closure |
| System.Runtime.CompilerServices.Unsafe | 6.1.2 | MIT | build-time closure |

All licenses are on the allow-list. `THIRD-PARTY-NOTICES.md` for a release lists the subset that is
not a `PrivateAssets="all"` reference or a SourceLink task package (seven packages at this
baseline), erring on the side of extra attribution.

Re-run the baseline after a dependency change with the command in `license-audit.yaml`'s
"Audit licenses" step, and update this table when a package with a new license enters the closure.
