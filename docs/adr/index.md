# Architecture Decision Records

Short records of the choices in this repository that are not obvious from the code, and would
otherwise be re-derived - usually worse - six months later. The format is Michael Nygard's
(context, decision, consequences); start from [TEMPLATE.md](TEMPLATE.md).

## Process

- A new ADR lands **in the same pull request** as the decision it describes, so the reasoning is
  reviewed alongside the change rather than reconstructed afterwards.
- Number ADRs sequentially (`NNNN-short-title.md`) and add a row to the table below in that PR.
- A superseded ADR is kept, not deleted: set its Status to `Superseded by ADR-NNNN` and link the
  replacement, so the history of a reversal stays legible. Status values: Proposed, Accepted,
  Superseded, Deprecated.
- ADR-0001 to ADR-0006 were written retroactively; their dates are the dates the decision first
  landed on `main`.

| ADR | Decision | Status | Date |
|---|---|---|---|
| [ADR-0001](0001-pin-assemblyversion.md) | Pin `AssemblyVersion` at `1.0.0.0` | Accepted | 2026-05-28 |
| [ADR-0002](0002-tomimestring-reuses-the-internal-mailwriter.md) | `ToMimeString` reuses System.Net.Mail's internal `MailWriter` through reflection | Accepted | 2026-04-15 |
| [ADR-0003](0003-lenient-dependency-free-eml-parser.md) | `EmlParser` is a lenient, dependency-free parser with opt-in strict mode | Accepted | 2026-07-06 |
| [ADR-0004](0004-static-compiled-regexes-with-timeouts.md) | Parser regexes are static, compiled, and carry a one-second timeout | Accepted | 2026-04-22 |
| [ADR-0005](0005-disable-implicit-usings.md) | Implicit usings are off on every target framework | Accepted | 2026-07-06 |
| [ADR-0006](0006-package-validation-baseline.md) | Gate every pack on binary compatibility with the last published version | Accepted | 2026-07-07 |
