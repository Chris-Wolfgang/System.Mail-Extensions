using System;
using System.IO;
using System.Runtime.CompilerServices;
using VerifyTests;
using VerifyXunit;

namespace Wolfgang.Extensions.Mail.Tests.Snapshots;

/// <summary>
/// Module-wide Verify configuration: every snapshot lives under <c>Snapshots/</c> in this
/// project, named <c>{Type}.{Method}.verified.*</c>.
/// </summary>
public static class SnapshotSettings
{

    /// <summary>
    /// Points Verify at the <c>Snapshots</c> folder of this project.
    /// </summary>
    /// <remarks>
    /// The project directory is found from <see cref="AppContext.BaseDirectory"/>, not from
    /// Verify's own <c>[CallerFilePath]</c> capture: CI builds set ContinuousIntegrationBuild,
    /// whose path map rewrites embedded source paths to <c>/_/...</c>, so the captured path
    /// does not exist on the build machine (the failure Etl-DbClient fixed in its #325).
    /// </remarks>
    [ModuleInitializer]
    public static void Initialize()
    {
        var snapshotDirectory = Path.Combine(ResolveProjectDirectory(), "Snapshots");

        Verifier.DerivePathInfo
        (
            (_, _, type, method) => new PathInfo
            (
                directory: snapshotDirectory,
                typeName: type.Name,
                methodName: method.Name
            )
        );
    }



    private static string ResolveProjectDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory.GetFiles("*.csproj").Length == 0)
        {
            directory = directory.Parent!;
        }

        return directory.FullName;
    }
}
