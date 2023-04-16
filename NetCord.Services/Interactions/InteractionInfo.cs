using System.Linq.Expressions;
using System.Reflection;

namespace NetCord.Services.Interactions;

public class InteractionInfo<TContext> where TContext : InteractionContext
{
    public IReadOnlyList<InteractionParameter<TContext>> Parameters { get; }
    public Func<object?[]?, TContext, IServiceProvider?, Task> InvokeAsync { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    internal InteractionInfo(MethodInfo method, InteractionServiceConfiguration<TContext> configuration)
    {
        if (method.ReturnType != typeof(Task))
            throw new InvalidDefinitionException($"Interactions must return '{typeof(Task)}'.", method);

        var declaringType = method.DeclaringType!;

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

        InvokeAsync = CreateDelegate(method, declaringType, p);

        Preconditions = PreconditionAttributeHelper.GetPreconditionAttributes<TContext>(declaringType, method);
    }

    private static Func<object?[]?, TContext, IServiceProvider?, Task> CreateDelegate(MethodInfo method, Type declaringType, InteractionParameter<TContext>[] interactionParameters)
    {
        var parameters = Expression.Parameter(typeof(object?[]));
        var contextType = typeof(TContext);
        var context = Expression.Parameter(contextType);
        var serviceProvider = Expression.Parameter(typeof(IServiceProvider));
        Expression? instance;
        if (method.IsStatic)
            instance = null;
        else
        {
            var module = Expression.Variable(declaringType);
            instance = Expression.Block(new[] { module },
                                        Expression.Assign(module, TypeHelper.GetCreateInstanceExpression(declaringType, serviceProvider)),
                                        Expression.Assign(Expression.Property(module, declaringType.GetProperty(nameof(BaseInteractionModule<TContext>.Context), contextType)!), context),
                                        module);
        }
        var call = Expression.Call(instance,
                                   method,
                                   interactionParameters.Select((p, i) => Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i)), p.Type)));
        var lambda = Expression.Lambda(call, parameters, context, serviceProvider);
        return (Func<object?[]?, TContext, IServiceProvider?, Task>)lambda.Compile();
    }

    internal async Task EnsureCanExecuteAsync(TContext context)
    {
        var count = Preconditions.Count;
        for (var i = 0; i < count; i++)
        {
            var preconditionAttribute = Preconditions[i];
            await preconditionAttribute.EnsureCanExecuteAsync(context).ConfigureAwait(false);
        }
    }
}
