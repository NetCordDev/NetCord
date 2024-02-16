using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NetCord.Services;

internal class ReadOnlyMemoryCharComparer(CompareInfo compareInfo, CompareOptions compareOptions) : IComparer<ReadOnlyMemory<char>>, IEqualityComparer<ReadOnlyMemory<char>>
{
    public static ReadOnlyMemoryCharComparer InvariantCulture { get; } = new(CultureInfo.InvariantCulture.CompareInfo, CompareOptions.None);

    public static ReadOnlyMemoryCharComparer InvariantCultureIgnoreCase { get; } = new(CultureInfo.InvariantCulture.CompareInfo, CompareOptions.IgnoreCase);

    public static ReadOnlyMemoryCharComparer CurrentCulture { get; } = new(CultureInfo.CurrentCulture.CompareInfo, CompareOptions.None);

    public static ReadOnlyMemoryCharComparer CurrentCultureIgnoreCase { get; } = new(CultureInfo.CurrentCulture.CompareInfo, CompareOptions.IgnoreCase);

    public static ReadOnlyMemoryCharComparer Ordinal { get; } = new(CultureInfo.InvariantCulture.CompareInfo, CompareOptions.Ordinal);

    public static ReadOnlyMemoryCharComparer OrdinalIgnoreCase { get; } = new(CultureInfo.InvariantCulture.CompareInfo, CompareOptions.OrdinalIgnoreCase);

    public int Compare(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
    {
        return compareInfo.Compare(x.Span, y.Span, compareOptions);
    }

    public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode([DisallowNull] ReadOnlyMemory<char> obj)
    {
        return compareInfo.GetHashCode(obj.Span, compareOptions);
    }
}
