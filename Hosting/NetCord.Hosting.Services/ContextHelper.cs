using System.Linq.Expressions;

namespace NetCord.Hosting.Services;

internal static class ContextHelper
{
    public static Func<TArgument1, TArgument2, IServiceProvider, TContext> CreateContextDelegate<TArgument1, TArgument2, [DAM(DAMT.PublicConstructors)] TContext>()
    {
        var argument1Type = typeof(TArgument1);
        var argument2Type = typeof(TArgument2);
        var contextType = typeof(TContext);

        var constructors = contextType.GetConstructors();
        if (constructors.Length == 0)
            throw new InvalidOperationException($"No public constructors found for '{contextType}'.");

        var argument1 = Expression.Parameter(argument1Type);
        var argument2 = Expression.Parameter(argument2Type);
        var serviceProvider = Expression.Parameter(typeof(IServiceProvider));

        var expressions = new Expression[(constructors.Length * 2) + 2];
        var ret = Expression.Label(contextType);

        var expressionIndex = 0;
        foreach (var (constructor, parameters) in constructors.Select(c => (Constructor: c, Parameters: c.GetParameters())).OrderByDescending(t => t.Parameters.Length))
        {
            var next = Expression.Label();
            var parametersLength = parameters.Length;
            var arguments = new Expression[parametersLength];
            var argIndex = 0;
            for (var i = 0; i < parametersLength; i++)
            {
                var parameter = parameters[i];
                var parameterType = parameter.ParameterType;
                if (argument1Type.IsAssignableTo(parameterType))
                    arguments[argIndex++] = argument1;
                else if (argument2Type.IsAssignableTo(parameterType))
                    arguments[argIndex++] = argument2;
                else
                    arguments[argIndex++] = NetCord.Services.Helpers.ServiceProviderHelper.GetGetServiceExpression(parameter, serviceProvider, Expression.Goto(next));
            }

            expressions[expressionIndex++] = Expression.Return(ret, Expression.New(constructor, arguments), contextType);
            expressions[expressionIndex++] = Expression.Label(next);
        }

        expressions[expressionIndex++] = Expression.Throw(Expression.New(typeof(InvalidOperationException).GetConstructor([typeof(string)])!, Expression.Constant($"Failed to initialize '{contextType}'.", typeof(string))));
        expressions[expressionIndex] = Expression.Label(ret, Expression.Default(contextType));

        var block = Expression.Block(expressions);
        var lambda = Expression.Lambda<Func<TArgument1, TArgument2, IServiceProvider, TContext>>(block, argument1, argument2, serviceProvider);
        return lambda.Compile();
    }
}
