window.BENCHMARK_DATA = {
  "lastUpdate": 1791674407375,
  "repoUrl": "https://github.com/Chris-Wolfgang/System.Mail-Extensions",
  "entries": {
    "BenchmarkDotNet": [
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "fd4f83580c750478b83a3e2ca7a8480a9764e63b",
          "message": "Merge pull request #229 from Chris-Wolfgang/vNext\n\nrelease: v0.3.1 — maintenance (code-scanning noise floor → 0)",
          "timestamp": "2026-08-14T12:17:46-04:00",
          "tree_id": "cc2eb44e27f888795e3d3bd78880a8ef9d8c0b4b",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/fd4f83580c750478b83a3e2ca7a8480a9764e63b"
        },
        "date": 1786724401030,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1024)",
            "value": 1151.2950859069824,
            "unit": "ns",
            "range": "± 12.826297123551681"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1024)",
            "value": 3171.890698750814,
            "unit": "ns",
            "range": "± 6.4703024099176085"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1048576)",
            "value": 1095.9748522440593,
            "unit": "ns",
            "range": "± 3.1678395808827635"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1048576)",
            "value": 2064337.2721354167,
            "unit": "ns",
            "range": "± 5034.723202293667"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 1000)",
            "value": 3578.3026898701987,
            "unit": "ns",
            "range": "± 54.70697140613589"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 1000)",
            "value": 8694.725596110025,
            "unit": "ns",
            "range": "± 128.62042657096833"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 1000)",
            "value": 14753.549296061197,
            "unit": "ns",
            "range": "± 81.18259641360099"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 1000)",
            "value": 7120.028167724609,
            "unit": "ns",
            "range": "± 94.35443145546125"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 100000)",
            "value": 53483.4209391276,
            "unit": "ns",
            "range": "± 2290.434777510667"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 100000)",
            "value": 299222.2024739583,
            "unit": "ns",
            "range": "± 21698.160766548906"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 100000)",
            "value": 731997.3248697916,
            "unit": "ns",
            "range": "± 14665.724980507885"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 100000)",
            "value": 398253.966796875,
            "unit": "ns",
            "range": "± 11986.46340631528"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 0)",
            "value": 14168.677510579428,
            "unit": "ns",
            "range": "± 425.10929721976896"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 0)",
            "value": 2025.4469731648762,
            "unit": "ns",
            "range": "± 5.765237650915686"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 64)",
            "value": 934043.390625,
            "unit": "ns",
            "range": "± 2331.4936652116903"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 64)",
            "value": 8808.226399739584,
            "unit": "ns",
            "range": "± 43.998631587481626"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "9234cdbfd529dc522af0685a8b47bbddcb7edd0a",
          "message": "Merge pull request #231 from Chris-Wolfgang/chore/bump-baseline-to-0.3.1\n\nchore(release): advance PackageValidationBaselineVersion to 0.3.1",
          "timestamp": "2026-08-16T10:10:38-04:00",
          "tree_id": "3c20bf6ff87a8fe6ddf793fd9574ee5fc1143053",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/9234cdbfd529dc522af0685a8b47bbddcb7edd0a"
        },
        "date": 1786889565908,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1024)",
            "value": 1297.9579855600994,
            "unit": "ns",
            "range": "± 8.269310510271385"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1024)",
            "value": 3165.765427271525,
            "unit": "ns",
            "range": "± 67.51110445066485"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1048576)",
            "value": 1245.7166970570881,
            "unit": "ns",
            "range": "± 32.42204667448257"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1048576)",
            "value": 1996174.1041666667,
            "unit": "ns",
            "range": "± 16496.95156424684"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 1000)",
            "value": 3999.112836201986,
            "unit": "ns",
            "range": "± 74.67954100275153"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 1000)",
            "value": 9325.586130777994,
            "unit": "ns",
            "range": "± 118.35029524614309"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 1000)",
            "value": 14432.4443359375,
            "unit": "ns",
            "range": "± 110.25343855011083"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 1000)",
            "value": 7304.954650878906,
            "unit": "ns",
            "range": "± 16.72793980435273"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 100000)",
            "value": 62204.38175455729,
            "unit": "ns",
            "range": "± 1284.2361973677037"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 100000)",
            "value": 285766.1207682292,
            "unit": "ns",
            "range": "± 11571.930251624366"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 100000)",
            "value": 706278.9147135416,
            "unit": "ns",
            "range": "± 16678.54169059469"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 100000)",
            "value": 382114.9078776042,
            "unit": "ns",
            "range": "± 11685.635861720883"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 0)",
            "value": 18152.321126302082,
            "unit": "ns",
            "range": "± 835.0251452753056"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 0)",
            "value": 2204.026226043701,
            "unit": "ns",
            "range": "± 9.740040747120933"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 64)",
            "value": 977504.76171875,
            "unit": "ns",
            "range": "± 37428.37714771678"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 64)",
            "value": 11620.247904459635,
            "unit": "ns",
            "range": "± 306.38254818074785"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "41fc580b1563c143d1385f40530b9e4c9dbdd8d3",
          "message": "ci: pin every workflow action to a commit SHA with an exact # vX.Y.Z comment (#249)\n\nRan repo-template's scripts/pin-actions.ps1 -PinTags: tag references become\nSHA pins and major-only comments (# v7) become the exact tag on the pinned\ncommit (# v7.0.1), so zizmor's ref-version-mismatch stops firing when the\nmajor tag moves on. Only the ref/comment text changed. Dependabot keeps the\nprecision it finds, so this stays converted.\n\n20 already exact, 30 line(s) rewritten, 0 tag reference(s), 0 pinned SHA(s) with no tag\n\nRefs Chris-Wolfgang/repo-template#447\n\nCo-authored-by: Chris Wolfgang <cwolfgan@ptd.net>\nCo-authored-by: Claude Opus 5 <noreply@anthropic.com>",
          "timestamp": "2026-09-16T22:10:51-04:00",
          "tree_id": "d9a2fe7f3654e86437373d9b99c8049ff4b97f44",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/41fc580b1563c143d1385f40530b9e4c9dbdd8d3"
        },
        "date": 1789611178076,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1024)",
            "value": 1334.7245928446453,
            "unit": "ns",
            "range": "± 4.1186966777828555"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1024)",
            "value": 3250.1852480570474,
            "unit": "ns",
            "range": "± 28.61718386533947"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1048576)",
            "value": 1326.3064047495525,
            "unit": "ns",
            "range": "± 11.206107927641023"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1048576)",
            "value": 2106262.625,
            "unit": "ns",
            "range": "± 20763.082502699737"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 1000)",
            "value": 4129.119361877441,
            "unit": "ns",
            "range": "± 43.2144844440388"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 1000)",
            "value": 10893.31923421224,
            "unit": "ns",
            "range": "± 487.9091677384927"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 1000)",
            "value": 15641.442372639975,
            "unit": "ns",
            "range": "± 298.6426514045888"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 1000)",
            "value": 7616.546401977539,
            "unit": "ns",
            "range": "± 118.25730437463932"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 100000)",
            "value": 64783.85900878906,
            "unit": "ns",
            "range": "± 3253.9268574394696"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 100000)",
            "value": 321753.103515625,
            "unit": "ns",
            "range": "± 7376.969600305818"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 100000)",
            "value": 795073.6927083334,
            "unit": "ns",
            "range": "± 44296.34608685252"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 100000)",
            "value": 411148.0302734375,
            "unit": "ns",
            "range": "± 14693.16974959754"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 0)",
            "value": 19031.598510742188,
            "unit": "ns",
            "range": "± 485.585782371495"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 0)",
            "value": 2316.392021179199,
            "unit": "ns",
            "range": "± 22.102995528086232"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 64)",
            "value": 1043476.7877604166,
            "unit": "ns",
            "range": "± 13795.157063957466"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 64)",
            "value": 13485.942993164062,
            "unit": "ns",
            "range": "± 230.21348201852715"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "49699333+dependabot[bot]@users.noreply.github.com",
            "name": "dependabot[bot]",
            "username": "dependabot[bot]"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "be6681ad5285a45e068e18716840e6a8418b4cdc",
          "message": "build(deps): bump the github-actions group with 4 updates (#279)\n\nBumps the github-actions group with 4 updates: [github/codeql-action/upload-sarif](https://github.com/github/codeql-action), [benchmark-action/github-action-benchmark](https://github.com/benchmark-action/github-action-benchmark), [github/codeql-action/init](https://github.com/github/codeql-action) and [github/codeql-action/analyze](https://github.com/github/codeql-action).\n\n\nUpdates `github/codeql-action/upload-sarif` from 4.38.0 to 4.38.1\n- [Release notes](https://github.com/github/codeql-action/releases)\n- [Changelog](https://github.com/github/codeql-action/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/github/codeql-action/compare/b96794f015dfd88f77b49b1c93e0fa7110f94c63...1c5b675653bb5c22dbe9b12b556ec555138e09fd)\n\nUpdates `benchmark-action/github-action-benchmark` from 1.22.1 to 1.22.2\n- [Release notes](https://github.com/benchmark-action/github-action-benchmark/releases)\n- [Changelog](https://github.com/benchmark-action/github-action-benchmark/blob/master/CHANGELOG.md)\n- [Commits](https://github.com/benchmark-action/github-action-benchmark/compare/v1.22.1...4322e5726e6334590d251fc4f92bec0efafc45dc)\n\nUpdates `github/codeql-action/init` from 4.38.0 to 4.38.1\n- [Release notes](https://github.com/github/codeql-action/releases)\n- [Changelog](https://github.com/github/codeql-action/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/github/codeql-action/compare/b96794f015dfd88f77b49b1c93e0fa7110f94c63...1c5b675653bb5c22dbe9b12b556ec555138e09fd)\n\nUpdates `github/codeql-action/analyze` from 4.38.0 to 4.38.1\n- [Release notes](https://github.com/github/codeql-action/releases)\n- [Changelog](https://github.com/github/codeql-action/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/github/codeql-action/compare/b96794f015dfd88f77b49b1c93e0fa7110f94c63...1c5b675653bb5c22dbe9b12b556ec555138e09fd)\n\n---\nupdated-dependencies:\n- dependency-name: github/codeql-action/upload-sarif\n  dependency-version: 4.38.1\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n- dependency-name: benchmark-action/github-action-benchmark\n  dependency-version: 1.22.2\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n- dependency-name: github/codeql-action/init\n  dependency-version: 4.38.1\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n- dependency-name: github/codeql-action/analyze\n  dependency-version: 4.38.1\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>\nCo-authored-by: Chris Wolfgang <210299580+Chris-Wolfgang@users.noreply.github.com>",
          "timestamp": "2026-10-10T19:07:39-04:00",
          "tree_id": "4488a82616c9bb5181f5123dd287fb301d8c3923",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/be6681ad5285a45e068e18716840e6a8418b4cdc"
        },
        "date": 1791674405193,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1024)",
            "value": 1259.8207467397053,
            "unit": "ns",
            "range": "± 9.898382492673761"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1024)",
            "value": 2869.6468772888184,
            "unit": "ns",
            "range": "± 4.088002515983742"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1048576)",
            "value": 1213.555311203003,
            "unit": "ns",
            "range": "± 4.768205465943477"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1048576)",
            "value": 1599508.8151041667,
            "unit": "ns",
            "range": "± 5835.545682722028"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 1000)",
            "value": 3888.9423344930015,
            "unit": "ns",
            "range": "± 16.87381087022711"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 1000)",
            "value": 9500.758117675781,
            "unit": "ns",
            "range": "± 51.04439184633617"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 1000)",
            "value": 15318.179077148438,
            "unit": "ns",
            "range": "± 54.73461022154276"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 1000)",
            "value": 7410.6451365153,
            "unit": "ns",
            "range": "± 53.59366863693846"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 100000)",
            "value": 57912.75821940104,
            "unit": "ns",
            "range": "± 3354.430960929304"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 100000)",
            "value": 259191.9208984375,
            "unit": "ns",
            "range": "± 3899.672253441476"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 100000)",
            "value": 725468.4713541666,
            "unit": "ns",
            "range": "± 4386.141010350908"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 100000)",
            "value": 407289.7291666667,
            "unit": "ns",
            "range": "± 13699.604970551854"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 0)",
            "value": 16612.610616048176,
            "unit": "ns",
            "range": "± 71.6288473167915"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 0)",
            "value": 2065.6650060017905,
            "unit": "ns",
            "range": "± 3.414965875048901"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 64)",
            "value": 959079.91015625,
            "unit": "ns",
            "range": "± 11015.146688375125"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 64)",
            "value": 13294.770161946615,
            "unit": "ns",
            "range": "± 591.5247342155433"
          }
        ]
      }
    ]
  }
}