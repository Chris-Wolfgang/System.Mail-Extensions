window.BENCHMARK_DATA = {
  "lastUpdate": 1790491570557,
  "repoUrl": "https://github.com/Chris-Wolfgang/System.Mail-Extensions",
  "entries": {
    "Mutation score": [
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "942f09537d1e3df5281c8780c15ad7c2c01f4882",
          "message": "docs(security): take repo-template's security policy, with the disclosure process and timelines (#276)\n\nScorecard scores this repository 4/10 on Security-Policy: \"security policy file\ndetected: Warn: no linked content found\". The file exists - all twelve affected\nrepositories carry the same 735-byte stub - but Scorecard also grades its\nCONTENT, and the stub has no links, no addresses and no concrete timelines, so\nit reads as a placeholder rather than a policy someone could act on.\n\nrepo-template's version is the one that scores: it names the private advisory\nURL for this repository, links GitHub's own guidance on private reporting, and\nreplaces \"we will acknowledge within 48 hours\" with a table that commits to\nassessment and fix windows by severity - explicitly scoped to what a\nsingle-maintainer project can actually meet. It also documents the disclosure\nprocess end to end (report, triage, private fork, release, publish + CVE) and\nsays reporters are credited.\n\nThe {{GITHUB_REPO_URL}} placeholder is substituted for this repository, so the\nadvisory link is real rather than pointing at the template.\n\nCo-authored-by: Claude Opus 5 <noreply@anthropic.com>",
          "timestamp": "2026-09-23T12:31:12Z",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/942f09537d1e3df5281c8780c15ad7c2c01f4882"
        },
        "date": 1790491565476,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 73.27,
            "unit": "%"
          }
        ]
      }
    ]
  }
}