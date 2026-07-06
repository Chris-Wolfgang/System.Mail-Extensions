namespace Wolfgang.Extensions.Mail.Tests.Unit;


/// <summary>
/// Shared helpers for tests that create real files on disk.
/// </summary>
internal static class TestFileHelpers
{

    /// <summary>
    /// Deletes temp files without letting a cleanup problem (antivirus lock,
    /// slow handle release) throw from a <c>finally</c> block and mask the
    /// actual test outcome. Leaked temp files are harmless; a masked
    /// assertion failure is not.
    /// </summary>
    internal static void BestEffortDelete
    (
        params string[] paths
    )
    {
        foreach (var path in paths)
        {
            try
            {
                File.Delete(path);
            }
            catch (IOException)
            {
                // Best-effort cleanup only.
            }
            catch (UnauthorizedAccessException)
            {
                // Best-effort cleanup only.
            }
        }
    }
}
