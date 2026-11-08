// Compiles every <example><code> block from the library's XML doc comments to
// catch documentation rot — an example that calls a renamed/removed member no
// longer compiles and fails this test. Scoped to net8+: the Roslyn 5.x that
// resolves the library's C# 14 extension members targets net8.0, and the
// examples don't vary by TFM, so running on net8/9/10 is sufficient.
#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Assert = Xunit.Assert;

#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;


public class DocExampleCompilationTests
{

    // Usings a copy-pasted example would need. The example bodies run inside an
    // async method, so `await` in an example is valid.
    private const string Preamble =
        "using System;\n" +
        "using System.IO;\n" +
        "using System.Net.Mail;\n" +
        "using System.Net.Mime;\n" +
        "using System.Threading;\n" +
        "using System.Threading.Tasks;\n" +
        "using Wolfgang.Extensions.Mail;\n" +
        "using Wolfgang.Extensions.Mail.Validation;\n";



    [Fact]
    public void Every_xml_doc_example_compiles()
    {
        var references = BuildReferences();
        var options = CompilationOptions();

        var examples = ExtractExamples().ToList();
        Assert.NotEmpty(examples);   // guard against a broken extractor silently passing

        var failures = examples
            .Select(example => CompileExample(example.File, example.Ordinal, example.Code, references, options))
            .Where(failure => failure != null)
            .ToList();

        Assert.True
        (
            failures.Count == 0,
            "XML-doc <example> blocks failed to compile (documentation rot):\n" +
                string.Join("\n", failures)
        );
    }



    [Fact]
    public void CompileExample_when_example_calls_a_missing_member_reports_the_error()
    {
        // The guard must be able to fail: an example calling a member that does not exist
        // has to come back as a failure naming the file, the example and the compiler error.
        var failure = CompileExample
        (
            "Sample.cs",
            7,
            "EmlParser.ParseTheWholeInternet();",
            BuildReferences(),
            CompilationOptions()
        );

        Assert.Equal
        (
            "Sample.cs (example #7): CS0117: 'EmlParser' does not contain a definition for 'ParseTheWholeInternet'",
            failure
        );
    }



    private static CSharpCompilationOptions CompilationOptions()
    {
        return new CSharpCompilationOptions
        (
            OutputKind.DynamicallyLinkedLibrary,
            allowUnsafe: false,
            nullableContextOptions: NullableContextOptions.Disable
        );
    }



    /// <summary>
    /// Compiles one example inside an async method and returns <c>null</c> when it compiles,
    /// or a one-line failure naming the file, the example ordinal and every compiler error.
    /// </summary>
    private static string? CompileExample
    (
        string file,
        int ordinal,
        string code,
        IEnumerable<MetadataReference> references,
        CSharpCompilationOptions options
    )
    {
        var source = Preamble +
            $"static class DocExample_{ordinal}\n" +
            "{\n" +
            "    static async System.Threading.Tasks.Task Run()\n" +
            "    {\n" +
            code + "\n" +
            "        await System.Threading.Tasks.Task.CompletedTask;\n" +
            "    }\n" +
            "}\n";

        var tree = CSharpSyntaxTree.ParseText
        (
            source,
            new CSharpParseOptions(LanguageVersion.Latest)
        );

        var compilation = CSharpCompilation.Create
        (
            $"DocExamples_{ordinal}",
            new[] { tree },
            references,
            options
        );

        var errors = compilation
            .GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(e => $"{e.Id}: {e.GetMessage(System.Globalization.CultureInfo.InvariantCulture)}")
            .ToList();

        return errors.Count == 0
            ? null
            : $"{Path.GetFileName(file)} (example #{ordinal}): {string.Join("; ", errors)}";
    }



    private static List<MetadataReference> BuildReferences()
    {
        var tpa = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty)
            .Split(Path.PathSeparator);

        var references = tpa
            .Where(p => !string.IsNullOrEmpty(p) && File.Exists(p))
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToList();

        // The library under documentation is not part of the framework TPA set.
        references.Add(MetadataReference.CreateFromFile(typeof(EmlParser).Assembly.Location));

