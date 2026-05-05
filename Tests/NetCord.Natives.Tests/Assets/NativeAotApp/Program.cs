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

    [LibraryImport("zstd", EntryPoint = "ZSTD_versionNumber")]
    internal static partial uint ZstdVersionNumber();
}

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("NetCord native ahead of time publish app.");

        Console.WriteLine($"Dave Max Supported Protocol Version: {NativeProbes.DaveMaxSupportedProtocolVersion()}");
        Console.WriteLine($"Sodium Init: {NativeProbes.SodiumInit()}. 0 = OK");
        Console.WriteLine($"Opus Version String: {Marshal.PtrToStringAnsi(NativeProbes.OpusGetVersionString())}");
        Console.WriteLine($"Zstd Version Number: {NativeProbes.ZstdVersionNumber()}");
    }
}
