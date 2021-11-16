using System.Reflection;

namespace NetCord.Commands;

public record CommandParameter<TContext> where TContext : ICommandContext
{
    public Func<string, TContext, CommandServiceOptions<TContext>, Task<object>>? ReadAsync { get; }
    public Type Type { get; }
    public bool HasDefaultValue { get; }
    public object? DefaultValue { get; }
    public bool Remainder { get; }
    public bool EnumTypeReader { get; }
    public bool Params { get; }


    internal CommandParameter(ParameterInfo parameter, Dictionary<Type, Func<string, TContext, CommandServiceOptions<TContext>, Task<object>>> typeReaders)
    {
        HasDefaultValue = parameter.HasDefaultValue;

        Remainder = parameter.GetCustomAttribute<RemainderAttribute>() != null;

        Type type;
        if (parameter.GetCustomAttribute<ParamArrayAttribute>() != null)
        {
            Params = true;
            type = parameter.ParameterType.GetElementType();
        }
        else
            type = parameter.ParameterType;

        var underlyingType = Nullable.GetUnderlyingType(type);
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
                EnumTypeReader = true;
            }
            else
                throw new TypeReaderNotFoundException("Type name: " + underlyingType.FullName + " or " + Type.FullName);
            Type = underlyingType;
        }
        else
        {
            if (HasDefaultValue)
                DefaultValue = parameter.DefaultValue;

            if (typeReaders.TryGetValue(type, out var typeReader))
                ReadAsync = typeReader;
            else if (type.IsEnum)
                EnumTypeReader = true;
            else
                throw new TypeReaderNotFoundException("Type name: " + Type.FullName);
            Type = type;
        }
    }
}