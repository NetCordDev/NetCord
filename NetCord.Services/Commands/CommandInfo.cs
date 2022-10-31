using System.Reflection;

namespace NetCord.Services.Commands;

public record CommandInfo<TContext> where TContext : ICommandContext
{
    public Type DeclaringType { get; }
    public IReadOnlyList<CommandParameter<TContext>> Parameters { get; }
    public int Priority { get; }
    public Permission RequiredBotPermissions { get; }
    public Permission RequiredBotChannelPermissions { get; }
    public Permission RequiredUserPermissions { get; }
    public Permission RequiredUserChannelPermissions { get; }
    public Func<object, object[], Task> InvokeAsync { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    public CommandInfo(MethodInfo method, CommandAttribute attribute, CommandServiceOptions<TContext> options)
    {
        if (method.ReturnType != typeof(Task))
            throw new InvalidDefinitionException($"Commands must return '{typeof(Task).FullName}'.", method);

        Priority = attribute.Priority;
        DeclaringType = method.DeclaringType!;

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
            p[i] = new(parameter, method, options);
        }
        Parameters = p;

        InvokeAsync = (obj, parameters) => (Task)method.Invoke(obj, BindingFlags.DoNotWrapExceptions, null, parameters, null)!;

        Preconditions = PreconditionAttributeHelper.GetPreconditionAttributes<TContext>(DeclaringType, method);
    }

    internal async Task EnsureCanExecuteAsync(TContext context)
    {
        foreach (var preconditionAttribute in Preconditions)
            await preconditionAttribute.EnsureCanExecuteAsync(context).ConfigureAwait(false);
    }
}
