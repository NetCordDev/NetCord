using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NetCord.Natives.Tests;

[TestClass]
public class NativesBuildTests
{
    // MSTest automatically injects this property
    public TestContext TestContext { get; set; }
    
    [TestMethod]
    [DataRow("libdave")]
    [DataRow("libsodium")]
    [DataRow("opus")]
    [DataRow("zstd")]
    public void NativesLoaded(string libName)
    {
        try
        {
            NativeLibrary.Load(libName);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Failed to load library '{libName}': {ex}");
        }
    }

    [TestMethod]
    [DataRow("libdave", "NetCord.Gateway.Voice.Dave")]
    [DataRow("libsodium", "NetCord.Gateway.Voice.Encryption.XChaCha20Poly1305")]
    [DataRow("opus", "NetCord.Gateway.Voice.Opus")]
    [DataRow("zstd", "NetCord.Gateway.Compression.Zstandard")]
    public void AllLibraryImportsExistInBinary(string libName, string className)
    {
        IntPtr libHandle = IntPtr.Zero;

        try
        {
            var assembly = typeof(NetCord.Application).Assembly;
            var typeWithImports = assembly.GetType(className, true);

            libHandle = NativeLibrary.Load(libName);

            var missingExports = new List<string>();

            // Reflect over methods with [LibraryImport]
            var methods = typeWithImports!.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Select(m => new { 
                    MethodName = m.Name, 
                    Attr = m.GetCustomAttribute<LibraryImportAttribute>() 
                })
                .Where(x => x.Attr != null);
            Assert.IsNotEmpty(methods, $"No methods with [LibraryImport] found in '{className}'.");

            foreach (var item in methods)
            {
                // Use EntryPoint if defined, otherwise fallback to Method Name
                string exportName = item.Attr!.EntryPoint ?? item.MethodName;

                if (!NativeLibrary.TryGetExport(libHandle, exportName, out _))
                {
                    missingExports.Add(exportName);
                }
            }
            Assert.IsEmpty(missingExports, $"The following entry points were not found in '{libName}': {string.Join(", ", missingExports)}");
        }
        catch (Exception ex)
        {
            Assert.Fail($"An error occurred while verifying imports for '{libName}': {ex}");
        }
        finally
        {
            NativeLibrary.Free(libHandle);
        }
    }

    [TestMethod]
    [DataRow("libdave;libsodium;opus;zstd")]
    public void NativeAotStaticLinking(string libName)
    {
        var projectDirectory = Path.Combine(AppContext.BaseDirectory, "Assets", "NativeAotApp");
        var projectFile = Path.Combine(projectDirectory, "NativeAotApp.csproj");
        var generatedProjectFile = Path.Combine(projectDirectory, "NativeAotApp.g.csproj");

        try
        {
            var originalProjectFileContents = File.ReadAllText(projectFile);
            var generatedProjectFileContents = originalProjectFileContents.Replace("$(NetCordDirectPInvoke)", libName);
            File.WriteAllText(generatedProjectFile, generatedProjectFileContents);

            // get NetCordNativesDir from AssemblyMetadata attribute
            var assembly = typeof(NativesBuildTests).Assembly;
            var nativesDir = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()?
                                     .FirstOrDefault(a => a.Key == "NetCordNativesDir")?.Value;
            Assert.IsNotNull(nativesDir, "NetCordNativesDir metadata attribute is not defined.");
            var vcpkgRoot = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()?
                                    .FirstOrDefault(a => a.Key == "VcpkgRoot")?.Value;
            Assert.IsNotNull(vcpkgRoot, "VcpkgRoot metadata attribute is not defined.");
            var targetFramework = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()?
                                    .FirstOrDefault(a => a.Key == "TargetFramework")?.Value;
            Assert.IsNotNull(targetFramework, "TargetFramework metadata attribute is not defined.");

            // build asset NativeAotApp with Native AoT enabled
            var buildProcess = new System.Diagnostics.Process();
            buildProcess.StartInfo.FileName = "dotnet";
            buildProcess.StartInfo.ArgumentList.Add("publish");
            buildProcess.StartInfo.ArgumentList.Add("NativeAotApp.g.csproj");
            buildProcess.StartInfo.ArgumentList.Add("-p:Configuration=Release");
            buildProcess.StartInfo.ArgumentList.Add($"-p:TargetFramework={targetFramework}");
            buildProcess.StartInfo.ArgumentList.Add("-p:RuntimeIdentifier=" + RuntimeInformation.RuntimeIdentifier);
            buildProcess.StartInfo.ArgumentList.Add($"-p:NetCordNativesDir={nativesDir}");
            buildProcess.StartInfo.ArgumentList.Add($"-p:VcpkgRoot={vcpkgRoot}");

            TestContext.WriteLine($"Building Native AoT app in ({projectDirectory}): 'dotnet {buildProcess.StartInfo.ArgumentList.Aggregate((a, b) => $"{a} {b}")}'");

            buildProcess.StartInfo.WorkingDirectory = projectDirectory;
            buildProcess.StartInfo.RedirectStandardOutput = true;
            buildProcess.StartInfo.RedirectStandardError = true;
            buildProcess.Start();
            buildProcess.WaitForExit();
            
            TestContext.WriteLine($"Build Output of AoT app for '{libName}': {buildProcess.StandardOutput.ReadToEnd()}");

            Assert.AreEqual(0, buildProcess.ExitCode, 
                $"Native AoT build failed for '{libName}'. Output: {buildProcess.StandardError.ReadToEnd()}");

            // check that the library is running without errors, which indicates that it was statically linked successfully
            var aotProcess = new System.Diagnostics.Process();
            aotProcess.StartInfo.FileName = Path.Combine(projectDirectory, "bin", "release", targetFramework,
                                                            RuntimeInformation.RuntimeIdentifier, "publish",
                                                            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 
                                                            "NativeAotApp.g.exe" : "NativeAotApp.g");

            TestContext.WriteLine($"Running Native AoT app: '{aotProcess.StartInfo.FileName}'");

            aotProcess.StartInfo.RedirectStandardOutput = true;
            aotProcess.StartInfo.RedirectStandardError = true;
            aotProcess.Start();
            aotProcess.WaitForExit();

            TestContext.WriteLine($"Output of AoT app for '{libName}': {aotProcess.StandardOutput.ReadToEnd()}");

            Assert.AreEqual(0, aotProcess.ExitCode, 
                $"Native AoT app failed to run for '{libName}'. Output: {aotProcess.StandardError.ReadToEnd()}");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Failed to statically link '{libName}' for Native Ahead-of-Time (AoT) compilation: {ex}");
        }
    }
}
