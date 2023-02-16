using System.Reflection;

using NetCord.Services.Utils;

namespace NetCord.Services.Commands;

public record CommandParameter<TContext> where TContext : ICommandContext
{
    public CommandTypeReader<TContext> TypeReader { get; }
    public Type NullableType { get; }
    public Type Type { get; }
    public bool HasDefaultValue { get; }
    public object? DefaultValue { get; }
    public bool Remainder { get; }
    public bool Params { get; }
    public IReadOnlyDictionary<Type, IReadOnlyList<Attribute>> Attributes { get; }
    public string? Name { get; }
    public string? Description { get; }
    public IReadOnlyList<ParameterPreconditionAttribute<TContext>> Preconditions { get; }

    internal CommandParameter(ParameterInfo parameter, MethodInfo method, CommandServiceConfiguration<TContext> configuration)
    {
        HasDefaultValue = parameter.HasDefaultValue;

        var attributesIEnumerable = parameter.GetCustomAttributes();
        Attributes = attributesIEnumerable.ToRankedDictionary(a => a.GetType());
        Remainder = Attributes.ContainsKey(typeof(RemainderAttribute));

        if (Attributes.TryGetValue(typeof(ParameterAttribute), out IReadOnlyList<Attribute>? attributes))
        {
            var commandParameterAttribute = (ParameterAttribute)attributes[0];
            Name = commandParameterAttribute.Name;
            Description = commandParameterAttribute.Description;
        }

        Type type;
        if (Attributes.ContainsKey(typeof(ParamArrayAttribute)))
        {
            Params = true;
            type = parameter.ParameterType.GetElementType()!;
        }
        else
            type = parameter.ParameterType;

        NullableType = type;
        var underlyingType = Nullable.GetUnderlyingType(type);

        var typeReaders = configuration.TypeReaders;

        if (Attributes.TryGetValue(typeof(TypeReaderAttribute), out attributes))
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

            TypeReader = TypeReaderAttributeHelper.GetTypeReader<TContext, ICommandTypeReader, CommandTypeReader<TContext>>((TypeReaderAttribute)attributes[0]);
        }
        else if (underlyingType != null)
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
                TypeReader = typeReader;
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
                TypeReader = configuration.EnumTypeReader;
            }
            else
                throw new TypeReaderNotFoundException($"Type name: '{underlyingType.FullName}' or '{type.FullName}'.");

            Type = underlyingType;
        }
        else
        {
            if (HasDefaultValue)
                DefaultValue = parameter.DefaultValue;

            if (typeReaders.TryGetValue(type, out var typeReader))
                TypeReader = typeReader;
            else if (type.IsEnum)
                TypeReader = configuration.EnumTypeReader;
            else
                throw new TypeReaderNotFoundException($"Type name: '{type.FullName}'.");

            Type = type;
        }

        Preconditions = ParameterPreconditionAttributeHelper.GetPreconditionAttributes<TContext>(attributesIEnumerable, method);
    }

    internal async Task EnsureCanExecuteAsync(object? value, TContext context)
    {
        var count = Preconditions.Count;
        for (var i = 0; i < count; i++)
        {
            ParameterPreconditionAttribute<TContext>? preconditionAttribute = Preconditions[i];
            await preconditionAttribute.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
        }
    }
}
