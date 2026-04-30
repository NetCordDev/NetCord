using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

internal static partial class NativeProbes
{
    [LibraryImport("libdave", EntryPoint = "daveMaxSupportedProtocolVersion")]
    internal static partial uint DaveMaxSupportedProtocolVersion();

    [LibraryImport("libsodium", EntryPoint = "sodium_init")]
    internal static partial int SodiumInit();

    [LibraryImport("opus", EntryPoint = "opus_get_version_string")]
    internal static partial nint OpusGetVersionString();

    [LibraryImport("libzstd", EntryPoint = "ZSTD_versionNumber")]
    internal static partial uint ZstdVersionNumber();
}

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("NetCord native ahead of time publish app.");

        Console.WriteLine($"Dave Max Supported Protocol Version: {NativeProbes.DaveMaxSupportedProtocolVersion()}");
        Console.WriteLine($"Sodium Init: {NativeProbes.SodiumInit()}");
        Console.WriteLine($"Opus Version String: {Marshal.PtrToStringAnsi(NativeProbes.OpusGetVersionString())}");
        Console.WriteLine($"Zstd Version Number: {NativeProbes.ZstdVersionNumber()}");

        var cwd = Path.GetFullPath(Directory.GetCurrentDirectory());
        Console.WriteLine($"Current working directory: {cwd}");

        string[] libraries = new[] { "libdave", "libsodium", "opus", "libzstd" };
        var process = Process.GetCurrentProcess();
        var modules = process.Modules;

        bool allUnderCwd = true;

        foreach (var lib in libraries)
        {
            ProcessModule found = null;
            foreach (ProcessModule m in modules)
            {
                if (!string.IsNullOrEmpty(m.FileName) && m.FileName.IndexOf(lib, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    found = m;
                    break;
                }
                if (!string.IsNullOrEmpty(m.ModuleName) && m.ModuleName.IndexOf(lib, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    found = m;
                    break;
                }
            }

            if (found == null)
            {
                Console.WriteLine($"Library '{lib}' not found among loaded modules.");
                allUnderCwd = false;
                continue;
            }

            Console.WriteLine($"{lib} loaded from: {found.FileName}");
            if (!IsUnderDirectory(found.FileName, cwd))
            {
                Console.WriteLine($"Library '{lib}' is not under current working directory.");
                allUnderCwd = false;
            }
        }

        Environment.Exit(allUnderCwd ? 0 : 1);
    }

    private static bool IsUnderDirectory(string path, string directory)
    {
        try
        {
            var fullPath = Path.GetFullPath(path);
            var fullDir = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var comparison = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return fullPath.StartsWith(fullDir, comparison);
        }
        catch
        {
            return false;
        }
    }
}
