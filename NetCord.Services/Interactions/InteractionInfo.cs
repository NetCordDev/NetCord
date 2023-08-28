using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Services.Helpers;

namespace NetCord.Services.Interactions;

public class InteractionInfo<TContext> where TContext : IInteractionContext
{
    public IReadOnlyList<InteractionParameter<TContext>> Parameters { get; }
    public Func<object?[]?, TContext, IServiceProvider?, Task> InvokeAsync { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    internal InteractionInfo(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, InteractionServiceConfiguration<TContext> configuration)
    {
        MethodHelper.EnsureMethodReturnTypeValid(method);

        var parameters = method.GetParameters();
        var parametersLength = parameters.Length;

        var p = new InteractionParameter<TContext>[parametersLength];

        // We allow required parameters to follow optional parameters.
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            p[i] = new(parameter, method, configuration);
        }
        Parameters = p;

        InvokeAsync = InvocationHelper.CreateDelegate<TContext>(method, declaringType, p.Select(p => p.Type));

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(declaringType, method);
    }

    internal ValueTask EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
        => PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider);
}
