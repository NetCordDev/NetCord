using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Services.Helpers;

namespace NetCord.Services.ComponentInteractions;

public class ComponentInteractionInfo<TContext> where TContext : IComponentInteractionContext
{
    public IReadOnlyList<ComponentInteractionParameter<TContext>> Parameters { get; }
    public Func<object?[]?, TContext, IServiceProvider?, ValueTask> InvokeAsync { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    internal ComponentInteractionInfo(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, ComponentInteractionServiceConfiguration<TContext> configuration)
    {
        var parameters = GetParameters(method.GetParameters(), method, configuration);
        Parameters = parameters;

        InvokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, parameters.Select(p => p.Type), configuration.ResultResolverProvider);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(declaringType, method);
    }

    private static ComponentInteractionParameter<TContext>[] GetParameters(ReadOnlySpan<ParameterInfo> parameters, MethodInfo method, ComponentInteractionServiceConfiguration<TContext> configuration)
    {
        var parametersLength = parameters.Length;

        var result = new ComponentInteractionParameter<TContext>[parametersLength];

        // We allow required parameters to follow optional parameters.
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            result[i] = new(parameter, method, configuration);
        }

        return result;
    }

    internal ComponentInteractionInfo(Delegate handler, ComponentInteractionServiceConfiguration<TContext> configuration)
    {
        var method = handler.Method;

        var split = ParametersHelper.SplitHandlerParameters<TContext>(method);

        var parameters = GetParameters(split.Parameters, method, configuration);
        Parameters = parameters;

        InvokeAsync = InvocationHelper.CreateHandlerDelegate(handler, split.Services, split.HasContext, parameters.Select(p => p.Type), configuration.ResultResolverProvider);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method);
    }

    internal ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
        => PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider);
}
