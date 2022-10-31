using System.Reflection;

namespace NetCord.Services.Interactions;

public class InteractionInfo<TContext> where TContext : InteractionContext
{
    public Type DeclaringType { get; }
    public IReadOnlyList<InteractionParameter<TContext>> Parameters { get; }
    public Func<object, object[], Task> InvokeAsync { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    public InteractionInfo(MethodInfo method, InteractionServiceOptions<TContext> options)
    {
        if (method.ReturnType != typeof(Task))
            throw new InvalidDefinitionException($"Interactions must return '{typeof(Task).FullName}'.", method);

        DeclaringType = method.DeclaringType!;

        var parameters = method.GetParameters();
        var parametersLength = parameters.Length;
        var p = new InteractionParameter<TContext>[parametersLength];
        for (var i = 0; i < parametersLength; i++)
            p[i] = new(parameters[i], method, options);
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
