# ADR-0004: Parser regexes are static, compiled, and carry a one-second timeout

- **Status**: Accepted
- **Date**: 2026-04-22

## Context

`EmlParser` uses regular expressions for header parameters (`boundary=`, `filename=`, `name=`),
encoded words and address forms, and it runs them on untrusted input. Two problems followed from
calling the static `Regex.Match` / `Regex.Replace` helpers:

- No match timeout, so a crafted header could make a backtracking pattern run for a very long
  time (ReDoS). The analyzers flagged it; the timeout was added in `e3c7856` ("Fix analyzer errors
  in EmlParser").
- The static helpers cache only a limited number of interpreted patterns, and a single message
  hits several patterns once per header line or MIME part.

## Decision

Every pattern is a `private static readonly Regex` field constructed once with
`RegexOptions.Compiled`, `RegexOptions.ExplicitCapture` (plus `IgnoreCase` where the grammar is
case-insensitive), and the explicit `RegexTimeout` of one second.

## Consequences

- A pathological header fails with `RegexMatchTimeoutException` after at most one second per match
  instead of hanging the caller.
- Compiling the patterns costs some start-up time on first use of `EmlParser`, repaid quickly on
  multi-part messages.
- New patterns must follow the same shape; a static `Regex.Match(input, pattern)` call in `src/`
  is a review finding.
- A source-generated `[GeneratedRegex]` would be faster on `net7.0`+ but is unavailable on
  `net462` / `netstandard2.0`; adopting it means per-TFM code and is not done today.
