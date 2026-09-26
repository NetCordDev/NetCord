using System.Runtime.InteropServices;

namespace NetCord.Gateway.Compression;

public class ZstandardException(nuint code) : Exception($"Zstandard returned an '{Marshal.PtrToStringUTF8((nint)Zstandard.GetErrorName(code))}' error.")
{
}
