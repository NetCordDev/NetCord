using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

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
    [DoNotParallelize]
    [TestMethod]
    [DataRow("libdave;libsodium;opus;zstd")]
    public void NativeAotStaticLinking(string libName)
    {
        var projectDirectory = Path.Combine(AppContext.BaseDirectory, "Assets", "NativeAotApp");
        var projectFile = Path.Combine(projectDirectory, "NativeAotApp.csproj");
        var generatedProjectFile = Path.Combine(projectDirectory, "NativeAotApp.g.csproj");

        // get properties to be passed to the NativeAotApp build from AssemblyMetadata attribute
        var assembly = typeof(NativesBuildTests).Assembly;

        var properties = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()?
                                 .FirstOrDefault(a => a.Key == "NativeAotAppProps")?
                                 .Value?.Split([';', ','], StringSplitOptions.RemoveEmptyEntries).Select(p => $"-p:{p.Trim()}");
        Assert.IsNotNull(properties, "NativeAotAppProps metadata attribute is not defined.");

        var binlogpath = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()?
                                .FirstOrDefault(a => a.Key == "NativeAotBinLogPath")?.Value;

        var configuration = assembly.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration ?? "Debug";

        // build asset NativeAotApp with Native AoT enabled
        var buildProcess = new System.Diagnostics.Process();
        buildProcess.StartInfo.FileName = "dotnet";
        buildProcess.StartInfo.ArgumentList.Add("publish");
        buildProcess.StartInfo.ArgumentList.Add("NativeAotApp.csproj");
        buildProcess.StartInfo.ArgumentList.Add("-c");
        buildProcess.StartInfo.ArgumentList.Add(configuration);
        buildProcess.StartInfo.ArgumentList.Add("-tl:off");
        buildProcess.StartInfo.ArgumentList.Add("-v:n");
        buildProcess.StartInfo.ArgumentList.Add("-nodeReuse:false");

        if (!string.IsNullOrEmpty(binlogpath))
            buildProcess.StartInfo.ArgumentList.Add($"-bl:{binlogpath}");
        foreach (var property in properties)
            buildProcess.StartInfo.ArgumentList.Add(property);

        Console.WriteLine($"[{NativeAotAppLogTag}/Publish] Building Native AoT app in ({projectDirectory}): 'dotnet {buildProcess.StartInfo.ArgumentList.Aggregate((a, b) => $"{a} {b}")}'");

        buildProcess.StartInfo.WorkingDirectory = projectDirectory;
        buildProcess.StartInfo.RedirectStandardOutput = true;
        buildProcess.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"[{NativeAotAppLogTag}/Publish] {e.Data}");
        };
        buildProcess.StartInfo.RedirectStandardError = true;
        buildProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.Error.WriteLine($"[{NativeAotAppLogTag}/Publish] {e.Data}");
        };
        var ok = buildProcess.Start();
        buildProcess.BeginOutputReadLine();
        buildProcess.BeginErrorReadLine();
        buildProcess.WaitForExit();

        Assert.AreEqual(0, buildProcess.ExitCode, $"Native AoT build failed for '{libName}'.");

        // Obtain the generated RunCommand from a build so we launch the same command the SDK would.
        var getRunCmd = new System.Diagnostics.Process();
        getRunCmd.StartInfo.FileName = "dotnet";
        getRunCmd.StartInfo.ArgumentList.Add("build");
        getRunCmd.StartInfo.ArgumentList.Add("NativeAotApp.csproj");
        getRunCmd.StartInfo.ArgumentList.Add("-c");
        getRunCmd.StartInfo.ArgumentList.Add(configuration);
        getRunCmd.StartInfo.ArgumentList.Add("-t:GetTargetPath");
        getRunCmd.StartInfo.ArgumentList.Add("-getProperty:PublishDir");
        getRunCmd.StartInfo.ArgumentList.Add("--no-restore");
        getRunCmd.StartInfo.ArgumentList.Add("-nodeReuse:false");

        foreach (var property in properties)
            getRunCmd.StartInfo.ArgumentList.Add(property);

        string? runCmdOutput = null;

        getRunCmd.StartInfo.WorkingDirectory = projectDirectory;
        getRunCmd.StartInfo.RedirectStandardOutput = true;
        getRunCmd.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"[{NativeAotAppLogTag}/GetRunCmd] {e.Data}");
                runCmdOutput = e.Data.Trim('\r', '\n', '"');
            }
        };
        getRunCmd.StartInfo.RedirectStandardError = true;
        getRunCmd.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.Error.WriteLine($"[{NativeAotAppLogTag}/GetRunCmd] {e.Data}");
        };
        getRunCmd.Start();
        getRunCmd.BeginErrorReadLine();
        getRunCmd.BeginOutputReadLine();
        getRunCmd.WaitForExit();

        Assert.AreEqual(0, getRunCmd.ExitCode, $"Failed to obtain PublishDir for '{libName}'.");
        Assert.IsFalse(string.IsNullOrEmpty(runCmdOutput), $"PublishDir is empty for '{libName}'.");

        runCmdOutput = Path.Combine(projectDirectory, runCmdOutput,
                        "NativeAotApp" + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : ""));

        // check that the library is running without errors, which indicates that it was statically linked successfully
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
        aotProcess.WaitForExit();

        Assert.AreEqual(0, aotProcess.ExitCode, $"Native AoT app failed to run for '{libName}'.");
    }
}
