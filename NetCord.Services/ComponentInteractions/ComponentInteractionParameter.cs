using System.Reflection;

using NetCord.Services.Helpers;
using NetCord.Services.Utils;

namespace NetCord.Services.ComponentInteractions;

public class ComponentInteractionParameter<TContext> where TContext : IComponentInteractionContext
{
    public string Name { get; }
    public ComponentInteractionTypeReader<TContext> TypeReader { get; }
    public Type ElementType { get; }
    public Type NonNullableElementType { get; }
    public Type Type { get; }
    public bool IsOptional { get; }
    public object? DefaultValue { get; }
    public bool Params { get; }
    public IReadOnlyDictionary<Type, IReadOnlyList<Attribute>> Attributes { get; }
    public IReadOnlyList<ParameterPreconditionAttribute<TContext>> Preconditions { get; }

    internal ComponentInteractionParameter(ParameterInfo parameter, MethodInfo method, ComponentInteractionServiceConfiguration<TContext> configuration)
    {
        IsOptional = parameter.IsOptional;

        var parameterAttributes = Attribute.GetCustomAttributes(parameter);
        var attributes = Attributes = parameterAttributes.ToRankedFrozenDictionary(a => a.GetType());

        Type? typeReaderType;
        if (attributes.TryGetValue(typeof(ComponentInteractionParameterAttribute), out var componentInteractionParameterAttributes))
        {
            var commandParameterAttribute = (ComponentInteractionParameterAttribute)componentInteractionParameterAttributes[0];
            Name = commandParameterAttribute.Name ?? parameter.Name!;
            typeReaderType = commandParameterAttribute.TypeReaderType;
        }
        else
        {
            Name = parameter.Name!;
            typeReaderType = null;
        }

        var (_, elementType) = (Params, ElementType) = ParametersHelper.GetParamsInfo(parameter, Type = parameter.ParameterType, attributes, method);

        (TypeReader, NonNullableElementType, DefaultValue) = ParametersHelper.GetParameterInfo<TContext, IInteractionTypeReader, ComponentInteractionTypeReader<TContext>>(elementType, parameter, typeReaderType, configuration.TypeReaders, configuration.EnumTypeReader);

        Preconditions = PreconditionsHelper.GetParameterPreconditions<TContext>(parameterAttributes, method);
    }

    public async ValueTask<ComponentInteractionTypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, ComponentInteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        try
        {
            return await TypeReader.ReadAsync(input, context, this, configuration, serviceProvider).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ComponentInteractionTypeReaderExceptionResult(ex);
        }
    }

    internal ValueTask<PreconditionResult> EnsureCanExecuteAsync(object? value, TContext context, IServiceProvider? serviceProvider)
    {
        return PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, value, context, serviceProvider);
    }
}
