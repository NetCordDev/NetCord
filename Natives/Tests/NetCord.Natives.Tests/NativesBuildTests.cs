using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Locator;
using Microsoft.Build.Logging;

namespace NetCord.Natives.Tests;

[TestClass]
public class NativesBuildTests
{
    [TestMethod]
    [DataRow("libdave")]
    [DataRow("libsodium")]
    [DataRow("opus")]
    [DataRow("zstd")]
    public void NativesLoaded(string libName)
    {
        _ = libName switch
        {
            "libdave" => NativeProbes.DaveMaxSupportedProtocolVersion(),
            "libsodium" => NativeProbes.SodiumInit(),
            "opus" => NativeProbes.OpusGetVersionString(),
            "zstd" => (object)NativeProbes.ZstdVersionNumber(),
            _ => throw new InvalidOperationException($"Unknown library name '{libName}' provided to test."),
        };
    }

    [TestMethod]
    [OSCondition(ConditionMode.Exclude, OperatingSystems.OSX)]
    [DataRow("libdave")]
    [DataRow("libsodium")]
    [DataRow("opus")]
    [DataRow("zstd")]
    public void AllLibraryImportsExistInBinary(string libName)
    {
        var libHandle = NativeProbes.GetLoadedNativeModuleHandle(libName);
        var missingExports = NativeProbes.GetMissingLibraryImports(libHandle, libName);

        Assert.IsEmpty(missingExports, $"The following entry points were not found in '{libName}': {string.Join(", ", missingExports)}");
    }

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        // This is guaranteed to run before any test method
        MSBuildLocator.RegisterDefaults();
    }

    const string NativeAotAppLogTag = nameof(NativeAotStaticLinking);

    [TestMethod]
    [DoNotParallelize]
    [DataRow("libdave;libsodium;opus;zstd")]
    public void NativeAotStaticLinking(string libNames)
    {
        MSBuildNativeAotPublish(libNames, false);
    }

    [TestMethod]
    [DoNotParallelize]
    [DataRow("libdave;libsodium;opus;zstd")]
    public void NativeAotStaticLinking_WithPackageVer(string libNames)
    {
        MSBuildNativeAotPublish(libNames, true);
    }

    private static void MSBuildNativeAotPublish(string libNames, bool testWithPackageVer)
    {
        // 1. Get properties from AssemblyMetadata attributes
        var assembly = typeof(NativesBuildTests).Assembly;

        var projectDirectory = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                                       .FirstOrDefault(a => a.Key == "NativeAotAppDir")?.Value;
        Assert.IsNotNull(projectDirectory, "NativeAotAppDir metadata attribute is not defined.");

        var projectFile = Path.Combine(projectDirectory, "NativeAotApp.csproj");

        var propsAttr = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                                .FirstOrDefault(a => a.Key == "NativeAotAppProps")?.Value;

        var binlogpath = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                                 .FirstOrDefault(a => a.Key == "NativeAotBinLogPath")?.Value;

        var configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration ?? "Debug";

        // 2. Parse MSBuild Global Properties into a Dictionary
        var globalProperties = new Dictionary<string, string>
        {
            { "Configuration", configuration }
        };

        var pairs = propsAttr?.Split([';', ','], StringSplitOptions.RemoveEmptyEntries) ?? [];
        foreach (var pair in pairs)
        {
            var kvp = pair.Split('=');
            if (kvp.Length == 2)
            {
                globalProperties[kvp[0].Trim()] = kvp[1].Trim();
            }
        }

        // 3. Set up Loggers (Console + optional Binary Logger)
        var loggers = new List<ILogger>
        {
            new ConsoleLogger(Enum.Parse<LoggerVerbosity>(Environment.GetEnvironmentVariable("DotnetVerbose") ?? "Minimal", true),
                msg => Console.WriteLine($"[{NativeAotAppLogTag}/Publish] {msg.Trim()}"), null, null)
        };

        if (!string.IsNullOrEmpty(binlogpath))
        {
            var withverbinlogpath = Path.GetFileNameWithoutExtension(binlogpath) + (testWithPackageVer ? "_with_pkg_ver" : "") + Path.GetExtension(binlogpath);
            loggers.Add(new BinaryLogger { Parameters = testWithPackageVer ? withverbinlogpath : binlogpath });
        }

        var buildParameters = new BuildParameters
        {
            Loggers = loggers,
            EnableNodeReuse = false,
        };

        if (testWithPackageVer)
            // Get the version of NetCord.Natives from the project file
            using (var prjVerCollection = new ProjectCollection())
            {
                var prjPath = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                                    .FirstOrDefault(a => a.Key == "NetCordNativesPath")?.Value;
                Assert.IsNotNull(prjPath, "NetCordNativesPath metadata attribute is not defined.");

                var prjVerInstance = new ProjectInstance(prjPath, globalProperties, null, prjVerCollection);
                var prjVerRequest = new BuildRequestData(prjVerInstance, ["GetAssemblyVersion"]);

                var prjVerResult = BuildManager.DefaultBuildManager.Build(buildParameters, prjVerRequest);
                Assert.AreEqual(BuildResultCode.Success, prjVerResult.OverallResult, $"Failed to get NetCord.Natives version from '{prjPath}'.");

                var prjVer = prjVerInstance.GetPropertyValue("Version");
                globalProperties["TestWithPackageVer"] = prjVer;

                Console.WriteLine($"[{NativeAotAppLogTag}/Publish] Using NetCord.Natives version '{prjVer}' for PackageReference testing.");
            }

        // Clear internal MSBuild engine caches
        BuildManager.DefaultBuildManager.ResetCaches();

        // Evaluate and Execute the "Restore" target first
        Console.WriteLine($"[{NativeAotAppLogTag}/Publish] Restoring Native AoT app dependencies...");

        using (var restoreCollection = new ProjectCollection())
        {
            var restoreInstance = new ProjectInstance(projectFile, globalProperties, null, restoreCollection);
            var restoreRequest = new BuildRequestData(restoreInstance, ["Restore"]);

            var restoreResult = BuildManager.DefaultBuildManager.Build(buildParameters, restoreRequest);
            Assert.AreEqual(BuildResultCode.Success, restoreResult.OverallResult, $"Native AoT restore failed for '{libNames}'.");
        }

        // Clear internal MSBuild engine caches so the next build sees the restored assets
        BuildManager.DefaultBuildManager.ResetCaches();

        // Evaluate and Execute the "Publish" target programmatically
        Console.WriteLine($"[{NativeAotAppLogTag}/Publish] Building and Publishing Native AoT app in ({projectDirectory}) via MSBuild API...");

        // We use a fresh collection so it reads the newly generated project.assets.json from disk
        using var publishCollection = new ProjectCollection();
        var publishInstance = new ProjectInstance(projectFile, globalProperties, null, publishCollection);
        var publishRequest = new BuildRequestData(publishInstance, ["Publish"]);

        var buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, publishRequest);

        // Assert build success
        Assert.AreEqual(BuildResultCode.Success, buildResult.OverallResult, $"Native AoT build failed for '{libNames}'.");

        // Instantly extract the 'PublishDir' property from the freshly evaluated project state
        var publishDirProperty = publishInstance.GetPropertyValue("PublishDir");
        Assert.IsFalse(string.IsNullOrEmpty(publishDirProperty), $"PublishDir is empty for '{libNames}'.");

        // sanitize back-slash
        publishDirProperty = publishDirProperty.Replace('\\', Path.DirectorySeparatorChar);

        var runCmdOutput = Path.GetFullPath(Path.Combine(projectDirectory, publishDirProperty,
            "NativeAotApp" + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "")));

        Console.WriteLine($"[{NativeAotAppLogTag}/Publish] Native AoT app published to: '{runCmdOutput}'");

        // Execute the generated Native AOT Binary
        var aotProcess = new System.Diagnostics.Process();
        aotProcess.StartInfo.FileName = runCmdOutput;
        aotProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(runCmdOutput);

        Console.WriteLine($"[{NativeAotAppLogTag}/Run] Running Native AoT app: '{runCmdOutput}'");

        aotProcess.StartInfo.RedirectStandardOutput = true;
        aotProcess.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"[{NativeAotAppLogTag}/Run] {e.Data}");
        };
        aotProcess.StartInfo.RedirectStandardError = true;
        aotProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.Error.WriteLine($"[{NativeAotAppLogTag}/Run] {e.Data}");
        };

        aotProcess.Start();
        aotProcess.BeginOutputReadLine();
        aotProcess.BeginErrorReadLine();

        if (!aotProcess.HasExited)
            aotProcess.WaitForExit();

        Assert.AreEqual(0, aotProcess.ExitCode, $"Native AoT app failed to run for '{libNames}'.");
    }
}
