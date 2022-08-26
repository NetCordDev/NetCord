using System.Reflection;

using NetCord.Services.Utils;

namespace NetCord.Services.Interactions;

public class InteractionParameter<TContext> where TContext : InteractionContext
{
    public InteractionTypeReader<TContext> TypeReader { get; }
    public Type Type { get; }
    public bool Params { get; }
    public IReadOnlyDictionary<Type, IReadOnlyList<Attribute>> Attributes { get; }
    public string? Name { get; }
    public string? Description { get; }

    internal InteractionParameter(ParameterInfo parameter, InteractionServiceOptions<TContext> options)
    {
        Attributes = parameter.GetCustomAttributes().ToRankedDictionary(a => a.GetType());

        Type type;

        if (Attributes.TryGetValue(typeof(ParameterAttribute), out IReadOnlyList<Attribute>? attributes))
        {
            var commandParameterAttribute = (ParameterAttribute)attributes[0];
            Name = commandParameterAttribute.Name;
            Description = commandParameterAttribute.Description;
        }

        if (Attributes.ContainsKey(typeof(ParamArrayAttribute)))
        {
            Params = true;
            type = parameter.ParameterType.GetElementType()!;
        }
        else
            type = parameter.ParameterType;

        var underlyingType = Nullable.GetUnderlyingType(type);

        var typeReaders = options.TypeReaders;

        if (Attributes.TryGetValue(typeof(TypeReaderAttribute), out attributes))
        {
            if (underlyingType != null)
            {
                Type = underlyingType;
            }
            else
            {
                Type = type;
            }

            TypeReader = TypeReaderAttributeHelper.GetTypeReader<TContext, IInteractionTypeReader, InteractionTypeReader<TContext>>((TypeReaderAttribute)attributes[0]);
        }
        else if (underlyingType != null)
        {
            if (typeReaders.TryGetValue(type, out var typeReader) || typeReaders.TryGetValue(underlyingType, out typeReader))
            {
                TypeReader = typeReader;
            }
            else if (underlyingType.IsEnum)
            {
                TypeReader = options.EnumTypeReader;
            }
            else
                throw new TypeReaderNotFoundException($"Type name: '{underlyingType.FullName}' or '{type.FullName}'.");
            Type = underlyingType;
        }
        else
        {
            if (typeReaders.TryGetValue(type, out var typeReader))
                TypeReader = typeReader;
            else if (type.IsEnum)
                TypeReader = options.EnumTypeReader;
            else
                throw new TypeReaderNotFoundException($"Type name: '{type.FullName}'.");
            Type = type;
        }
    }
}
