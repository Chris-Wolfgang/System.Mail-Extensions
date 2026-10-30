# Mutation testing

[Stryker.NET](https://stryker-mutator.io/docs/stryker-net/introduction/) changes the library's code
one small edit at a time (a flipped condition, a removed statement, a changed constant) and runs the
unit tests against each change. A change the tests do not notice is a *survivor*: a line that is
covered but not really tested. The *mutation score* is the share of mutants the tests kill.

## Floor

| Project | Floor (`thresholds.break`) | Measured | Where |
|---|---|---|---|
| `src/Wolfgang.Extensions.Mail` | **92 %** | 92.01 % (829 killed, 67 survived, 35 compile errors excluded), Stryker 5.0.0, `net10.0`, 2026-10-30 | [`stryker-config.json`](../stryker-config.json) |

The floor starts at the current score and **only ratchets up**. When a pull request raises the
score by a whole point or more, raise `break` (and `low`, which Stryker requires to be at least
`break`) to the new whole-number score in the same pull request. Lowering the floor needs the
reason in the PR body; "a refactor moved code around" is not one, because the same tests should
still kill the same mutants.

`thresholds.high` (95 %) only colours the HTML report.

## Where it runs

[`.github/workflows/stryker.yaml`](../.github/workflows/stryker.yaml):

- **Pull requests to `main`** that touch `src/**`, `tests/**/*.cs`, `stryker-config.json` or the
  workflow: Stryker runs with the configuration above and **fails the check when the score is below
  `break`**. That is the release-gate quality bar: nothing below the floor merges.
- **Weekly (Sunday 06:00 UTC) and on demand**: the same run, plus
  - the score history is appended to the `gh-pages` branch under `dev/stryker` and charted by
    `benchmark-action/github-action-benchmark`, the same action as the benchmark charts;
  - one rolling issue labelled `kind:mutation-survives` lists the current survivors (deliberately one
    issue, not one per survivor).

The workflow runs on Windows when any project still targets .NET Framework, so the initial build
succeeds; the mutation run itself uses the `net10.0` target from `stryker-config.json`.

## Running it locally

```bash
dotnet tool restore
dotnet restore
dotnet stryker --config-file stryker-config.json
```

The run takes about four minutes on a desktop machine and writes the HTML and JSON reports under
`StrykerOutput/` (ignored by git). Exit code 0 means the score is at or above the floor.

## Survivors

Not every survivor is worth killing. Equivalent mutants (a change that cannot alter behaviour, for
example a mutated string in an exception message nobody asserts on, or a boundary that cannot be
reached) stay alive by definition. When you decide a survivor is equivalent, say why in the PR that
leaves it; a test added only to pin a meaningless detail makes the suite harder to change without
making it better.
