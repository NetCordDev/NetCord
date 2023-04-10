using System.Reflection;

using NetCord.Services.Utils;

namespace NetCord.Services.Commands;

public class CommandParameter<TContext> where TContext : ICommandContext
{
    public CommandTypeReader<TContext> TypeReader { get; }
    public Type ElementType { get; }
    public Type NonNullableElementType { get; }
    public Type Type { get; }
    public bool HasDefaultValue { get; }
    public object? DefaultValue { get; }
    public bool Remainder { get; }
    public bool Params { get; }
    public IReadOnlyDictionary<Type, IReadOnlyList<Attribute>> Attributes { get; }
    public IReadOnlyList<ParameterPreconditionAttribute<TContext>> Preconditions { get; }

    internal CommandParameter(ParameterInfo parameter, MethodInfo method, CommandServiceConfiguration<TContext> configuration)
    {
        HasDefaultValue = parameter.HasDefaultValue;

        var attributesIEnumerable = parameter.GetCustomAttributes();
        Attributes = attributesIEnumerable.ToRankedDictionary(a => a.GetType());

        Type? typeReaderType;
        if (Attributes.TryGetValue(typeof(CommandParameterAttribute), out var attributes))
        {
            var commandParameterAttribute = (CommandParameterAttribute)attributes[0];
            Remainder = commandParameterAttribute.Remainder;
            typeReaderType = commandParameterAttribute.TypeReaderType;
        }
        else
            typeReaderType = null;

        Type type = Type = parameter.ParameterType;
        Type elementType;
        if (Attributes.ContainsKey(typeof(ParamArrayAttribute)))
        {
            Params = true;
            elementType = ElementType = type.GetElementType()!;
        }
        else
            elementType = ElementType = type;

        (TypeReader, NonNullableElementType, DefaultValue) = TypeReaderHelper.GetTypeInfo<TContext, ICommandTypeReader, CommandTypeReader<TContext>>(elementType, parameter, typeReaderType, configuration.TypeReaders, configuration.EnumTypeReader);

        Preconditions = ParameterPreconditionAttributeHelper.GetPreconditionAttributes<TContext>(attributesIEnumerable, method);
    }

    internal async Task EnsureCanExecuteAsync(object? value, TContext context)
    {
        var count = Preconditions.Count;
        for (var i = 0; i < count; i++)
        {
            var preconditionAttribute = Preconditions[i];
            await preconditionAttribute.EnsureCanExecuteAsync(value, context).ConfigureAwait(false);
        }
    }
}
