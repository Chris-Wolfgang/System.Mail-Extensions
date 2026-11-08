using System;

namespace Wolfgang.Extensions.Mail.Tests.Fuzz;

/// <summary>
/// Run-length settings shared by every property, read from the environment so the same
/// properties serve as the quick per-PR pass and as the long scheduled run.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item><c>FUZZ_ITERATIONS</c>: cases per property (default 500, so a plain <c>dotnet test</c>
/// finishes in seconds).</item>
/// <item><c>FUZZ_TIME_SECONDS</c>: time budget per property in seconds; when set it replaces the
/// iteration count (CsCheck runs until the budget is spent).</item>
/// <item><c>FUZZ_SEED</c>: replays one recorded failure. CsCheck prints the seed of the smallest
/// failing case; set it here to reproduce that case deterministically.</item>
/// </list>
/// </remarks>
internal static class FuzzSettings
{

    public static long Iterations => ReadPositive("FUZZ_ITERATIONS", 500);



    public static int TimeSeconds => (int)ReadPositive("FUZZ_TIME_SECONDS", -1);



    public static string? Seed => Environment.GetEnvironmentVariable("FUZZ_SEED") is { Length: > 0 } seed ? seed : null;



    private static long ReadPositive(string name, long fallback)
    {
        return long.TryParse(Environment.GetEnvironmentVariable(name), out var value) && value > 0 ? value : fallback;
    }
}
