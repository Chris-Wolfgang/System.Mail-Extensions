window.BENCHMARK_DATA = {
  "lastUpdate": 1786889568003,
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
      }
    ]
  }
}