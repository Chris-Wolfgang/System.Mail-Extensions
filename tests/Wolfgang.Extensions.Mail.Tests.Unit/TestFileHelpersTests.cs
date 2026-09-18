using Xunit;
using Assert = Xunit.Assert;
#pragma warning disable CA1707

namespace Wolfgang.Extensions.Mail.Tests.Unit;

public class TestFileHelpersTests
{
    [Fact]
    public void BestEffortDelete_when_paths_is_null_does_nothing()
    {
        var exception = Record.Exception(() => TestFileHelpers.BestEffortDelete(null));

        Assert.Null(exception);
    }



    [Fact]
    public void BestEffortDelete_when_path_cannot_be_deleted_swallows_the_error()
    {
        // File.Delete on a directory throws (UnauthorizedAccessException on every
        // platform); the helper must not let that escape a finally block.
        var directory = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        try
        {
            var exception = Record.Exception(() => TestFileHelpers.BestEffortDelete(directory, "", null!));

            Assert.Null(exception);
            Assert.True(Directory.Exists(directory));
        }
        finally
        {
            Directory.Delete(directory);
        }
    }
}
