using System.Globalization;

namespace NetCord.Services.EnumTypeReaders;

internal delegate bool TryParseDelegate<T>(ReadOnlySpan<char> input, out T value);

internal static class TryParseDelegateHelper
{
    public static unsafe TryParseDelegate<T> Create<T>(delegate*<ReadOnlySpan<char>, NumberStyles, IFormatProvider?, out T, bool> tryParse, NumberStyles numberStyles, CultureInfo cultureInfo) where T : struct
    {
        return (ReadOnlySpan<char> input, out T value) => tryParse(input, numberStyles, cultureInfo, out value);
    }
}
