window.BENCHMARK_DATA = {
  "lastUpdate": 1786724402403,
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
      }
    ]
  }
}