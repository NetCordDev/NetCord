using System.Reflection;

namespace NetCord.Services.Interactions;

public class InteractionInfo<TContext> where TContext : InteractionContext
{
    public Type DeclaringType { get; }
    public IReadOnlyList<InteractionParameter<TContext>> Parameters { get; }
    public Func<object, object[], Task> InvokeAsync { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    public InteractionInfo(MethodInfo methodInfo, InteractionServiceOptions<TContext> options)
    {
        if (methodInfo.ReturnType != typeof(Task))
            throw new InvalidDefinitionException($"Interactions must return '{typeof(Task).FullName}'.", methodInfo);

        DeclaringType = methodInfo.DeclaringType!;

        var parameters = methodInfo.GetParameters();
        var parametersLength = parameters.Length;
        var p = new InteractionParameter<TContext>[parametersLength];
        for (var i = 0; i < parametersLength; i++)
            p[i] = new(parameters[i], options);
        Parameters = p;

        InvokeAsync = (obj, parameters) => (Task)methodInfo.Invoke(obj, BindingFlags.DoNotWrapExceptions, null, parameters, null)!;

        Preconditions = PreconditionAttributeHelper.GetPreconditionAttributes<TContext>(methodInfo, DeclaringType);
    }

    internal async Task EnsureCanExecuteAsync(TContext context)
    {
        foreach (var preconditionAttribute in Preconditions)
            await preconditionAttribute.EnsureCanExecuteAsync(context).ConfigureAwait(false);
    }
}
