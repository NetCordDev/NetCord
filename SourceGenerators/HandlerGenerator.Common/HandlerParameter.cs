using Microsoft.CodeAnalysis;

namespace HandlerGenerator.Common;

public record struct HandlerParameter(string Name, string Type)
{
    public HandlerParameter(string name, ISymbol type) : this(name, type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
    {
    }
}

