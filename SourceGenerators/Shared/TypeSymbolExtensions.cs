using System.Text;

using Microsoft.CodeAnalysis;

namespace Shared;

public static class TypeSymbolExtensions
{
    private static readonly SymbolDisplayFormat _qualifiedNameFormat = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

    public static string ToQualifiedName(this INamedTypeSymbol symbol)
    {
        symbol = symbol.OriginalDefinition;
        var arity = symbol.Arity;
        return arity == 0
            ? symbol.ToDisplayString(_qualifiedNameFormat)
            : $"{symbol.ToDisplayString(_qualifiedNameFormat)}`{arity}";
    }

    public static string ToUniqueSimpleName(this ITypeSymbol symbol)
    {
        StringBuilder stringBuilder = new();
        AppendUniqueSimpleName(stringBuilder, symbol);
        return stringBuilder.ToString();
    }

    private static void AppendUniqueSimpleName(StringBuilder stringBuilder, ITypeSymbol symbol)
    {
        stringBuilder.Append(symbol.Name);

        if (symbol is not INamedTypeSymbol { TypeArguments: var typeArguments and not { Length: 0 } })
            return;

        stringBuilder.Append('[');
        var maxIndex = typeArguments.Length - 1;
        for (int i = 0; i < maxIndex; i++)
        {
            AppendUniqueSimpleName(stringBuilder, typeArguments[i]);
            stringBuilder.Append(',');
        }

        AppendUniqueSimpleName(stringBuilder, typeArguments[maxIndex]);
        stringBuilder.Append(']');
    }
}
