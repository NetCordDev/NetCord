using System.Linq.Expressions;
using System.Reflection;

namespace NetCord.Services.Commands;

public class CommandInfo<TContext> where TContext : ICommandContext
{
    public int Priority { get; }
    public IReadOnlyList<CommandParameter<TContext>> Parameters { get; }
    public Func<object?[]?, TContext, Task> InvokeAsync { get; }
    public IReadOnlyList<PreconditionAttribute<TContext>> Preconditions { get; }

    internal CommandInfo(MethodInfo method, CommandAttribute attribute, CommandServiceConfiguration<TContext> configuration)
    {
        if (method.ReturnType != typeof(Task))
            throw new InvalidDefinitionException($"Commands must return '{typeof(Task)}'.", method);

        Priority = attribute.Priority;
        var declaringType = method.DeclaringType!;

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

        InvokeAsync = CreateDelegate(method, declaringType, p);

        Preconditions = PreconditionAttributeHelper.GetPreconditionAttributes<TContext>(declaringType, method);
    }

    private static Func<object?[]?, TContext, Task> CreateDelegate(MethodInfo method, Type declaringType, CommandParameter<TContext>[] commandParameters)
    {
        var parameters = Expression.Parameter(typeof(object?[]));
        Type contextType = typeof(TContext);
        var context = Expression.Parameter(contextType);
        Expression? instance;
        if (method.IsStatic)
            instance = null;
        else
        {
            var module = Expression.Variable(declaringType);
            instance = Expression.Block(new[] { module },
                                        Expression.Assign(module, Expression.New(declaringType)),
                                        Expression.Assign(Expression.Property(module, declaringType.GetProperty(nameof(BaseCommandModule<TContext>.Context), contextType)!), context),
                                        module);
        }
        var call = Expression.Call(instance,
                                   method,
                                   commandParameters.Select((p, i) => Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i)), p.Type)));
        var lambda = Expression.Lambda(call, parameters, context);
        return (Func<object?[]?, TContext, Task>)lambda.Compile();
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
