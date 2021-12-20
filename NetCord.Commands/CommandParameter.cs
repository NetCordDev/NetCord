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
    public IReadOnlyDictionary<Type, Attribute> Attributes { get; }

    internal CommandParameter(ParameterInfo parameter, CommandServiceOptions<TContext> options)
    {
        HasDefaultValue = parameter.HasDefaultValue;

        Attributes = parameter.GetCustomAttributes().ToDictionary(a => a.GetType());
        Remainder = Attributes.TryGetValue(typeof(RemainderAttribute), out _);

        Type type;
        if (Attributes.TryGetValue(typeof(ParamArrayAttribute), out _))
        {
            Params = true;
            type = parameter.ParameterType.GetElementType()!;
        }
        else
            type = parameter.ParameterType;

        var underlyingType = Nullable.GetUnderlyingType(type);

        var typeReaders = options.TypeReaders;
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