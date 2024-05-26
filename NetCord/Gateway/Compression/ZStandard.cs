using System.Runtime.InteropServices;

namespace NetCord.Gateway.Compression;

internal static partial class ZStandard
{
    public static bool TryLoad()
    {
        return NativeLibrary.TryLoad("libzstd", typeof(ZStandard).Assembly, null, out _);
    }

    [LibraryImport("libzstd", EntryPoint = "ZSTD_isError")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool IsError(nuint code);

    [LibraryImport("libzstd", EntryPoint = "ZSTD_getErrorName", StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial string GetErrorName(nuint code);

    [LibraryImport("libzstd", EntryPoint = "ZSTD_createDStream")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial DStreamHandle CreateDStream();

    [LibraryImport("libzstd", EntryPoint = "ZSTD_freeDStream")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial nuint FreeDStream(nint zds);

    [LibraryImport("libzstd", EntryPoint = "ZSTD_initDStream")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
    public static partial nuint InitDStream(DStreamHandle zds);

    [LibraryImport("libzstd", EntryPoint = "ZSTD_decompressStream")]
    [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
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
