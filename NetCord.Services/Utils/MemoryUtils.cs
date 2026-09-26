using System.Buffers;

namespace NetCord.Services.Utils;

internal static class MemoryUtils
{
    public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> memory, SearchValues<char> trimElements)
    {
        int index = memory.Span.IndexOfAnyExcept(trimElements);
        return index >= 0 ? memory[index..] : default;
    }
}
