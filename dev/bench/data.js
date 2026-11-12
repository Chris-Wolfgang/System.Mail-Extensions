window.BENCHMARK_DATA = {
  "lastUpdate": 1794509949047,
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
          "id": "df7cf87080fe0e08db3ab23e6f5962e8b685ab9f",
          "message": "test: tighter assertions and the missing cases from the review (#332)\n\n- EmlParserTests: malformed From asserts From is null; malformed base64\n  asserts the raw body; three Contains -> Equal (HTML body, QP body,\n  folded subject); #if NETFRAMEWORK instead of NETSTANDARD2_0||NETFRAMEWORK.\n- Validate: exactly two errors. ToMimeString: assert the header lines.\n- New: FormatMailAddress quote and backslash escaping, ExceedsLimit at the\n  exact boundary, InlineHtmlBuilder undoubled brace -> FormatException and\n  doubled braces -> single braces.\n- S8969 pragma comments name the .NET Framework TFMs (no netstandard2.0\n  test target).\n- Benchmarks dispose the parsed/cloned message per iteration.\n\nRefs #308\n\nCo-authored-by: Claude Fable 5.1 <noreply@anthropic.com>",
          "timestamp": "2026-11-05T11:59:30-05:00",
          "tree_id": "bdbcdfd7d5a2a7f2ec8a8854fc4e2c8b01406e0a",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/df7cf87080fe0e08db3ab23e6f5962e8b685ab9f"
        },
        "date": 1793898113915,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1024)",
            "value": 1260.3812478383381,
            "unit": "ns",
            "range": "± 27.213205197128833"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1024)",
            "value": 2940.491288503011,
            "unit": "ns",
            "range": "± 83.90581009166168"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1048576)",
            "value": 1240.9169457753499,
            "unit": "ns",
            "range": "± 2.5195872181898684"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1048576)",
            "value": 1638632.8671875,
            "unit": "ns",
            "range": "± 22677.13247110905"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 1000)",
            "value": 4043.7409439086914,
            "unit": "ns",
            "range": "± 54.95674290167176"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 1000)",
            "value": 9462.330301920572,
            "unit": "ns",
            "range": "± 59.44587376665642"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 1000)",
            "value": 15648.731852213541,
            "unit": "ns",
            "range": "± 227.00907735097996"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 1000)",
            "value": 7563.2187093098955,
            "unit": "ns",
            "range": "± 77.10590797847497"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 100000)",
            "value": 60816.8857828776,
            "unit": "ns",
            "range": "± 545.3969537608165"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 100000)",
            "value": 268301.3766276042,
            "unit": "ns",
            "range": "± 2733.2790442886544"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 100000)",
            "value": 687031.6705729166,
            "unit": "ns",
            "range": "± 9658.617530499328"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 100000)",
            "value": 406083.5895182292,
            "unit": "ns",
            "range": "± 5267.777118497223"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 0)",
            "value": 17428.36181640625,
            "unit": "ns",
            "range": "± 1312.2448912364675"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 0)",
            "value": 2203.6873054504395,
            "unit": "ns",
            "range": "± 24.05863044111647"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 64)",
            "value": 944432.673828125,
            "unit": "ns",
            "range": "± 17472.12559400253"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 64)",
            "value": 12933.690633138021,
            "unit": "ns",
            "range": "± 199.96648276339806"
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
          "id": "30a0198eb5dbedef248cccdfaa7c8cd2de07d8d3",
          "message": "ci: raise the coverage gates to src 95 % / tests 100 % (protected-only) (#337)\n\n* ci: raise the coverage gates to src 95 % / tests 100 % (pr.yaml, release.yaml, build-pr.ps1)\n\npr.yaml: CODECOV_MINIMUM 90 -> 95, CODECOV_TEST_MINIMUM 94 -> 100 (the\nrepository floor comment goes with it); the 90 % step names and fallbacks\nfollow. release.yaml and build-pr.ps1 defaults: 90 -> 95 (both already\nheld tests at 100). Possible now that Tests.Unit is at 100 % (the PR\nbelow) and src is at 97.7 %.\n\nRefs #284\n\nCo-Authored-By: Claude Fable 5.1 <noreply@anthropic.com>\n\n* ci(pr): coverage step names carry no percentage\n\nThe coverage steps are now just 'Enforce coverage thresholds' and the\nStage 1 summary line says 'coverage gate'; the values live only in the\nenv block (CODECOV_MINIMUM / CODECOV_TEST_MINIMUM), so changing a\nthreshold never means renaming a step. Step names are not required-check\ncontexts (job names are), so the ruleset is unaffected. Same change as\nChris-Wolfgang/repo-template#795.\n\nCo-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>\n\n---------\n\nCo-authored-by: Claude Fable 5.1 <noreply@anthropic.com>",
          "timestamp": "2026-11-05T13:48:53-05:00",
          "tree_id": "c0760e91725671631e6f75de082eaf7174b8cb84",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/30a0198eb5dbedef248cccdfaa7c8cd2de07d8d3"
        },
        "date": 1793904681631,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1024)",
            "value": 909.5083325703939,
            "unit": "ns",
            "range": "± 6.837943571124572"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1024)",
            "value": 2579.0172589619956,
            "unit": "ns",
            "range": "± 68.46531860366599"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1048576)",
            "value": 902.2922919591268,
            "unit": "ns",
            "range": "± 11.049610364867304"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1048576)",
            "value": 1605377.4921875,
            "unit": "ns",
            "range": "± 8406.43488178048"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 1000)",
            "value": 2888.0799992879233,
            "unit": "ns",
            "range": "± 65.41665827905074"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 1000)",
            "value": 7110.499542236328,
            "unit": "ns",
            "range": "± 65.55219362886211"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 1000)",
            "value": 11569.16431681315,
            "unit": "ns",
            "range": "± 242.71735783837082"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 1000)",
            "value": 5437.491947174072,
            "unit": "ns",
            "range": "± 105.60039689491724"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 100000)",
            "value": 49245.29890950521,
            "unit": "ns",
            "range": "± 952.2698728906453"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 100000)",
            "value": 229705.90966796875,
            "unit": "ns",
            "range": "± 3526.2803297694863"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 100000)",
            "value": 558224.234375,
            "unit": "ns",
            "range": "± 11063.188057957374"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 100000)",
            "value": 322470.65218098956,
            "unit": "ns",
            "range": "± 14251.936612761274"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 0)",
            "value": 10936.326436360678,
            "unit": "ns",
            "range": "± 605.4921149348382"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 0)",
            "value": 1628.6477495829265,
            "unit": "ns",
            "range": "± 24.926119489785513"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 64)",
            "value": 713540.185546875,
            "unit": "ns",
            "range": "± 21482.75081301931"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 64)",
            "value": 6620.791585286458,
            "unit": "ns",
            "range": "± 56.979356217388684"
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
          "id": "1389851701a52b62e06ba683fd5da13aa7d517a9",
          "message": "fix(parser): skip empty custom headers; report a non-ASCII header name (#339)\n\nApplyCustomHeaders copied every non-standard header into\nMailMessage.Headers, which throws ArgumentException for an empty value\n(X-Foo:) and FormatException for a non-ASCII name, so the lenient Parse\nthrew on both (net462, net6.0 and net10.0 alike). Empty custom headers\nare now skipped; a malformed name is skipped and reported through the\nparse context (diagnostic, or EmlParseException in Strict mode).\n\nRefs #338\n\nCo-authored-by: Claude Fable 5.1 <noreply@anthropic.com>",
          "timestamp": "2026-11-05T16:42:27-05:00",
          "tree_id": "93ce03cf7a20c0dea2adcb4dbe2658471d2d8f3e",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/1389851701a52b62e06ba683fd5da13aa7d517a9"
        },
        "date": 1793915099171,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1024)",
            "value": 1206.6930885314941,
            "unit": "ns",
            "range": "± 5.227717713185458"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1024)",
            "value": 3135.4012616475425,
            "unit": "ns",
            "range": "± 23.10405667425858"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1048576)",
            "value": 1232.81742477417,
            "unit": "ns",
            "range": "± 12.937757458345446"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1048576)",
            "value": 1945550.2682291667,
            "unit": "ns",
            "range": "± 7755.764732916495"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 1000)",
            "value": 3832.697504679362,
            "unit": "ns",
            "range": "± 52.510745058577065"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 1000)",
            "value": 8934.513580322266,
            "unit": "ns",
            "range": "± 64.04866186389742"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 1000)",
            "value": 14369.565673828125,
            "unit": "ns",
            "range": "± 246.70688230364706"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 1000)",
            "value": 7061.132146199544,
            "unit": "ns",
            "range": "± 16.126393430014552"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 100000)",
            "value": 64210.5752360026,
            "unit": "ns",
            "range": "± 1762.3186965633543"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 100000)",
            "value": 274639.6077473958,
            "unit": "ns",
            "range": "± 15746.786415895649"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 100000)",
            "value": 663963.1217447916,
            "unit": "ns",
            "range": "± 8674.568872223632"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 100000)",
            "value": 380057.14892578125,
            "unit": "ns",
            "range": "± 3132.40699665563"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 0)",
            "value": 17936.111938476562,
            "unit": "ns",
            "range": "± 1095.3550011596121"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 0)",
            "value": 2364.723066965739,
            "unit": "ns",
            "range": "± 19.51043078120395"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 64)",
            "value": 911841.4622395834,
            "unit": "ns",
            "range": "± 15816.345957212669"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 64)",
            "value": 9844.643005371094,
            "unit": "ns",
            "range": "± 111.0162385576623"
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
          "id": "8c9135ebc96efb34e4b5774af04b52fcd63db81c",
          "message": "ci: weekly fuzz workflow for the Tests.Fuzz properties (fuzz.yaml) (#374)\n\nRuns tests/Wolfgang.Extensions.Mail.Tests.Fuzz weekly (Monday 04:17 UTC) and\non demand with 100,000 cases and a 300-second budget per property (inputs\nfor both, plus a seed to replay a finding through FUZZ_SEED). The verdict\ncomes from the TRX counters, so a build or adapter failure fails the run\nwithout filing anything. The log and TRX are uploaded as the fuzz-results\nartifact (the seed is the durable record); a failing property files one\n\"Fuzz finding\" issue, or comments on the open one, and fails the run.\nShape from DateTime-Extensions' fuzz.yaml.\n\nCo-authored-by: Claude Fable 5.1 <noreply@anthropic.com>",
          "timestamp": "2026-11-06T13:55:08-05:00",
          "tree_id": "8e4493cbee8fdc93fa5d962791a4e884caecd7b9",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/8c9135ebc96efb34e4b5774af04b52fcd63db81c"
        },
        "date": 1793991816737,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1024)",
            "value": 1240.1854832967122,
            "unit": "ns",
            "range": "± 30.38844405673216"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1024)",
            "value": 3042.723808288574,
            "unit": "ns",
            "range": "± 53.304726298425706"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1048576)",
            "value": 1186.9655793507893,
            "unit": "ns",
            "range": "± 11.81940812712802"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1048576)",
            "value": 1922449.7643229167,
            "unit": "ns",
            "range": "± 7250.178602157927"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 1000)",
            "value": 3817.928398132324,
            "unit": "ns",
            "range": "± 2.069999319173819"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 1000)",
            "value": 9217.017313639322,
            "unit": "ns",
            "range": "± 95.47253737148883"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 1000)",
            "value": 13792.524007161459,
            "unit": "ns",
            "range": "± 194.8807826882336"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 1000)",
            "value": 7101.697977701823,
            "unit": "ns",
            "range": "± 44.10248967100666"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 100000)",
            "value": 61778.90376790365,
            "unit": "ns",
            "range": "± 1080.149607531009"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 100000)",
            "value": 292046.2311197917,
            "unit": "ns",
            "range": "± 24065.040199417832"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 100000)",
            "value": 684811.515625,
            "unit": "ns",
            "range": "± 20018.730164625504"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 100000)",
            "value": 397134.5615234375,
            "unit": "ns",
            "range": "± 8916.33551154258"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 0)",
            "value": 14459.451334635416,
            "unit": "ns",
            "range": "± 232.41060437693554"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 0)",
            "value": 2257.4896659851074,
            "unit": "ns",
            "range": "± 4.596847614176722"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 64)",
            "value": 906286.9010416666,
            "unit": "ns",
            "range": "± 12796.220935793719"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 64)",
            "value": 9101.76889038086,
            "unit": "ns",
            "range": "± 46.618526142406026"
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
          "id": "71e77cd704319438d9e9f04a89824a2ee7933340",
          "message": "chore(pack): stop requiring licence acceptance for the MIT package (#381)\n\nPackageRequireLicenseAcceptance=True makes Visual Studio show an\naccept-licence prompt on install. MIT does not require acceptance, so the\nprompt is only friction.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-11-08T17:07:13-05:00",
          "tree_id": "959915f92afd3b069f6d47c51d3f4f42a1d15181",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/71e77cd704319438d9e9f04a89824a2ee7933340"
        },
        "date": 1794176230933,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1024)",
            "value": 833.0758169492086,
            "unit": "ns",
            "range": "± 8.057879301208722"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1024)",
            "value": 2488.907283782959,
            "unit": "ns",
            "range": "± 86.3312622497535"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1048576)",
            "value": 847.6525402069092,
            "unit": "ns",
            "range": "± 3.112381272219609"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1048576)",
            "value": 1634791.4244791667,
            "unit": "ns",
            "range": "± 2397.1618725299227"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 1000)",
            "value": 2808.4318466186523,
            "unit": "ns",
            "range": "± 8.772563503279926"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 1000)",
            "value": 6627.351888020833,
            "unit": "ns",
            "range": "± 25.0963572332266"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 1000)",
            "value": 10646.17855834961,
            "unit": "ns",
            "range": "± 59.23892886555841"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 1000)",
            "value": 5184.408594767253,
            "unit": "ns",
            "range": "± 16.70759256330853"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 100000)",
            "value": 45006.83692423502,
            "unit": "ns",
            "range": "± 182.41092312859004"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 100000)",
            "value": 239225.78873697916,
            "unit": "ns",
            "range": "± 351.83943108205244"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 100000)",
            "value": 570896.1663411459,
            "unit": "ns",
            "range": "± 24655.053653742394"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 100000)",
            "value": 307356.6044921875,
            "unit": "ns",
            "range": "± 1221.9797324125666"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 0)",
            "value": 9658.440724690756,
            "unit": "ns",
            "range": "± 334.0940875031738"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 0)",
            "value": 1573.4346917470295,
            "unit": "ns",
            "range": "± 10.849154601719649"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 64)",
            "value": 705203.8177083334,
            "unit": "ns",
            "range": "± 2412.042544748327"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 64)",
            "value": 6536.580581665039,
            "unit": "ns",
            "range": "± 30.338274068723244"
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
          "id": "8b091d4d8be5062b3e369b5b8b1c7ce9946959e9",
          "message": "chore(editorconfig): VSTHRD111 comment points at CA2007 instead of contradicting it (#380)\n\nThe global comment said ConfigureAwait is \"not needed in library code\ntargeting modern .NET\", while the src/ section enforces it with CA2007\nbecause net462/netstandard2.0 ship. Comment only; severity unchanged. The\nlast open item of #306.\n\nCo-authored-by: Claude Fable 5.1 <noreply@anthropic.com>",
          "timestamp": "2026-11-08T18:50:32-05:00",
          "tree_id": "e8b9be0a08749845b80afc4ff840cc47c37beb77",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/8b091d4d8be5062b3e369b5b8b1c7ce9946959e9"
        },
        "date": 1794181976442,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1024)",
            "value": 928.5433190663656,
            "unit": "ns",
            "range": "± 7.234730634742571"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1024)",
            "value": 2558.369659423828,
            "unit": "ns",
            "range": "± 19.544151796863556"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1048576)",
            "value": 972.1802132924398,
            "unit": "ns",
            "range": "± 27.57390670721911"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1048576)",
            "value": 1654210.0416666667,
            "unit": "ns",
            "range": "± 8771.577883902608"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 1000)",
            "value": 3257.3720105489097,
            "unit": "ns",
            "range": "± 27.75552259588802"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 1000)",
            "value": 6044.077499389648,
            "unit": "ns",
            "range": "± 56.569761001281925"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 1000)",
            "value": 10109.343856811523,
            "unit": "ns",
            "range": "± 59.43384815691103"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 1000)",
            "value": 6039.444338480632,
            "unit": "ns",
            "range": "± 217.03970541888208"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 100000)",
            "value": 46641.84266153971,
            "unit": "ns",
            "range": "± 1660.8008157969257"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 100000)",
            "value": 122420.68595377605,
            "unit": "ns",
            "range": "± 1113.2395198688287"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 100000)",
            "value": 444689.8766276042,
            "unit": "ns",
            "range": "± 35070.52298689789"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 100000)",
            "value": 309558.54410807294,
            "unit": "ns",
            "range": "± 12015.057278923976"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 0)",
            "value": 10646.91103108724,
            "unit": "ns",
            "range": "± 101.1420525650968"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 0)",
            "value": 1679.7538013458252,
            "unit": "ns",
            "range": "± 6.830603303615523"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 64)",
            "value": 737063.0296223959,
            "unit": "ns",
            "range": "± 5193.463878576246"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 64)",
            "value": 7225.866607666016,
            "unit": "ns",
            "range": "± 78.92362244032344"
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
          "id": "564efa372cb1a5bf0ae2c1a8267c6b2a6abf5167",
          "message": "fix(parser): treat an empty angle-bracket address as malformed in lenient mode (#416)\n\n* fix(parser): treat an empty angle-bracket address as malformed in lenient mode\n\nMailAddress throws ArgumentException (not FormatException) for an empty\naddress, so Cc: -@=_< > escaped Parse's lenient path. Found by the\nscheduled fuzz run (seed 3mx1jzLAnQC1); refs #415.\n\nCo-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>\n\n* docs(changelog): fragment for the empty angle-bracket address fix\n\nCo-Authored-By: Claude Sonnet 5.5 <noreply@anthropic.com>\n\n---------\n\nCo-authored-by: Claude Sonnet 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-11-10T13:48:10-05:00",
          "tree_id": "48cd42785f732293ec7618ff58e314f0ff066368",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/564efa372cb1a5bf0ae2c1a8267c6b2a6abf5167"
        },
        "date": 1794336690657,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1024)",
            "value": 1219.212547938029,
            "unit": "ns",
            "range": "± 9.595198760657611"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1024)",
            "value": 3103.5520617167153,
            "unit": "ns",
            "range": "± 25.80688008521117"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1048576)",
            "value": 1230.5687376658123,
            "unit": "ns",
            "range": "± 19.317196835654574"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1048576)",
            "value": 1964983.9114583333,
            "unit": "ns",
            "range": "± 21926.26606139682"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 1000)",
            "value": 3956.5900700887046,
            "unit": "ns",
            "range": "± 15.619979242030547"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 1000)",
            "value": 7533.527229309082,
            "unit": "ns",
            "range": "± 34.42644329386655"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 1000)",
            "value": 12387.118611653646,
            "unit": "ns",
            "range": "± 118.02373608189701"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 1000)",
            "value": 6982.954182942708,
            "unit": "ns",
            "range": "± 93.21646954542372"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 100000)",
            "value": 59461.108154296875,
            "unit": "ns",
            "range": "± 545.6293216221745"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 100000)",
            "value": 159581.2042643229,
            "unit": "ns",
            "range": "± 16106.189052479373"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 100000)",
            "value": 561056.3697916666,
            "unit": "ns",
            "range": "± 28121.221107668378"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 100000)",
            "value": 395488.2692057292,
            "unit": "ns",
            "range": "± 22861.400250865798"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 0)",
            "value": 14132.055541992188,
            "unit": "ns",
            "range": "± 317.92821485656646"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 0)",
            "value": 2291.291695912679,
            "unit": "ns",
            "range": "± 24.935589187014205"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 64)",
            "value": 908659.5872395834,
            "unit": "ns",
            "range": "± 5277.285699580876"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 64)",
            "value": 9480.104654947916,
            "unit": "ns",
            "range": "± 225.8440061212388"
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
          "id": "9887b984cade2139e4b9fada51a51fe2fefd6f17",
          "message": "build(deps): bump the github-actions group across 1 directory with 5 updates (#420)\n\nBumps the github-actions group with 5 updates in the / directory:\n\n| Package | From | To |\n| --- | --- | --- |\n| [actions/setup-python](https://github.com/actions/setup-python) | `7.0.0` | `7.1.0` |\n| [github/codeql-action/upload-sarif](https://github.com/github/codeql-action) | `4.39.1` | `4.39.3` |\n| [actions/setup-dotnet](https://github.com/actions/setup-dotnet) | `6.0.0` | `6.1.0` |\n| [github/codeql-action/init](https://github.com/github/codeql-action) | `4.39.1` | `4.39.3` |\n| [github/codeql-action/analyze](https://github.com/github/codeql-action) | `4.39.1` | `4.39.3` |\n\n\n\nUpdates `actions/setup-python` from 7.0.0 to 7.1.0\n- [Release notes](https://github.com/actions/setup-python/releases)\n- [Commits](https://github.com/actions/setup-python/compare/v7...6d4fd22b20465ccc0f7354476588e8df7c9ca0e0)\n\nUpdates `github/codeql-action/upload-sarif` from 4.39.1 to 4.39.3\n- [Release notes](https://github.com/github/codeql-action/releases)\n- [Changelog](https://github.com/github/codeql-action/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/github/codeql-action/compare/9b3254085494b55a0f7c4c2b3d2d52cc712ce370...33d7b85da1a04171e211befe2c739303b9a4c066)\n\nUpdates `actions/setup-dotnet` from 6.0.0 to 6.1.0\n- [Release notes](https://github.com/actions/setup-dotnet/releases)\n- [Commits](https://github.com/actions/setup-dotnet/compare/a98b56852c35b8e3190ac28c8c2271da59106c68...0f4a354dd5547a28c70b7853dbd95d5b67886e73)\n\nUpdates `github/codeql-action/init` from 4.39.1 to 4.39.3\n- [Release notes](https://github.com/github/codeql-action/releases)\n- [Changelog](https://github.com/github/codeql-action/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/github/codeql-action/compare/9b3254085494b55a0f7c4c2b3d2d52cc712ce370...33d7b85da1a04171e211befe2c739303b9a4c066)\n\nUpdates `github/codeql-action/analyze` from 4.39.1 to 4.39.3\n- [Release notes](https://github.com/github/codeql-action/releases)\n- [Changelog](https://github.com/github/codeql-action/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/github/codeql-action/compare/9b3254085494b55a0f7c4c2b3d2d52cc712ce370...33d7b85da1a04171e211befe2c739303b9a4c066)\n\n---\nupdated-dependencies:\n- dependency-name: actions/setup-dotnet\n  dependency-version: 6.1.0\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: github-actions\n- dependency-name: actions/setup-python\n  dependency-version: 7.1.0\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: github-actions\n- dependency-name: github/codeql-action/analyze\n  dependency-version: 4.39.3\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n- dependency-name: github/codeql-action/init\n  dependency-version: 4.39.3\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n- dependency-name: github/codeql-action/upload-sarif\n  dependency-version: 4.39.3\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: github-actions\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>\nCo-authored-by: Chris Wolfgang <210299580+Chris-Wolfgang@users.noreply.github.com>",
          "timestamp": "2026-11-12T13:42:18-05:00",
          "tree_id": "7d3460da639fd55f3f5ca1538a0ed1241b49045c",
          "url": "https://github.com/Chris-Wolfgang/System.Mail-Extensions/commit/9887b984cade2139e4b9fada51a51fe2fefd6f17"
        },
        "date": 1794509945702,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1024)",
            "value": 1288.6149234771729,
            "unit": "ns",
            "range": "± 18.69704560349175"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1024)",
            "value": 2953.8360481262207,
            "unit": "ns",
            "range": "± 29.466529814738383"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBytes(PayloadSizeBytes: 1048576)",
            "value": 1322.7357552846272,
            "unit": "ns",
            "range": "± 8.485444462465532"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.AttachmentFactoryBenchmarks.FromBase64(PayloadSizeBytes: 1048576)",
            "value": 1711039.5364583333,
            "unit": "ns",
            "range": "± 74354.82131139698"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 1000)",
            "value": 4348.627090454102,
            "unit": "ns",
            "range": "± 29.39316398427602"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 1000)",
            "value": 8153.229034423828,
            "unit": "ns",
            "range": "± 13.086991591629896"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 1000)",
            "value": 14190.349446614584,
            "unit": "ns",
            "range": "± 483.21374222576884"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 1000)",
            "value": 7719.173522949219,
            "unit": "ns",
            "range": "± 20.25258425470033"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParsePlainText(PayloadSizeBytes: 100000)",
            "value": 62410.50919596354,
            "unit": "ns",
            "range": "± 3685.7827762404445"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseMultipartAlternative(PayloadSizeBytes: 100000)",
            "value": 168549.63818359375,
            "unit": "ns",
            "range": "± 11594.179985997189"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseWithBase64Attachment(PayloadSizeBytes: 100000)",
            "value": 604542.0045572916,
            "unit": "ns",
            "range": "± 13640.652352129115"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.EmlParserBenchmarks.ParseQuotedPrintable(PayloadSizeBytes: 100000)",
            "value": 413909.6028645833,
            "unit": "ns",
            "range": "± 14339.627129155157"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 0)",
            "value": 15686.103881835938,
            "unit": "ns",
            "range": "± 831.4279382812686"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 0)",
            "value": 2273.754670461019,
            "unit": "ns",
            "range": "± 2.6764497422710876"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.ToMimeString(AttachmentSizeKb: 64)",
            "value": 991722.4401041666,
            "unit": "ns",
            "range": "± 4821.97088091705"
          },
          {
            "name": "Wolfgang.Extensions.Mail.Benchmarks.MimeSerializationBenchmarks.Clone(AttachmentSizeKb: 64)",
            "value": 14138.963734944662,
            "unit": "ns",
            "range": "± 259.7442247865087"
          }
        ]
      }
    ]
  }
}