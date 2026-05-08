using System.Runtime.InteropServices;
using NetCord.Natives.Tests;

Console.WriteLine("NetCord native ahead of time publish app.");

Console.WriteLine($"Dave Max Supported Protocol Version: {NativeProbes.DaveMaxSupportedProtocolVersion()}");
Console.WriteLine($"Sodium Init: {NativeProbes.SodiumInit()}. 0 = OK");
Console.WriteLine($"Opus Version String: {Marshal.PtrToStringAnsi(NativeProbes.OpusGetVersionString())}");
Console.WriteLine($"Zstd Version Number: {NativeProbes.ZstdVersionNumber()}");
