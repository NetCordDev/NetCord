using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Services.Helpers;

namespace NetCord.Services.Interactions;

public class InteractionInfo<TContext> where TContext : IInteractionContext
{
    public IReadOnlyList<InteractionParameter<TContext>> Parameters { get; }
    public Func<object?[]?, TContext, IServiceProvider?, ValueTask> InvokeAsync { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    internal InteractionInfo(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, InteractionServiceConfiguration<TContext> configuration)
    {
        var parameters = GetParameters(method.GetParameters(), method, configuration);
        Parameters = parameters;

        InvokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, parameters.Select(p => p.Type), configuration.ResultResolverProvider);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(declaringType, method);
    }

    private static InteractionParameter<TContext>[] GetParameters(ReadOnlySpan<ParameterInfo> parameters, MethodInfo method, InteractionServiceConfiguration<TContext> configuration)
    {
        var parametersLength = parameters.Length;

        var result = new InteractionParameter<TContext>[parametersLength];

        // We allow required parameters to follow optional parameters.
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            result[i] = new(parameter, method, configuration);
        }

        return result;
    }

    internal InteractionInfo(Delegate handler, InteractionServiceConfiguration<TContext> configuration)
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
