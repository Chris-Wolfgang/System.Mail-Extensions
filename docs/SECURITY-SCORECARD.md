# OSSF Scorecard: baseline and score floor

[OSSF Scorecard](https://github.com/ossf/scorecard) scores this repository's security posture
(branch protection, pinned dependencies, token permissions, dangerous-workflow patterns, and so on)
on a 0-10 scale. It scores the *repository configuration and release engineering*, not the code;
CodeQL, Semgrep, DevSkim and gitleaks cover the code.

- Workflow: [`.github/workflows/scorecard.yaml`](../.github/workflows/scorecard.yaml) - weekly
  (Mondays 06:31 UTC), on every push to `main`, on branch-protection changes, and on demand.
- Findings: uploaded as SARIF to
  [Security -> Code scanning](https://github.com/Chris-Wolfgang/System.Mail-Extensions/security/code-scanning?query=tool%3AScorecard),
  next to the CodeQL alerts.
- Public score and badge: the README's "OpenSSF Scorecard" badge, backed by
  <https://scorecard.dev/viewer/?uri=github.com/Chris-Wolfgang/System.Mail-Extensions>.

## Score floor

**Target: an aggregate score of at least 7.5 on `main`.**

A pull request that drops the score below 7.5 (or that is expected to, for example by adding an
unpinned dependency or broadening a workflow's token permissions) must:

1. Say so in the PR body: which check drops, by how much, and why the change is still wanted.
2. Add a changelog fragment (`changelog/unreleased/<name>.md`, `type: internal`) recording the
   accepted lower score, for example:

   ```
   type: internal

   OSSF Scorecard: Pinned-Dependencies dropped from 10 to 9 because <reason>; accepted until <condition>.
   ```

   `CHANGELOG.md` is assembled from fragments at release time, so the note reaches the release
   notes without anyone editing `CHANGELOG.md` by hand.

This is a soft gate (reviewer attention), not a required check. Scorecard is strict enough that a
single stale pin or a transient false positive can drop the score while the fix is in flight, and
a hard gate would block unrelated work. A score below **6.0** is treated as an incident: open an
issue and fix it before the next release.

## Baseline

The first scans ran before the results could be published: `publish_results: true` was rejected by
scorecard.dev because the analysis job also contained a `run:` step (#297). So there is no public
aggregate score yet (`api.scorecard.dev` answers 404 for this repository), and the baseline below is
taken from the per-check SARIF that the workflow uploaded to code scanning.

| | |
|---|---|
| Scan | `main` @ `0b54993`, 2026-10-16 (Scorecard analyses 1967076717, 1967076814, 1967076915) |
| Checks evaluated | 18: Binary-Artifacts, Branch-Protection, CI-Tests, CII-Best-Practices, Code-Review, Contributors, Dangerous-Workflow, Dependency-Update-Tool, Fuzzing, License, Maintained, Packaging, Pinned-Dependencies, SAST, Security-Policy, Signed-Releases, Token-Permissions, Vulnerabilities |
| Findings reported | 2: **Branch-Protection 4/10**, **Fuzzing 0/10** |
| Findings suppressed by the workflow | Code-Review and CII-Best-Practices (always), and Pinned-Dependencies findings whose snippet is a `dotnet restore` / `nuget restore` command; see the comment block in `scorecard.yaml` for why |
| Aggregate score | Not yet published. Fill it in from the first run after the publishing fix (#319) reaches `main`, and record the date. |

## Known-acceptable findings

Every entry names the check, the finding, and why it is accepted rather than fixed. Review yearly.

| Check | Finding | Rationale | Reviewed |
|---|---|---|---|
| Branch-Protection (4/10) | `main` does not require approvers, does not require code-owner review, and has "last push approval" off | Single-maintainer repository: a required approver would be the author. The `main` ruleset still requires a pull request, the status checks and linear history, and blocks force-pushes and deletion. Tracked as #255 (labelled `blocked`). | 2026-10-30 |
| Fuzzing (0/10) | No fuzzer integration Scorecard recognises (OSS-Fuzz, ClusterFuzzLite, Go native fuzzing) | Scorecard has no .NET fuzzing detector that a library like this can satisfy locally; accepted as unfixable (fleet decision of 2026-08-22). Tracked as #253 (labelled `blocked`). | 2026-10-30 |
| Code-Review | Suppressed in `scorecard.yaml` | Same single-maintainer reason as Branch-Protection; the finding would never clear. | 2026-10-30 |
| CII-Best-Practices | Suppressed in `scorecard.yaml` | The OpenSSF Best Practices badge is an external self-certification, not something a pull request can fix. | 2026-10-30 |

## Relationship to other security work

- **CodeQL** (`codeql.yaml`) and **Semgrep** (`semgrep.yaml`) scan the code for vulnerabilities.
- **DevSkim** and **gitleaks** (in `pr.yaml`) scan source patterns and committed secrets.
- **InspectCode** (in `pr.yaml`) reports code-quality findings to code scanning.
- **OSSF Scorecard** (this document) scores the repository configuration and release practices.

None of them subsumes the others.
