using Microsoft.CodeAnalysis;

namespace Shared;

public class SymbolArrayEqualityComparer : IEqualityComparer<ISymbol[]>
{
    public static SymbolArrayEqualityComparer Default { get; } = new();

    private SymbolArrayEqualityComparer()
    {
    }

    public bool Equals(ISymbol[] x, ISymbol[] y)
    {
        return x.SequenceEqual(y, SymbolEqualityComparer.Default);
    }

    public int GetHashCode(ISymbol[] obj)
    {
        HashCode hashCode = default;

        int length = obj.Length;
        for (int i = 0; i < length; i++)
            hashCode.Add(obj[i], SymbolEqualityComparer.Default);

        return hashCode.ToHashCode();
    }
}
