using Microsoft.CodeAnalysis;

namespace Shared;

public static class INamedTypeSymbolExtensions
{
    private static readonly SymbolDisplayFormat _qualifiedNameFormat = new(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        parameterOptions: SymbolDisplayParameterOptions.IncludeType);

    public static string ToQualifiedName(this INamedTypeSymbol symbol)
    {
        var typeParameters = symbol.TypeParameters;
        var length = typeParameters.Length;
        return length == 0
            ? symbol.ToDisplayString(_qualifiedNameFormat)
            : $"{symbol.ToDisplayString(_qualifiedNameFormat)}`{length}";
    }
}
