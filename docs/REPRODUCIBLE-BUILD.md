# Verifying a release by rebuilding it

"Our builds are reproducible" is only a claim until someone else can check it. This page is the
procedure for a third party: rebuild `Wolfgang.Extensions.Mail` from the source at a release tag,
in your own environment, and compare what you get with what was published.

Provenance (who built the package, from which commit) is a different question, answered by the
build-provenance attestation; see "Verifying a release" in [SECURITY.md](../SECURITY.md). A rebuild
answers the stronger question: do the published binaries follow from the published source?

## What is reproducible, and what is not

- The **DLLs and PDBs inside the packages** are reproducible: `Directory.Build.props` sets
  `Deterministic`, and `ContinuousIntegrationBuild` (with `CI=true`) maps source paths, so two
  builds of the same commit on the same OS produce byte-identical files.
  `.github/workflows/reproducible-build.yaml` checks that on every pull request: its
  `Reproducible build` job packs the commit twice on Ubuntu, with a full `git clean` in between,
  and fails unless every `.dll`/`.pdb` is byte-identical and every package matches entry by entry;
  its `Reproducible build (Windows report)` job packs once on Windows and reports, without failing,
  how its hashes compare with the Ubuntu ones.
- The **`.nupkg` / `.snupkg` files themselves are not**: the zip container carries timestamps and
  per-pack identifiers, and nuget.org adds its repository signature to every package it serves. Do
  not compare package hashes; compare the files inside.
- **Same OS only.** The SDK writes generated `obj/*.AssemblyInfo.cs` files with the OS newline and
  the PDB records their checksums. `Directory.Build.targets` rewrites them to LF
  (`NormalizeGeneratedLineEndings`), but cross-OS identity is only reported (by the Windows report
  job above), not promised. Releases are built on **Windows** (`release.yaml`'s
  `pack-and-validate` job runs on `windows-latest`), so rebuild on Windows.

## Expected hashes: `reproducible-build-manifest.json`

Every GitHub Release carries a `reproducible-build-manifest.json` asset, written by
`release.yaml` from the exact packages attached to that release:

```json
{
  "tag": "v0.4.0",
  "commit": "<40-character commit SHA the release was built from>",
  "packages": [
    {
      "file": "Wolfgang.Extensions.Mail.0.4.0.nupkg",
      "sha256": "<hash of the package as built, before nuget.org signed it>",
      "files": {
        "lib/net10.0/Wolfgang.Extensions.Mail.dll": "<sha256>",
        "lib/net462/Wolfgang.Extensions.Mail.dll": "<sha256>"
      }
    }
  ]
}
```

The `files` map is what you compare against: one SHA-256 per `.dll` and `.pdb` inside each
`.nupkg` and `.snupkg`. Releases published before the manifest step was added do not have this
asset; for those, compare against the files inside the package downloaded from nuget.org.

## Expected hashes: `SHA256SUMS.txt` and `BUILD-MANIFEST.txt`

Releases cut after these assets were added also carry two text files, written by `release.yaml`
in `pack-and-validate` right after packing:

| Asset | Contents |
|---|---|
| `SHA256SUMS.txt` | `<sha256>  <tfm>/<file>` for every `.dll`/`.pdb` in `src/*/bin/Release/<tfm>/` (the files that were packed), and `<sha256>  <package-file>` for every `.nupkg`/`.snupkg` as built |
| `BUILD-MANIFEST.txt` | tag, commit, workflow run URL, exact SDK version (`dotnet --version`), runner OS and image, and the `CI` / `ContinuousIntegrationBuild` / `Deterministic` values |

Use the `dotnet-sdk:` line of `BUILD-MANIFEST.txt` as the SDK to install. The package hashes in
`SHA256SUMS.txt` are of the packages before nuget.org signed them, so, as above, compare the
`.dll`/`.pdb` lines, not the package lines, with your rebuild.

Both files, like the `.nupkg` files, are subjects of the release's build-provenance attestation:

```powershell
gh attestation verify SHA256SUMS.txt --repo Chris-Wolfgang/System.Mail-Extensions
gh attestation verify BUILD-MANIFEST.txt --repo Chris-Wolfgang/System.Mail-Extensions
```

## Tooling

