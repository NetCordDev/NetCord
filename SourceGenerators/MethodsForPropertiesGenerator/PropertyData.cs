using Microsoft.CodeAnalysis;

namespace MethodsForPropertiesGenerator;

internal class PropertyData(IPropertySymbol symbol, INamedTypeSymbol containingType, bool inherited)
{
    public IPropertySymbol Symbol { get; } = symbol;

    public INamedTypeSymbol ContainingType { get; } = containingType;

    public bool Inherited { get; } = inherited;
}
