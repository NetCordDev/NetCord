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

    internal CommandInfo(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, CommandAttribute attribute, CommandServiceConfiguration<TContext> configuration)
    {
        Priority = attribute.Priority;

        var parameters = method.GetParameters();
        var parametersLength = parameters.Length;

        var p = new CommandParameter<TContext>[parametersLength];
        var hasDefaultValue = false;
        for (var i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];

            if (parameter.HasDefaultValue)
                hasDefaultValue = true;
            else if (hasDefaultValue)
                throw new InvalidDefinitionException($"Optional parameters must appear after all required parameters.", method);

            p[i] = new(parameter, method, configuration);
        }
        Parameters = p;

        InvokeAsync = InvocationHelper.CreateDelegate(method, declaringType, p.Select(p => p.Type), configuration.ResultResolverProvider);

        Preconditions = PreconditionsHelper.GetPreconditions<TContext>(declaringType, method);
    }

    internal ValueTask EnsureCanExecuteAsync(TContext context, IServiceProvider? serviceProvider)
        => PreconditionsHelper.EnsureCanExecuteAsync(Preconditions, context, serviceProvider);
}
