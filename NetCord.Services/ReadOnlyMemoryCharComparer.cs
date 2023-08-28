using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NetCord.Services;

internal class ReadOnlyMemoryCharComparer : IComparer<ReadOnlyMemory<char>>, IEqualityComparer<ReadOnlyMemory<char>>
{
    public static ReadOnlyMemoryCharComparer InvariantCulture { get; } = new(CultureInfo.InvariantCulture.CompareInfo, CompareOptions.None);

    public static ReadOnlyMemoryCharComparer InvariantCultureIgnoreCase { get; } = new(CultureInfo.InvariantCulture.CompareInfo, CompareOptions.IgnoreCase);

    public static ReadOnlyMemoryCharComparer CurrentCulture { get; } = new(CultureInfo.CurrentCulture.CompareInfo, CompareOptions.None);

    public static ReadOnlyMemoryCharComparer CurrentCultureIgnoreCase { get; } = new(CultureInfo.CurrentCulture.CompareInfo, CompareOptions.IgnoreCase);

    public static ReadOnlyMemoryCharComparer Ordinal { get; } = new(CultureInfo.InvariantCulture.CompareInfo, CompareOptions.Ordinal);

    public static ReadOnlyMemoryCharComparer OrdinalIgnoreCase { get; } = new(CultureInfo.InvariantCulture.CompareInfo, CompareOptions.OrdinalIgnoreCase);

    private readonly CompareInfo _compareInfo;
    private readonly CompareOptions _compareOptions;

    public ReadOnlyMemoryCharComparer(CompareInfo compareInfo, CompareOptions compareOptions)
    {
        _compareInfo = compareInfo;
        _compareOptions = compareOptions;
    }

    public int Compare(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
    {
        return _compareInfo.Compare(x.Span, y.Span, _compareOptions);
    }

    public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode([DisallowNull] ReadOnlyMemory<char> obj)
    {
        return _compareInfo.GetHashCode(obj.Span, _compareOptions);
    }
}
