using Xunit;

namespace Wolfgang.Extensions.Mail.Tests.Integration;


public sealed class TempDirectoryTests
{

    [Fact]
    public void DeleteQuietly_when_directory_exists_deletes_it()
    {
        var path = Path.Combine(Path.GetTempPath(), $"mailext-td-{Guid.NewGuid():N}");
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, "a.txt"), "a");

        TempDirectory.DeleteQuietly(path);

        Assert.False(Directory.Exists(path));
    }



    [Fact]
    public void DeleteQuietly_when_Directory_Delete_throws_IOException_swallows_it()
    {
        // Directory.Delete on a path that is a file, not a directory, throws IOException.
        var file = Path.GetTempFileName();
        try
        {
            var exception = Record.Exception(() => TempDirectory.DeleteQuietly(file));

            Assert.Null(exception);
        }
        finally
        {
            File.Delete(file);
        }
    }
}