        return references;
    }



    private static IEnumerable<(string File, int Ordinal, string Code)> ExtractExamples()
    {
        var ordinal = 0;
        foreach (var file in EnumerateSourceFiles())
        {
            foreach (var code in ExtractCodeBlocks(File.ReadAllLines(file)))
            {
                yield return (file, ordinal++, code);
            }
        }
    }



    private static IEnumerable<string> ExtractCodeBlocks
    (
        string[] lines
    )
    {
        var current = new StringBuilder();
        var inExample = false;
        var inCode = false;

        foreach (var raw in lines)
        {
            var trimmed = raw.TrimStart();
            if (!trimmed.StartsWith("///", StringComparison.Ordinal))
            {
                continue;
            }

            var content = StripDocPrefix(trimmed);

            if (content.Contains("<example>", StringComparison.Ordinal))
            {
                inExample = true;
            }

            if (content.Contains("</example>", StringComparison.Ordinal))
            {
                inExample = false;
            }

            // Only <code> nested inside <example> is a runnable snippet. Inline
            // <code>…</code> in <summary>/<remarks> is prose, not a statement
            // body, and would produce spurious compile failures if collected.
            if (inExample && content.Contains("<code>", StringComparison.Ordinal))
            {
                inCode = true;
                current.Clear();
                continue;
            }

            if (content.Contains("</code>", StringComparison.Ordinal))
            {
                inCode = false;
                yield return current.ToString();
                continue;
            }

            if (inCode)
            {
                current.AppendLine(Unescape(content));
            }
        }
    }



    private static string StripDocPrefix
    (
        string trimmedLine
    )
    {
        var withoutSlashes = trimmedLine.Substring(3);   // drop the leading "///"
        return withoutSlashes.StartsWith(" ", StringComparison.Ordinal)
            ? withoutSlashes.Substring(1)
            : withoutSlashes;
    }



    private static string Unescape
    (
        string line
    )
    {
        return line
            .Replace("&lt;", "<", StringComparison.Ordinal)
            .Replace("&gt;", ">", StringComparison.Ordinal)
            .Replace("&quot;", "\"", StringComparison.Ordinal)
            .Replace("&apos;", "'", StringComparison.Ordinal)
            .Replace("&amp;", "&", StringComparison.Ordinal);
    }



    private static IEnumerable<string> EnumerateSourceFiles()
    {
        var sourceDirectory = SourceDirectory(AppContext.BaseDirectory, Path.Combine("src", "Wolfgang.Extensions.Mail"));
        // Ordinal sort so example numbering — and thus the failure output — is
        // stable across operating systems and filesystems, which don't agree on
        // Directory.EnumerateFiles ordering.
        return Directory
            .EnumerateFiles(sourceDirectory, "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                     && !p.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .OrderBy(p => p, StringComparer.Ordinal);
    }



    [Fact]
    public void SourceDirectory_when_no_ancestor_contains_the_library_throws_DirectoryNotFoundException()
    {
        // A relative path no checkout can contain, so the walk always reaches the root.
        var missing = Path.Combine("src", $"missing-{Guid.NewGuid():N}");

        var exception = Assert.Throws<DirectoryNotFoundException>(() => SourceDirectory(AppContext.BaseDirectory, missing));

        Assert.Contains(missing, exception.Message, StringComparison.Ordinal);
    }



    internal static string SourceDirectory(string startDirectory, string relativePath)
    {
        // Walk up from the test assembly to the repo root (the ancestor that
        // contains src/Wolfgang.Extensions.Mail). [CallerFilePath] can't be used
        // here: CI maps source paths deterministically (ContinuousIntegrationBuild
        // rewrites them to "/_/…"), so the compile-time path does not exist on the
        // test runner and enumeration throws DirectoryNotFoundException.
        for (var dir = new DirectoryInfo(startDirectory); dir != null; dir = dir.Parent)
        {
            var candidate = Path.Combine(dir.FullName, relativePath);
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException
        (
            $"Could not locate {relativePath} by walking up from {startDirectory}."
        );
    }
}

#endif
