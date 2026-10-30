#!/usr/bin/env python3
"""Compare a shadow-workload run of this commit against a run of the baseline release.

Both runs come from the same job on the same runner (shadow.yaml), so the comparison is
like-for-like: the only difference is the library build under test.

Usage:
    python3 compare.py <current-results-dir> <baseline-results-dir> [--report report.md]

Each directory is a BenchmarkDotNet ``BenchmarkDotNet.Artifacts/results`` folder written with
``--exporters json`` (one ``*-report-full-compressed.json`` per benchmark class).

Thresholds (relative regression of current vs baseline) are configurable through environment
variables, so a workflow input or a repository variable can tune them without a code change:

    SHADOW_LATENCY_TOLERANCE      default 0.20  (median time per operation)
    SHADOW_ALLOCATION_TOLERANCE   default 0.50  (bytes allocated per operation)
    SHADOW_GEN2_TOLERANCE         default 0.50  (gen-2 collections per 1,000 operations)

Exit codes: 0 every scenario within tolerance; 1 at least one regression; 2 the inputs are
unusable (no results, or a scenario missing from one side), which is a broken run, not a finding.
A markdown table is printed and, with ``--report``, also written to a file for the issue body.
"""

from __future__ import annotations

import argparse
import glob
import json
import os
import sys

METRICS = (
    # key, label, environment variable, default tolerance
    ("median_ns", "Median time (ns)", "SHADOW_LATENCY_TOLERANCE", 0.20),
    ("allocated_bytes", "Allocated (B/op)", "SHADOW_ALLOCATION_TOLERANCE", 0.50),
    ("gen2_per_1000", "Gen2 / 1k ops", "SHADOW_GEN2_TOLERANCE", 0.50),
)


def tolerance(variable: str, default: float) -> float:
    raw = os.environ.get(variable, "").strip()
    if not raw:
        return default
    value = float(raw)
    if value < 0:
        raise ValueError(f"{variable} must be >= 0, got {raw}")
    return value


def load(results_dir: str) -> dict[str, dict[str, float]]:
    scenarios: dict[str, dict[str, float]] = {}
    for path in sorted(glob.glob(os.path.join(results_dir, "*-report-full-compressed.json"))):
        with open(path, encoding="utf-8") as handle:
            report = json.load(handle)
        for benchmark in report.get("Benchmarks", []):
            statistics = benchmark.get("Statistics") or {}
            memory = benchmark.get("Memory") or {}
            scenarios[benchmark["Method"]] = {
                "median_ns": float(statistics.get("Median") or 0.0),
                "allocated_bytes": float(memory.get("BytesAllocatedPerOperation") or 0.0),
                "gen0_per_1000": float(memory.get("Gen0Collections") or 0.0) * 1000.0 / max(1, int(memory.get("TotalOperations") or 1)),
                "gen2_per_1000": float(memory.get("Gen2Collections") or 0.0) * 1000.0 / max(1, int(memory.get("TotalOperations") or 1)),
            }
    return scenarios


def delta(current: float, baseline: float) -> float:
    if baseline == 0:
        return 0.0 if current == 0 else float("inf")
    return (current - baseline) / baseline


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("current")
    parser.add_argument("baseline")
    parser.add_argument("--report", help="also write the markdown table to this file")
    args = parser.parse_args()

    current = load(args.current)
    baseline = load(args.baseline)
    if not current or not baseline:
        print(f"::error::No BenchmarkDotNet results in {args.current if not current else args.baseline}; the run failed before measuring.")
        return 2

    missing = sorted(set(current) ^ set(baseline))
    if missing:
        print(f"::error::Scenarios present in only one run: {', '.join(missing)}. The two runs did not measure the same code.")
        return 2

    tolerances = {key: tolerance(variable, default) for key, _, variable, default in METRICS}
    lines = [
        "| Scenario | Metric | Baseline | Current | Change | Tolerance |",
        "|---|---|---:|---:|---:|---:|",
    ]
    regressions: list[str] = []

    for name in sorted(current):
        for key, label, _, _ in METRICS:
            change = delta(current[name][key], baseline[name][key])
            limit = tolerances[key]
            flag = " **REGRESSION**" if change > limit else ""
            lines.append(
                f"| {name} | {label} | {baseline[name][key]:,.1f} | {current[name][key]:,.1f} | {change:+.1%}{flag} | {limit:.0%} |"
            )
            if change > limit:
                regressions.append(f"{name}: {label} {change:+.1%} (tolerance {limit:.0%})")
        lines.append(
            f"| {name} | Gen0 / 1k ops (info) | {baseline[name]['gen0_per_1000']:,.1f} | {current[name]['gen0_per_1000']:,.1f} | "
            f"{delta(current[name]['gen0_per_1000'], baseline[name]['gen0_per_1000']):+.1%} | - |"
        )

    table = "\n".join(lines)
    print(table)
    if args.report:
        with open(args.report, "w", encoding="utf-8") as handle:
            handle.write(table + "\n")

    if regressions:
        print()
        for regression in regressions:
            print(f"::error::{regression}")
        return 1

    print("\nEvery scenario is within tolerance of the baseline release.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
