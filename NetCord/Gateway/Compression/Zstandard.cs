using System.Runtime.InteropServices;

namespace NetCord.Gateway.Compression;

internal static partial class Zstandard
{
    private const string DllName = "libzstd";

    public static bool TryLoad()
    {
        return NativeLibrary.TryLoad(DllName, typeof(Zstandard).Assembly, null, out _);
    }

    [SuppressGCTransition]
    [LibraryImport(DllName, EntryPoint = "ZSTD_isError")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool IsError(nuint code);

    [LibraryImport(DllName, EntryPoint = "ZSTD_getErrorName", StringMarshalling = StringMarshalling.Utf8)]
    public static partial string GetErrorName(nuint code);

    [LibraryImport(DllName, EntryPoint = "ZSTD_createDStream")]
    public static partial DStreamHandle CreateDStream();

    [LibraryImport(DllName, EntryPoint = "ZSTD_freeDStream")]
    public static partial nuint FreeDStream(nint zds);

    [LibraryImport(DllName, EntryPoint = "ZSTD_initDStream")]
    public static partial nuint InitDStream(DStreamHandle zds);

    [LibraryImport(DllName, EntryPoint = "ZSTD_decompressStream")]
    public static partial nuint DecompressStream(DStreamHandle zds, ref Buffer output, ref Buffer input);

    public class DStreamHandle() : SafeHandle(0, true)
    {
        public override bool IsInvalid => handle is 0;

        protected override bool ReleaseHandle()
        {
            FreeDStream(handle);
            return true;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Buffer
    {
        public byte* Ptr;
        public nuint Size;
        public nuint Pos;
    }
}
