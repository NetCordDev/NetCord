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

    const string NativeAotAppLogTag = nameof(NativeAotStaticLinking);
    [TestMethod]
    [DataRow("libdave;libsodium;opus;zstd")]
    public void NativeAotStaticLinking(string libNames)
    {
        MSBuildLocator.RegisterDefaults();
        MSBuildNativeAotPublish(libNames);
    }

    private static void MSBuildNativeAotPublish(string libNames)
    {
        var projectDirectory = Path.Combine(AppContext.BaseDirectory, "Assets", "NativeAotApp");
        var projectFile = Path.Combine(projectDirectory, "NativeAotApp.csproj");

        // 1. Get properties from AssemblyMetadata attributes
        var assembly = typeof(NativesBuildTests).Assembly;

        var propsAttr = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                                .FirstOrDefault(a => a.Key == "NativeAotAppProps")?.Value;
        Assert.IsNotNull(propsAttr, "NativeAotAppProps metadata attribute is not defined.");

        var binlogpath = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                                 .FirstOrDefault(a => a.Key == "NativeAotBinLogPath")?.Value;

        var configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration ?? "Debug";

        // 2. Parse MSBuild Global Properties into a Dictionary
        var globalProperties = new Dictionary<string, string>
        {
            { "Configuration", configuration }
        };

        var pairs = propsAttr.Split([';', ','], StringSplitOptions.RemoveEmptyEntries);
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
            new ConsoleLogger(LoggerVerbosity.Normal, msg => Console.WriteLine($"[{NativeAotAppLogTag}/Publish] {msg.Trim()}"), null, null)
        };

        if (!string.IsNullOrEmpty(binlogpath))
        {
            loggers.Add(new BinaryLogger { Parameters = binlogpath });
        }

        // 4. Evaluate and Execute the "Publish" target programmatically
        Console.WriteLine($"[{NativeAotAppLogTag}/Publish] Building Native AoT app in ({projectDirectory}) via MSBuild API...");
        
        using var projectCollection = new ProjectCollection();
        var projectInstance = new ProjectInstance(projectFile, globalProperties, null, projectCollection);
        
        var buildParameters = new BuildParameters 
        { 
            Loggers = loggers,
            EnableNodeReuse = false,
        };
        var buildRequest = new BuildRequestData(projectInstance, ["Publish"]);

        var buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest);

        // Assert build success
        Assert.AreEqual(BuildResultCode.Success, buildResult.OverallResult, $"Native AoT build failed for '{libNames}'.");

        // 5. Instantly extract the 'PublishDir' property from the evaluated project state
        // (This replaces the entire second "dotnet build -getProperty" process call!)
        var publishDirProperty = projectInstance.GetPropertyValue("PublishDir");
        Assert.IsFalse(string.IsNullOrEmpty(publishDirProperty), $"PublishDir is empty for '{libNames}'.");

        var runCmdOutput = Path.GetFullPath(Path.Combine(projectDirectory, publishDirProperty, 
            "NativeAotApp" + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : "")));

        // 6. Assertions on copied native binaries
        var libs = libNames.Split([';', ','], StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim());
        var deps = new List<string>();

        foreach (var lib in libs)
        {
            switch (lib)
            {
                case "libdave":
                    deps.AddRange([lib, "crypto"]);
                    break;
                case "libsodium":
                case "opus":
                case "zstd":
                    deps.Add(lib);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown library name '{lib}' provided to test.");
            }
        }

        var copiedDlls = Directory.GetFiles(Path.GetDirectoryName(runCmdOutput) ?? string.Empty, 
                                            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "*.dll" : 
                                            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "lib*.so" : "lib*.dylib",
                                            SearchOption.TopDirectoryOnly)
                                  .Select(Path.GetFileName);
        
        var matchCopied = copiedDlls.Where(dll => dll != null && deps.Any(lib => 
                                        dll.StartsWith(lib, StringComparison.OrdinalIgnoreCase) || 
                                        dll.StartsWith($"lib{lib}", StringComparison.OrdinalIgnoreCase)))
                                    .ToList();

        Assert.IsEmpty(matchCopied, $"These should've not been copied to the publish output directory: {string.Join(", ", matchCopied)}");

        // 7. Execute the generated Native AOT Binary
        // NOTE: Running the compiled native application still requires System.Diagnostics.Process
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
