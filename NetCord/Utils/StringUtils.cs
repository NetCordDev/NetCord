using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetCord;

internal static class StringUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsSpanFast(this string s) => MemoryMarshal.CreateReadOnlySpan(in s.GetPinnableReference(), s.Length);
}
