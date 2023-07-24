using Microsoft.CodeAnalysis;

namespace MethodsForPropertiesGenerator;

internal class PropertyData
{
    public PropertyData(IPropertySymbol symbol, INamedTypeSymbol containingType, bool inherited)
    {
        Symbol = symbol;
        ContainingType = containingType;
        Inherited = inherited;
    }

    public IPropertySymbol Symbol { get; }

    public INamedTypeSymbol ContainingType { get; }

    public bool Inherited { get; }
}
