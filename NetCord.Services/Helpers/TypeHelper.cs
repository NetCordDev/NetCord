using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace NetCord.Services.Helpers;

internal static class TypeHelper
{
    public static Expression GetCreateInstanceExpression([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type moduleType, ParameterExpression serviceProvider)
    {
        var constructors = moduleType.GetConstructors();
        if (constructors.Length == 0)
            throw new InvalidOperationException($"No public constructors found for '{moduleType}'.");

        var expressions = new Expression[(constructors.Length * 2) + 2];
        var ret = Expression.Label(moduleType);

        int expressionIndex = 0;
        foreach (var (constructor, parameters) in constructors.Select(c => (Constructor: c, Parameters: c.GetParameters())).OrderByDescending(t => t.Parameters.Length))
        {
            var next = Expression.Label();
            var parametersLength = parameters.Length;
            var arguments = new Expression[parametersLength];
            var argIndex = 0;
            for (int i = 0; i < parametersLength; i++)
            {
                var parameter = parameters[i];
                arguments[argIndex++] = ServiceProviderHelper.GetGetServiceExpression(parameter, serviceProvider, Expression.Goto(next));
            }

            expressions[expressionIndex++] = Expression.Return(ret, Expression.New(constructor, arguments), moduleType);
            expressions[expressionIndex++] = Expression.Label(next);
        }

        expressions[expressionIndex++] = Expression.Throw(Expression.New(typeof(InvalidOperationException).GetConstructor([typeof(string)])!, Expression.Constant($"Failed to initialize '{moduleType}'.", typeof(string))));
        expressions[expressionIndex] = Expression.Label(ret, Expression.Default(moduleType));
        return Expression.Block(expressions);
    }
}
