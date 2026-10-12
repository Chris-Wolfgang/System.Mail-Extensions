namespace Wolfgang.Extensions.Mail.Tests.Integration;


/// <summary>
/// Cleanup for the per-test temporary directories.
/// </summary>
internal static class TempDirectory
{

    /// <summary>
    /// Deletes <paramref name="path"/> recursively. Best-effort: an
    /// <see cref="IOException"/> (e.g. a file still open) is swallowed, since a
    /// leaked temporary directory is harmless.
    /// </summary>
    internal static void DeleteQuietly(string path)
    {
        try
        {
            Directory.Delete(path, recursive: true);
        }
        catch (IOException)
        {
            // Best-effort cleanup; leaked temp directories are harmless.
        }
    }
}
