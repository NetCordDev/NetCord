using System.Reflection;

namespace NetCord.Commands;

public record CommandParameter<TContext> where TContext : ICommandContext
{
    public CommandServiceOptions<TContext>.TypeReader? ReadAsync { get; }
    public Type Type { get; }
    public bool HasDefaultValue { get; }
    public object? DefaultValue { get; }
    public bool Remainder { get; }
    public bool Params { get; }
    public IReadOnlyDictionary<Guid, Attribute> Attributes { get; }
    public string? Name { get; }
    public string? Description { get; }

    internal CommandParameter(ParameterInfo parameter, CommandServiceOptions<TContext> options)
    {
        HasDefaultValue = parameter.HasDefaultValue;

        Attributes = parameter.GetCustomAttributes().ToDictionary(a => a.GetType().GUID);
        Remainder = Attributes.TryGetValue(typeof(RemainderAttribute).GUID, out _);

        Type type;

        if (Attributes.TryGetValue(typeof(CommandParameterAttribute).GUID, out Attribute? commandParameterAttributeAttribute))
        {
            var commandParameterAttribute = (CommandParameterAttribute)commandParameterAttributeAttribute;
            Name = commandParameterAttribute.Name;
            Description = commandParameterAttribute.Description;
        }

        if (Attributes.ContainsKey(typeof(ParamArrayAttribute).GUID))
        {
            Params = true;
            type = parameter.ParameterType.GetElementType()!;
        }
        else
            type = parameter.ParameterType;

        var underlyingType = Nullable.GetUnderlyingType(type);

        var typeReaders = options.TypeReaders;

        if (Attributes.TryGetValue(typeof(TypeReaderAttribute<,>).GUID, out var typeReaderAttribute) || Attributes.TryGetValue(typeof(TypeReaderAttribute<>).GUID, out typeReaderAttribute))
        {
            if (underlyingType != null)
            {
                if (HasDefaultValue)
                {
                    var d = parameter.DefaultValue;
                    if (underlyingType.IsEnum && d != null)
                        DefaultValue = Enum.ToObject(underlyingType, d);
                    else
                        DefaultValue = d;
                }
                Type = underlyingType;
            }
            else
            {
                if (HasDefaultValue)
                    DefaultValue = parameter.DefaultValue;
                Type = type;
            }

            ReadAsync = ((dynamic)typeReaderAttribute).ReadAsync;
        }
        else
        {
            if (underlyingType != null)
            {
                if (typeReaders.TryGetValue(type, out var typeReader) || typeReaders.TryGetValue(underlyingType, out typeReader))
                {
                    if (HasDefaultValue)
                    {
                        var d = parameter.DefaultValue;
                        if (underlyingType.IsEnum && d != null)
                            DefaultValue = Enum.ToObject(underlyingType, d);
                        else
                            DefaultValue = d;
                    }
                    ReadAsync = typeReader;
                }
                else if (underlyingType.IsEnum)
                {
                    if (HasDefaultValue)
                    {
                        var d = parameter.DefaultValue;
                        if (d != null)
                            DefaultValue = Enum.ToObject(underlyingType, d);
                        else
                            DefaultValue = d;
                    }
                    ReadAsync = options.EnumTypeReader;
                }
                else
                    throw new TypeReaderNotFoundException("Type name: " + underlyingType.FullName + " or " + type.FullName);
                Type = underlyingType;
            }
            else
            {
                if (HasDefaultValue)
                    DefaultValue = parameter.DefaultValue;

                if (typeReaders.TryGetValue(type, out var typeReader))
                    ReadAsync = typeReader;
                else if (type.IsEnum)
                    ReadAsync = options.EnumTypeReader;
                else
                    throw new TypeReaderNotFoundException("Type name: " + type.FullName);
                Type = type;
            }
        }
    }
}