using System.Reflection;

using NetCord.Services.Helpers;
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
        var attributes = attributesIEnumerable.ToRankedDictionary(a => a.GetType());
        Attributes = attributes;

        Type? typeReaderType;
        if (attributes.TryGetValue(typeof(CommandParameterAttribute), out var commandParameterAttributes))
        {
            var commandParameterAttribute = (CommandParameterAttribute)commandParameterAttributes[0];
            Remainder = commandParameterAttribute.Remainder;
            typeReaderType = commandParameterAttribute.TypeReaderType;
        }
        else
            typeReaderType = null;

        Type type = Type = parameter.ParameterType;
        Type elementType;
        if (attributes.ContainsKey(typeof(ParamArrayAttribute)))
        {
            Params = true;
            elementType = ElementType = type.GetElementType()!;
        }
        else
            elementType = ElementType = type;

        (TypeReader, NonNullableElementType, DefaultValue) = ParametersHelper.GetParameterInfo<TContext, ICommandTypeReader, CommandTypeReader<TContext>>(elementType, parameter, typeReaderType, configuration.TypeReaders, configuration.EnumTypeReader);

        Preconditions = PreconditionsHelper.GetParameterPreconditions<TContext>(attributesIEnumerable, method);
    }

    internal async ValueTask EnsureCanExecuteAsync(object? value, TContext context, IServiceProvider? serviceProvider)
    {
        var preconditions = Preconditions;
        var count = preconditions.Count;
        for (var i = 0; i < count; i++)
        {
            var preconditionAttribute = preconditions[i];
            await preconditionAttribute.EnsureCanExecuteAsync(value, context, serviceProvider).ConfigureAwait(false);
        }
    }
}
