using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using NetCord.Services.Helpers;

namespace NetCord.Services.Commands;

public class CommandInfo<TContext> where TContext : ICommandContext
{
    public int Priority { get; }
    public IReadOnlyList<CommandParameter<TContext>> Parameters { get; }
    public Func<object?[]?, TContext, IServiceProvider?, ValueTask> InvokeAsync { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    internal CommandInfo(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, int priority, CommandServiceConfiguration<TContext> configuration)
    {
        Priority = priority;

        var parameters = GetParameters(method.GetParameters(), method, configuration);
        Parameters = parameters;

        InvokeAsync = InvocationHelper.CreateModuleDelegate(method, declaringType, parameters.Select(p => p.Type), configuration.ResultResolverProvider);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(declaringType, method);
    }

    internal CommandInfo(Delegate handler, int priority, CommandServiceConfiguration<TContext> configuration)
    {
        Priority = priority;

        var method = handler.Method;

        var split = ParametersHelper.SplitHandlerParameters<TContext>(method);

        var parameters = GetParameters(split.Parameters, method, configuration);
        Parameters = parameters;

        InvokeAsync = InvocationHelper.CreateHandlerDelegate(handler, split.Services, split.HasContext, parameters.Select(p => p.Type), configuration.ResultResolverProvider);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(method);
    }

    private static CommandParameter<TContext>[] GetParameters(ReadOnlySpan<ParameterInfo> parameters, MethodInfo method, CommandServiceConfiguration<TContext> configuration)
    {
        var parametersLength = parameters.Length;

        var result = new CommandParameter<TContext>[parametersLength];
        var hasDefaultValue = false;
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];

            if (parameter.HasDefaultValue)
                hasDefaultValue = true;
            else if (hasDefaultValue)
                throw new InvalidDefinitionException("Optional parameters must appear after all required parameters.", method);

            result[i] = new(parameter, method, configuration);
        }

        return result;
    }

    internal ValueTask<PreconditionResult> EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
        => PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider);
}
