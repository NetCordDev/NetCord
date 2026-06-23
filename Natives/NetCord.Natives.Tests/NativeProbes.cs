using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NetCord.Natives.Tests;

public static partial class NativeProbes
{
    [LibraryImport("libdave", EntryPoint = "daveMaxSupportedProtocolVersion")]
    internal static partial ushort DaveMaxSupportedProtocolVersion();

    [LibraryImport("libsodium", EntryPoint = "sodium_init")]
    internal static partial int SodiumInit();

    [LibraryImport("opus", EntryPoint = "opus_get_version_string")]
    internal static partial IntPtr OpusGetVersionString();

    [LibraryImport("zstd", EntryPoint = "ZSTD_versionNumber")]
    internal static partial uint ZstdVersionNumber();

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode", Justification = "This method is only used in tests to verify native library exports and is not called in the runtime path of trimmed/AOT applications.")]
    [UnconditionalSuppressMessage("Trimming", "IL2075:DynamicallyAccessedMembers", Justification = "This method is only used in tests to verify native library exports and is not called in the runtime path of trimmed/AOT applications.")]
    internal static IReadOnlyList<string> GetMissingLibraryImports(IntPtr libHandle, string libName)
    {
        var missingExports = new List<string>();
        var methods = new List<(MethodInfo Method, string? EntryPoint)>();

        // typeof(NetCord.Application).Assembly ensures the NetCord assembly is loaded before scanning.
        // Exclude the test assembly itself — its probe stubs are not real library consumers.
        var testAssembly = Assembly.GetExecutingAssembly();
        var netcordAssemblies = new[] { typeof(NetCord.Application).Assembly }
            .Concat(AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a != testAssembly && a.GetName().Name?.StartsWith("NetCord", StringComparison.OrdinalIgnoreCase) == true))
            .DistinctBy(a => a.FullName)
            .ToList();

        Console.WriteLine($"[{libName}] Scanning {netcordAssemblies.Count} NetCord assemblies: {string.Join(", ", netcordAssemblies.Select(a => a.GetName().Name))}");

        foreach (var type in netcordAssemblies.SelectMany(a => a.GetTypes()))
        {
            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var libraryImport = method.GetCustomAttribute<LibraryImportAttribute>();
                if (libraryImport != null)
                {
                    if (IsMatchingLibrary(libraryImport.LibraryName, libName))
                    {
                        methods.Add((method, libraryImport.EntryPoint));
                    }
                    continue;
                }

                var dllImport = method.GetCustomAttribute<DllImportAttribute>();
                if (dllImport != null)
                {
                    if (IsMatchingLibrary(dllImport.Value, libName))
                    {
                        methods.Add((method, dllImport.EntryPoint));
                    }
                }
            }
        }

        if (methods.Count == 0)
            throw new InvalidOperationException($"No methods with [LibraryImport] or [DllImport] found for library '{libName}'.");

        foreach (var item in methods.DistinctBy(m => m.EntryPoint ?? m.Method.Name))
        {
            // Use EntryPoint if defined, otherwise fallback to method name.
            string exportName = item.EntryPoint ?? item.Method.Name;
            string importSig = $"{item.Method.ReturnType.Name} {item.Method.DeclaringType!.FullName}.{item.Method.Name}({string.Join(", ", item.Method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})";

            if (!NativeLibrary.TryGetExport(libHandle, exportName, out _))
            {
                Console.WriteLine($"[{libName}] MISSING: {exportName}");
                Console.WriteLine($"          Import: {importSig}");
                missingExports.Add(exportName);
            }
            else
            {
                Console.WriteLine($"[{libName}] Found:   {exportName}");
                Console.WriteLine($"          Import: {importSig}");
            }
        }

        return missingExports;
    }

    private static bool IsMatchingLibrary(string? attributeLibName, string searchLibName)
    {
        if (attributeLibName == null)
            return false;

        if (string.Equals(attributeLibName, searchLibName, StringComparison.OrdinalIgnoreCase))
            return true;

        if (attributeLibName.StartsWith("lib", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(attributeLibName.Substring(3), searchLibName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        else
        {
            if (string.Equals("lib" + attributeLibName, searchLibName, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
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
            "libdave" => DaveMaxSupportedProtocolVersion(),
            "libsodium" => SodiumInit(),
            "opus" => OpusGetVersionString(),
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
