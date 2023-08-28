using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NetCord.Services;

internal class InvariantIgnoreCaseReadOnlyMemoryCharComparer : IComparer<ReadOnlyMemory<char>>, IEqualityComparer<ReadOnlyMemory<char>>
{
    private static readonly CompareInfo _invariantCompareInfo = CultureInfo.InvariantCulture.CompareInfo;

    public static InvariantIgnoreCaseReadOnlyMemoryCharComparer Instance { get; } = new();

    public int Compare(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
    {
        return _invariantCompareInfo.Compare(x.Span, y.Span, CompareOptions.IgnoreCase);
    }

    public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode([DisallowNull] ReadOnlyMemory<char> obj)
    {
        return _invariantCompareInfo.GetHashCode(obj.Span, CompareOptions.IgnoreCase);
    }
}
