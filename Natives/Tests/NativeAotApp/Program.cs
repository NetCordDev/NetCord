using System.Runtime.InteropServices;
using NetCord.Natives.Tests;

Console.WriteLine("NetCord native ahead of time publish app.");

Console.WriteLine($"Dave Max Supported Protocol Version: {NativeProbes.DaveMaxSupportedProtocolVersion()}");
Console.WriteLine($"Sodium Init: {NativeProbes.SodiumInit()}. 0 = OK");
Console.WriteLine($"Opus Version String: {Marshal.PtrToStringAnsi(NativeProbes.OpusGetVersionString())}");
uint v = NativeProbes.ZstdVersionNumber();
Console.WriteLine($"Zstd Version: {v / 10000}.{(v / 100) % 100}.{v % 100}");
