using System.Reflection;
using System.Runtime.InteropServices;

namespace NetCord.Natives.Tests;

public static partial class NativeProbes
{
    [LibraryImport("libdave", EntryPoint = "daveMaxSupportedProtocolVersion")]
    internal static partial uint DaveMaxSupportedProtocolVersion();

    [LibraryImport("libsodium", EntryPoint = "sodium_init")]
    internal static partial int SodiumInit();

    [LibraryImport("opus", EntryPoint = "opus_get_version_string")]
    internal static partial IntPtr OpusGetVersionString();

    [LibraryImport("zstd", EntryPoint = "ZSTD_versionNumber")]
    internal static partial uint ZstdVersionNumber();

    internal static IReadOnlyList<string> GetMissingLibraryImports(string libName, string className, Assembly assemblyWithImports)
    {
        var typeWithImports = assemblyWithImports.GetType(className, true);
        var libHandle = GetLoadedNativeModuleHandle(libName);
        var missingExports = new List<string>();

        var methods = typeWithImports!.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(m => new
            {
                MethodName = m.Name,
                Attr = m.GetCustomAttribute<LibraryImportAttribute>(),
            })
            .Where(x => x.Attr != null)
            .ToArray();

        if (methods.Length == 0)
            throw new InvalidOperationException($"No methods with [LibraryImport] found in '{className}'.");

        foreach (var item in methods)
        {
            // Use EntryPoint if defined, otherwise fallback to method name.
            string exportName = item.Attr!.EntryPoint ?? item.MethodName;

            if (!NativeLibrary.TryGetExport(libHandle, exportName, out _))
            {
                missingExports.Add(exportName);
            }
        }

        return missingExports;
    }

    /*
     * This method is not working on macOS due to System.Diagnostics.ProcessModule 
     * not listing native libraries loaded by the runtime's dynamic library loader (dyld).
     * This is likely because dyld does not expose these libraries to the process in the 
     * same way that LoadLibrary on Windows or dlopen on Linux does, and as a result they 
     * do not appear in the list of modules for the process. This is a known limitation 
     * when trying to inspect loaded native libraries on macOS using .NET, and may require 
     * platform-specific workarounds or tools to verify the presence of native libraries 
     * on that platform.
    */
    internal static IntPtr GetLoadedNativeModuleHandle(string libName)
    {
        _ = libName switch
        {
            "libdave" => (object)DaveMaxSupportedProtocolVersion(),
            "libsodium" => (object)SodiumInit(),
            "opus" => (object)OpusGetVersionString(),
            "zstd" => (object)ZstdVersionNumber(),
            _ => throw new InvalidOperationException($"Unknown library name '{libName}' provided to test."),
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            throw new PlatformNotSupportedException("Retrieving loaded native module handles is not supported on macOS due to platform limitations.");

        var module = System.Diagnostics.Process.GetCurrentProcess().Modules
            .Cast<System.Diagnostics.ProcessModule>()
            .FirstOrDefault(m =>
            {
                var moduleName = Path.GetFileName(m.FileName);
                return moduleName.StartsWith(libName, StringComparison.OrdinalIgnoreCase)
                    || moduleName.StartsWith($"lib{libName}", StringComparison.OrdinalIgnoreCase);
            })
            ?? throw new InvalidOperationException($"Native module '{libName}' was not found in the current process.");

        if (NativeLibrary.TryLoad(module.FileName, out var libHandleByPath))
        {
            Console.WriteLine($"Successfully obtained a handle for native module '{module.ModuleName}' using path '{module.FileName}'.");
            return libHandleByPath;
        }

        throw new InvalidOperationException($"Failed to obtain a handle for native module '{module.ModuleName}'.");
    }
}