| Tool | Version |
|---|---|
| OS | Windows (x64) |
| .NET SDK | The SDK the release used: the `dotnet-sdk:` line of the release's `BUILD-MANIFEST.txt`. `global.json` pins the major and feature band (`rollForward: latestFeature`); install that exact version and check that `dotnet --version` prints it from inside the clone. For a release without `BUILD-MANIFEST.txt`, the version is printed in the release run's "Setup .NET" step (linked from the GitHub Release's workflow run). |
| Git | Any; the clone must have `origin` set to `https://github.com/Chris-Wolfgang/System.Mail-Extensions.git`, because SourceLink embeds the remote URL in the PDB. |
| PowerShell 7 | Only if you use the comparison snippet below. |

The older target frameworks (`net462`, `netstandard2.0`, `netstandard2.1`) are compiled by the same
SDK from reference assemblies; you do not need a .NET Framework developer pack.

## Procedure

```powershell
# 1. Source at the tag, with the canonical remote URL (SourceLink records it).
git clone https://github.com/Chris-Wolfgang/System.Mail-Extensions.git
cd System.Mail-Extensions
git checkout v0.4.0
git rev-parse HEAD        # must equal the manifest's "commit"

# 2. Build and pack exactly as release.yaml does.
$env:CI = 'true'
dotnet restore
dotnet build --no-restore --configuration Release
dotnet pack src/Wolfgang.Extensions.Mail/Wolfgang.Extensions.Mail.csproj --no-build --configuration Release --output rebuilt

# 3. Hash every .dll and .pdb inside the rebuilt packages and compare with the manifest.
$manifest = Get-Content reproducible-build-manifest.json -Raw | ConvertFrom-Json
Add-Type -AssemblyName System.IO.Compression.FileSystem
$sha = [System.Security.Cryptography.SHA256]::Create()
$mismatch = 0
foreach ($pkg in $manifest.packages) {
    $zip = [System.IO.Compression.ZipFile]::OpenRead((Join-Path rebuilt $pkg.file))
    try {
        foreach ($entry in $pkg.files.PSObject.Properties) {
            $stream = $zip.GetEntry($entry.Name).Open()
            try { $hash = -join ($sha.ComputeHash($stream) | ForEach-Object { $_.ToString('x2') }) }
            finally { $stream.Dispose() }
            if ($hash -ne $entry.Value) { $mismatch++; Write-Host "DIFFERS  $($pkg.file) $($entry.Name)" }
            else { Write-Host "matches  $($pkg.file) $($entry.Name)" }
        }
    } finally { $zip.Dispose() }
}
"$mismatch file(s) differ"
```

Download `reproducible-build-manifest.json` from the release's assets first (for example
`gh release download v0.4.0 --repo Chris-Wolfgang/System.Mail-Extensions --pattern reproducible-build-manifest.json`).

For a release that carries `SHA256SUMS.txt`, the same rebuild can also be checked against it. Step
2 leaves the assemblies in `src/*/bin/Release/<tfm>/`, keyed exactly as in the file:

```powershell
# 4. Compare the rebuilt .dll/.pdb files with SHA256SUMS.txt (download it from the release first).
$mismatch = 0
foreach ($line in Get-Content SHA256SUMS.txt) {
    $expected, $name = $line -split '  ', 2
    if ($name -notmatch '\.(dll|pdb)$') { continue }   # package lines: see "What is reproducible"
    $found = Get-ChildItem src -Recurse -File -Filter (Split-Path $name -Leaf) |
        Where-Object { $_.FullName.Replace('\', '/') -like "*/bin/Release/$name" }
    $hash = if ($found) { (Get-FileHash $found[0].FullName -Algorithm SHA256).Hash.ToLowerInvariant() }
    if ($hash -ne $expected) { $mismatch++; Write-Host "DIFFERS  $name" } else { Write-Host "matches  $name" }
}
"$mismatch file(s) differ"
```

## Reporting a discrepancy

If any file differs, open an issue at
<https://github.com/Chris-Wolfgang/System.Mail-Extensions/issues/new> titled
`Reproducible build mismatch: <tag>` with:

- the tag and the commit `git rev-parse HEAD` printed;
- your OS version, `dotnet --version`, and `dotnet --list-sdks`;
- the comparison output (which files differ);
- if you can, both copies of one differing DLL or PDB (attach them, or link to them).

A mismatch is not automatically a compromise. The usual causes are a different SDK feature band,
a different remote URL in the clone, or building on Linux. Rule those out first; the issue is still
welcome if they do not explain it. If you believe the difference is malicious, report it privately
instead, as described in [SECURITY.md](../SECURITY.md).

## Publishing a verification attestation

A successful rebuild is worth more when it is recorded where others can find it. Following the
[Reproducible Builds](https://reproducible-builds.org/docs/) convention of independent rebuilders
publishing signed statements:

1. Write a statement file (`verification-<tag>.json`) containing the tag, the commit, the manifest's
   `files` map, your environment (OS, `dotnet --version`), the date, and `"result": "match"`.
2. Sign it with a key or identity others can check, for example keyless with Sigstore:
   `cosign sign-blob --bundle verification-<tag>.sigstore.json verification-<tag>.json`
   (records the signature in the public Rekor transparency log under your OIDC identity), or with
   a GPG key published on your profile (`gpg --armor --detach-sign verification-<tag>.json`).
3. Open an issue titled `Reproducible build verified: <tag>` and attach the statement and its
   signature or bundle. Services that collect third-party rebuild attestations (such as
   vouchsafe.io) can be given the same statement.

The maintainer links verified tags from the GitHub Release notes. Verification by the maintainer's
own CI does not count as independent; the point is a rebuild outside this repository's workflows.
