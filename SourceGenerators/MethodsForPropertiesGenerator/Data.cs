using Microsoft.CodeAnalysis;

namespace MethodsForPropertiesGenerator;

public record Data
{
    private Data()
    {
    }

    public record Success(INamedTypeSymbol Symbol) : Data;

    public record Error(INamedTypeSymbol Symbol) : Data;
}
