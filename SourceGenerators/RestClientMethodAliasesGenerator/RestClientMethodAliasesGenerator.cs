using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

using Shared;

namespace RestClientMethodAliasesGenerator;

[Generator(LanguageNames.CSharp)]
public class RestClientMethodAliasesGenerator : IIncrementalGenerator
{
    private const string AttributeName = "NetCord.Rest.GenerateAliasAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(context =>
        {
            context.AddSource("GenerateAliasAttribute.g.cs", SourceText.From(
                """
                #nullable enable

                namespace NetCord.Rest;

                [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
                internal class GenerateAliasAttribute(Type[] types, params string?[] parameterAliases) : Attribute
                {
                    public Type[] Types { get; } = types;

                    public string?[] ParameterAliases { get; } = parameterAliases;

                    public string? TypeNameOverride { get; init; }

                    public string? NameOverride { get; init; }

                    public bool Cast { get; init; }

                    public string[]? Modifiers { get; init; }

                    public Type? CastType { get; init; }

                    public string? ClientName { get; init; }
                }

                """, Encoding.UTF8));
        });

        var methodSymbols = context.SyntaxProvider.ForAttributeWithMetadataName(AttributeName,
                                                                                (syntaxNode, cancellationToken) => syntaxNode is MethodDeclarationSyntax,
                                                                                (context, cancellationToken) => (IMethodSymbol)context.SemanticModel.GetDeclaredSymbol(context.TargetNode, cancellationToken)!);

        context.RegisterSourceOutput(methodSymbols, GenerateMethods);
    }

    private void GenerateMethods(SourceProductionContext context, IMethodSymbol methodSymbol)
    {
        var attributes = methodSymbol.GetAttributes().Where(attributeData => attributeData.AttributeClass!.ToQualifiedName() == AttributeName);

        foreach (var attribute in attributes)
        {
            var constructorArguments = attribute.ConstructorArguments;

            var typeSymbols = constructorArguments[0].Values!.Select(c => (INamedTypeSymbol)c.Value!).ToArray();
            var parameterAliases = constructorArguments.Length >= 2 ? constructorArguments[1].Values.Select(value => (string?)value.Value).ToArray() : [];
            var nameOverride = GetNameOverride(attribute);
            string methodName;
            if (nameOverride is null)
            {
                var typeNameOverride = GetTypeNameOverride(attribute, typeSymbols);
                methodName = methodSymbol.Name.Replace(typeNameOverride, string.Empty);
            }
            else
                methodName = nameOverride;

            var cast = GetCast(attribute);
            var modifiers = GetModifiers(attribute);
            var castType = GetCastType(attribute);
            var clientName = GetClientName(attribute);
            var parameters = GetParameters(methodSymbol, parameterAliases);
            var documentation = GetDocumentation(methodSymbol, parameters);
            WriteMethodsForType(context, typeSymbols, methodName, methodSymbol, parameters, cast, modifiers, castType, clientName, documentation);
        }
    }

    private static ParameterInfo[] GetParameters(IMethodSymbol methodSymbol, string?[] parameterAliases)
    {
        var parameters = new ParameterInfo[methodSymbol.Parameters.Length];

        var methodParameters = methodSymbol.Parameters;
        int parameterAliasesLength = parameterAliases.Length;
        for (int i = 0; i < parameterAliasesLength; i++)
        {
            var alias = parameterAliases[i];
            if (alias is null)
            {
                var parameter = methodParameters[i];
                parameters[i] = new(parameter.Name, parameter);
            }
            else
                parameters[i] = new(alias, null);
        }
        var methodParametersLength = methodParameters.Length;
        for (int i = parameterAliasesLength; i < methodParametersLength; i++)
        {
            var parameter = methodParameters[i];
            parameters[i] = new(parameter.Name, parameter);
        }

        return parameters;
    }

    private static void WriteMethodsForType(SourceProductionContext context,
                                            INamedTypeSymbol[] currentTypeSymbols,
                                            string methodName,
                                            IMethodSymbol methodSymbol,
                                            ParameterInfo[] parameters,
                                            bool cast,
                                            string[]? modifiers,
                                            INamedTypeSymbol? castType,
                                            string clientName,
                                            string documentation)
    {
        foreach (var currentTypeSymbol in currentTypeSymbols)
        {
            WriteMethods(context, currentTypeSymbol, null, methodSymbol, parameters, methodName, cast, false, modifiers, castType, clientName, documentation);

            if (currentTypeSymbol.TypeKind is TypeKind.Interface)
            {
                if (cast)
                    ImplementInterfaceWithCasting(context, currentTypeSymbol, currentTypeSymbols, methodSymbol, parameters, methodName, documentation, modifiers, castType, clientName);
                else
                    ImplementInterface(context, currentTypeSymbol, currentTypeSymbols, methodSymbol, parameters, methodName, false, false, documentation, modifiers, castType, clientName);
            }
            else if (cast)
                WriteCastingMethodsForChildren(context, currentTypeSymbol, methodSymbol, parameters, methodName, documentation, modifiers, castType, clientName);
        }
    }

    private static void ImplementInterfaceWithCasting(SourceProductionContext context,
                                                      INamedTypeSymbol currentTypeSymbol,
                                                      INamedTypeSymbol[] typeSymbols,
                                                      IMethodSymbol methodSymbol,
                                                      ParameterInfo[] parameters,
                                                      string methodName,
                                                      string documentation,
                                                      string[]? modifiers,
                                                      INamedTypeSymbol? castType,
                                                      string clientName)
    {
        foreach (var type in GetAllNamespaceTypes(GetGlobalNamespace(currentTypeSymbol)))
        {
            if (type.AllInterfaces.Contains(currentTypeSymbol, SymbolEqualityComparer.Default))
            {
                if (type.TypeKind is TypeKind.Interface)
                {
                    WriteMethods(context, type, null, methodSymbol, parameters, methodName, true, true, modifiers, castType, clientName, documentation);
                    ImplementInterface(context, type, typeSymbols, methodSymbol, parameters, methodName, true, true, documentation, modifiers, castType, clientName);
                }
                else if (IsInterfaceImplemented(type, currentTypeSymbol))
                    WriteMethods(context, type, null, methodSymbol, parameters, methodName, true, true, modifiers, castType, clientName, documentation);
                else
                {
                    WriteMethods(context, type, currentTypeSymbol, methodSymbol, parameters, methodName, true, false, modifiers, castType, clientName, documentation);
                    WriteMethods(context, type, null, methodSymbol, parameters, methodName, true, false, modifiers, castType, clientName, documentation);
                }
            }
        }
    }

    private static void ImplementInterface(SourceProductionContext context,
                                           INamedTypeSymbol interfaceSymbol,
                                           INamedTypeSymbol[] typeSymbols,
                                           IMethodSymbol methodSymbol,
                                           ParameterInfo[] parameters,
                                           string methodName,
                                           bool cast,
                                           bool @explicit,
                                           string documentation,
                                           string[]? modifiers,
                                           INamedTypeSymbol? castType,
                                           string clientName)
    {
        foreach (var type in GetAllNamespaceTypes(GetGlobalNamespace(interfaceSymbol)))
        {
            if (type.AllInterfaces.Contains(interfaceSymbol, SymbolEqualityComparer.Default))
            {
                if (type.TypeKind is not TypeKind.Interface
                    && !IsInterfaceImplemented(type, interfaceSymbol)
                    && !GetBaseTypes(type).Any(t => typeSymbols.Contains(t, SymbolEqualityComparer.Default)))
                    WriteMethods(context, type, @explicit ? interfaceSymbol : null, methodSymbol, parameters, methodName, cast, false, modifiers, castType, clientName, documentation);
            }
        }
    }

    private static void WriteCastingMethodsForChildren(SourceProductionContext context,
                                                       INamedTypeSymbol currentTypeSymbol,
                                                       IMethodSymbol methodSymbol,
                                                       ParameterInfo[] parameters,
                                                       string methodName,
                                                       string documentation,
                                                       string[]? modifiers,
                                                       INamedTypeSymbol? castType,
                                                       string clientName)
    {
        foreach (var type in GetAllNamespaceTypes(GetGlobalNamespace(currentTypeSymbol)))
        {
            if (InheritsFrom(type, currentTypeSymbol))
                WriteMethods(context, type, null, methodSymbol, parameters, methodName, true, true, modifiers, castType, clientName, documentation);
        }
    }

    private static void WriteMethods(SourceProductionContext context,
                                     INamedTypeSymbol typeSymbol,
                                     INamedTypeSymbol? interfaceToImplementSymbol,
                                     IMethodSymbol methodSymbol,
                                     ParameterInfo[] parameters,
                                     string methodName,
                                     bool cast,
                                     bool hides,
                                     string[]? modifiers,
                                     INamedTypeSymbol? castType,
                                     string clientName,
                                     string documentation)
    {
        StringWriter stringWriter = new();

        stringWriter.WriteAutoGeneratedComment();
        stringWriter.WriteLine();

        stringWriter.WriteNullableDirective();
        stringWriter.WriteLine();

        stringWriter.WriteNamespace(typeSymbol);
        stringWriter.WriteLine();

        stringWriter.WriteTypeDeclaration(typeSymbol);

        stringWriter.Write("{");

        WriteMethodDefinition(stringWriter, typeSymbol, interfaceToImplementSymbol, methodSymbol, parameters, methodName, ref cast, hides, documentation, modifiers, castType, out var returnType);

        if (typeSymbol.TypeKind is TypeKind.Interface)
            stringWriter.WriteLine(";");
        else
        {
            stringWriter.WriteLine();
            stringWriter.WriteIndentation(1);
            stringWriter.WriteLine('{');
            WriteMethodBody(typeSymbol, interfaceToImplementSymbol, methodSymbol, parameters, stringWriter, cast, castType, clientName);
            stringWriter.WriteIndentation(1);
            stringWriter.WriteLine('}');
        }

        stringWriter.WriteLine('}');

        var name = GetFileName(returnType, typeSymbol, methodSymbol, methodName);
        context.AddSource(name, SourceText.From(stringWriter.ToString(), Encoding.UTF8));
    }

    private static void WriteMethodDefinition(StringWriter stringWriter,
                                              INamedTypeSymbol typeSymbol,
                                              INamedTypeSymbol? interfaceToImplementSymbol,
                                              IMethodSymbol sourceMethodSymbol,
                                              ParameterInfo[] parameters,
                                              string methodName,
                                              ref bool cast,
                                              bool hides,
                                              string documentation,
                                              string[]? modifiers,
                                              INamedTypeSymbol? castType,
                                              out ITypeSymbol returnType)
    {
        stringWriter.WriteLine();

        var explicitImplementation = interfaceToImplementSymbol is not null;

        if (!explicitImplementation)
            stringWriter.Write(documentation);

        stringWriter.WriteIndentation(1);
        if (!explicitImplementation)
            stringWriter.Write("public ");

        if (hides)
            stringWriter.Write("new ");

        if (modifiers is not null)
        {
            int modifiersLength = modifiers.Length;
            for (int i = 0; i < modifiersLength; i++)
            {
                stringWriter.Write(modifiers[i]);
                stringWriter.Write(' ');
            }
        }

        var methodReturnType = sourceMethodSymbol.ReturnType;
        if (castType is not null)
        {
            if (methodReturnType is not INamedTypeSymbol { Arity: 1 } namedTypeSymbol)
                throw new InvalidOperationException($"Failed to cast the return type of '{sourceMethodSymbol.ToDisplayString()}'");

            returnType = namedTypeSymbol.ConstructedFrom.Construct(castType);
            cast = !SymbolEqualityComparer.Default.Equals(returnType, methodReturnType);
        }
        else if (cast)
        {
            if (methodReturnType is not INamedTypeSymbol { Arity: 1 } namedTypeSymbol)
                throw new InvalidOperationException($"Failed to cast the return type of '{sourceMethodSymbol.ToDisplayString()}'");

            returnType = namedTypeSymbol.ConstructedFrom.Construct(explicitImplementation ? interfaceToImplementSymbol! : typeSymbol);
            cast = !SymbolEqualityComparer.Default.Equals(returnType, methodReturnType);
        }
        else
            returnType = methodReturnType;

        if (cast && typeSymbol.TypeKind is not TypeKind.Interface)
            stringWriter.Write("async ");

        stringWriter.Write(returnType.ToDisplayString());
        stringWriter.Write(' ');

        if (explicitImplementation)
        {
            stringWriter.Write(interfaceToImplementSymbol!.ToDisplayString());
            stringWriter.Write('.');
        }

        stringWriter.Write(methodName);
        stringWriter.Write('(');

        int maxIndex = parameters.Length - 1;

        for (int i = 0; i < maxIndex; i++)
        {
            var parameter = parameters[i];
            if (!parameter.Alias)
            {
                stringWriter.Write(parameter.Parameter!.ToDefinitionString(explicitImplementation));
                stringWriter.Write(", ");
            }
        }

        var lastParameter = parameters[maxIndex];
        if (!lastParameter.Alias)
            stringWriter.Write(lastParameter.Parameter!.ToDefinitionString(explicitImplementation));

        stringWriter.Write(')');
    }

    private static void WriteMethodBody(INamedTypeSymbol typeSymbol,
                                        INamedTypeSymbol? interfaceToImplementSymbol,
                                        IMethodSymbol methodSymbol,
                                        ParameterInfo[] parameters,
                                        StringWriter stringWriter,
                                        bool cast,
                                        INamedTypeSymbol? castType,
                                        string clientName)
    {
        stringWriter.WriteIndentation(2);

        stringWriter.Write("return ");

        if (cast)
        {
            stringWriter.Write('(');
            stringWriter.Write((castType ?? interfaceToImplementSymbol ?? typeSymbol).ToDisplayString());
            stringWriter.Write(')');

            stringWriter.Write("await ");
            stringWriter.Write(clientName);
            stringWriter.Write('.');
        }
        else
        {
            stringWriter.Write(clientName);
            stringWriter.Write('.');
        }

        stringWriter.Write(methodSymbol.Name);
        stringWriter.Write('(');

        int maxIndex = parameters.Length - 1;
        for (int i = 0; i < maxIndex; i++)
        {
            stringWriter.Write(parameters[i].Name);
            stringWriter.Write(", ");
        }

        stringWriter.Write(parameters[maxIndex].Name);

        stringWriter.WriteLine(");");
    }

    private string? GetNameOverride(AttributeData attribute)
    {
        var namedArguments = attribute.NamedArguments;
        var length = namedArguments.Length;
        for (int i = 0; i < length; i++)
        {
            var argument = namedArguments[i];

            if (argument.Key == "NameOverride")
                return (string?)argument.Value.Value;
        }

        return null;
    }

    private static string GetTypeNameOverride(AttributeData generateAliasAttributeData, INamedTypeSymbol[] types)
    {
        var namedArguments = generateAliasAttributeData.NamedArguments;
        var length = namedArguments.Length;
        for (int i = 0; i < length; i++)
        {
            var argument = namedArguments[i];

            if (argument.Key == "TypeNameOverride")
            {
                var value = argument.Value.Value;
                if (value is null)
                    break;

                return (string)value;
            }
        }

        var type = types[0];

        var name = type.Name;

        if (type.TypeKind is TypeKind.Interface && name.Length > 0 && name[0] == 'I')
            return name[1..];

        return name;
    }

    private static bool GetCast(AttributeData generateAliasAttributeData)
    {
        var namedArguments = generateAliasAttributeData.NamedArguments;
        var length = namedArguments.Length;
        for (int i = 0; i < length; i++)
        {
            var argument = namedArguments[i];

            if (argument.Key == "Cast")
                return (bool)argument.Value.Value!;
        }

        return false;
    }

    private static string[]? GetModifiers(AttributeData generateAliasAttributeData)
    {
        var namedArguments = generateAliasAttributeData.NamedArguments;
        var length = namedArguments.Length;
        for (int i = 0; i < length; i++)
        {
            var argument = namedArguments[i];

            if (argument.Key == "Modifiers")
                return argument.Value.Values.Select(v => (string)v.Value!).ToArray();
        }

        return null;
    }

    private static INamedTypeSymbol? GetCastType(AttributeData attribute)
    {
        var namedArguments = attribute.NamedArguments;
        var length = namedArguments.Length;
        for (int i = 0; i < length; i++)
        {
            var argument = namedArguments[i];

            if (argument.Key == "CastType")
                return (INamedTypeSymbol)argument.Value.Value!;
        }

        return null;
    }

    private static string GetClientName(AttributeData attribute)
    {
        var namedArguments = attribute.NamedArguments;
        var length = namedArguments.Length;
        for (int i = 0; i < length; i++)
        {
            var argument = namedArguments[i];

            if (argument.Key == "ClientName")
                return (string?)argument.Value.Value ?? "_client";
        }

        return "_client";
    }

    private static string GetDocumentation(IMethodSymbol methodSymbol, ParameterInfo[] parameters)
    {
        StringWriter documentationWriter = new();
        documentationWriter.CopyXmlComment(methodSymbol, node =>
        {
            if (node is { Name: "param", Attributes: var attributes } && attributes["name"] is var name)
                return parameters.Any(parameter => parameter.Name == name.Value);

            return true;
        });
        return documentationWriter.ToString();
    }

    private static string GetFileName(ITypeSymbol returnType, INamedTypeSymbol typeSymbol, IMethodSymbol methodSymbol, string methodName)
    {
        StringWriter fileNameWriter = new();

        fileNameWriter.Write(returnType.ToUniqueSimpleName());
        fileNameWriter.Write(' ');

        fileNameWriter.Write(typeSymbol.ToUniqueSimpleName());
        fileNameWriter.Write('.');
        fileNameWriter.Write(methodName);

        var parameters = methodSymbol.Parameters;
        var parametersLength = parameters.Length;
        for (int i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            fileNameWriter.Write(',');
            fileNameWriter.Write(parameter.Type.ToUniqueSimpleName());
        }
        fileNameWriter.Write(".g.cs");

        return fileNameWriter.ToString();
    }

    private static IEnumerable<INamedTypeSymbol> GetAllNamespaceTypes(INamespaceSymbol @namespace)
    {
        var types = @namespace.GetTypeMembers();
        int length = types.Length;
        for (var i = 0; i < length; i++)
            yield return types[i];

        foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
            foreach (var type in GetAllNamespaceTypes(nestedNamespace))
                yield return type;
    }

    private static bool IsInterfaceImplemented(INamedTypeSymbol typeSymbol, INamedTypeSymbol interfaceSymbol)
    {
        var baseType = typeSymbol.BaseType;
        if (baseType is not null && baseType.AllInterfaces.Contains(interfaceSymbol, SymbolEqualityComparer.Default))
            return true;

        return false;
    }

    private static INamespaceSymbol GetGlobalNamespace(ITypeSymbol typeSymbol)
    {
        var namespaceSymbol = typeSymbol.ContainingNamespace;
        while (!namespaceSymbol.IsGlobalNamespace)
            namespaceSymbol = namespaceSymbol.ContainingNamespace;

        return namespaceSymbol;
    }

    private static IEnumerable<INamedTypeSymbol> GetBaseTypes(INamedTypeSymbol typeSymbol)
    {
        var baseType = typeSymbol.BaseType;
        while (baseType is not null)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
    }

    private static bool InheritsFrom(INamedTypeSymbol typeSymbol, INamedTypeSymbol baseTypeSymbol)
    {
        return GetBaseTypes(typeSymbol).Contains(baseTypeSymbol, SymbolEqualityComparer.Default);
    }

    private record struct ParameterInfo(string Name, IParameterSymbol? Parameter)
    {
        public readonly bool Alias => Parameter is null;
    }
}
