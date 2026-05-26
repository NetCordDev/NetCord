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
            "libdave" => (object)NativeProbes.DaveMaxSupportedProtocolVersion(),
            "libsodium" => (object)NativeProbes.SodiumInit(),
            "opus" => (object)NativeProbes.OpusGetVersionString(),
            "zstd" => (object)NativeProbes.ZstdVersionNumber(),
            _ => throw new InvalidOperationException($"Unknown library name '{libName}' provided to test."),
        };
    }

    [TestMethod]
    [OSCondition(ConditionMode.Exclude, OperatingSystems.OSX)]
    [DataRow("libdave", "NetCord.Gateway.Voice.Dave")]
    [DataRow("libsodium", "NetCord.Gateway.Voice.Encryption.XChaCha20Poly1305")]
    [DataRow("opus", "NetCord.Gateway.Voice.Opus")]
    [DataRow("zstd", "NetCord.Gateway.Compression.Zstandard")]
    public void AllLibraryImportsExistInBinary(string libName, string className)
    {
        var missingExports = NativeProbes.GetMissingLibraryImports(libName, className, typeof(NetCord.Application).Assembly);

        Assert.IsEmpty(missingExports, $"The following entry points were not found in '{libName}': {string.Join(", ", missingExports)}");
    }

    const string NativeAotAppLogTag = $"[{nameof(NativeAotStaticLinking)}]";
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
                                 .FirstOrDefault(a => a.Key == "NativeAotAppProps")?.Value;
        Assert.IsNotNull(properties, "NativeAotAppProps metadata attribute is not defined.");

        // build asset NativeAotApp with Native AoT enabled
        var buildProcess = new System.Diagnostics.Process();
        buildProcess.StartInfo.FileName = "dotnet";
        buildProcess.StartInfo.ArgumentList.Add("publish");
        buildProcess.StartInfo.ArgumentList.Add("NativeAotApp.csproj");
        buildProcess.StartInfo.ArgumentList.Add("-tl:off");
        buildProcess.StartInfo.ArgumentList.Add("-v:n");
        buildProcess.StartInfo.ArgumentList.Add($"-p:{properties}");
        
        Console.WriteLine($"{NativeAotAppLogTag} Building Native AoT app in ({projectDirectory}): 'dotnet {buildProcess.StartInfo.ArgumentList.Aggregate((a, b) => $"{a} {b}")}'");

        buildProcess.StartInfo.WorkingDirectory = projectDirectory;
        buildProcess.StartInfo.RedirectStandardOutput = true;
        buildProcess.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"{NativeAotAppLogTag} {e.Data}");
        };
        buildProcess.StartInfo.RedirectStandardError = true;
        buildProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"{NativeAotAppLogTag} {e.Data}");
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
        getRunCmd.StartInfo.ArgumentList.Add($"-p:{properties}");
        getRunCmd.StartInfo.ArgumentList.Add("-t:GetTargetPath");
        getRunCmd.StartInfo.ArgumentList.Add("-getProperty:PublishDir");
        getRunCmd.StartInfo.ArgumentList.Add("--no-restore");
        
        string? runCmdOutput = null;

        getRunCmd.StartInfo.WorkingDirectory = projectDirectory;
        getRunCmd.StartInfo.RedirectStandardOutput = true;
        getRunCmd.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"{NativeAotAppLogTag} {e.Data}");
                runCmdOutput = e.Data.Trim('\r', '\n', '"');
            }
        };
        getRunCmd.StartInfo.RedirectStandardError = true;
        getRunCmd.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"{NativeAotAppLogTag} {e.Data}");
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

        Console.WriteLine($"{NativeAotAppLogTag} Running Native AoT app: '{runCmdOutput}'");

        aotProcess.StartInfo.RedirectStandardOutput = true;
        aotProcess.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"{NativeAotAppLogTag} {e.Data}");
        };
        aotProcess.StartInfo.RedirectStandardError = true;
        aotProcess.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                Console.WriteLine($"{NativeAotAppLogTag} {e.Data}");
        };
        aotProcess.Start();
        aotProcess.BeginOutputReadLine();
        aotProcess.BeginErrorReadLine();
        aotProcess.WaitForExit();

        Assert.AreEqual(0, aotProcess.ExitCode, $"Native AoT app failed to run for '{libName}'.");
    }
}
