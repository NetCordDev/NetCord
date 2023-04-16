using System.Linq.Expressions;
using System.Reflection;

namespace NetCord.Services;

internal static class TypeHelper
{
    public static Expression GetCreateInstanceExpression(Type moduleType, ParameterExpression serviceProvider)
    {
        var constructors = moduleType.GetConstructors();
        if (constructors.Length == 0)
            throw new InvalidOperationException($"No public constructors found for '{moduleType}'.");

        var getServiceMethod = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService), BindingFlags.Instance | BindingFlags.Public)!;
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
                var parameterValueVariable = Expression.Variable(typeof(object));
                var parameterType = parameter.ParameterType;
                var argRet = Expression.Label(parameterType);
                if (parameter.IsOptional)
                    arguments[argIndex] = Expression.Block(new[] { parameterValueVariable },
                        Expression.Assign(parameterValueVariable, Expression.Call(serviceProvider, getServiceMethod, Expression.Constant(parameterType, typeof(Type)))),
                        Expression.IfThen(Expression.NotEqual(parameterValueVariable, Expression.Constant(null, typeof(object))),
                            Expression.Return(argRet, Expression.Convert(parameterValueVariable, parameterType))),
                        Expression.Label(argRet, ParameterHelper.GetParameterDefaultValueExpression(parameterType, parameter)));
                else
                    arguments[argIndex] = Expression.Block(new[] { parameterValueVariable },
                        Expression.Assign(parameterValueVariable, Expression.Call(serviceProvider, getServiceMethod, Expression.Constant(parameterType, typeof(Type)))),
                        Expression.IfThenElse(Expression.Equal(parameterValueVariable, Expression.Constant(null, typeof(object))),
                            Expression.Goto(next),
                            Expression.Return(argRet, Expression.Convert(parameterValueVariable, parameterType))),
                        Expression.Label(argRet, Expression.Default(parameterType)));

                argIndex++;
            }

            expressions[expressionIndex++] = Expression.Return(ret, Expression.New(constructor, arguments), moduleType);
            expressions[expressionIndex++] = Expression.Label(next);
        }

        expressions[expressionIndex++] = Expression.Throw(Expression.New(typeof(InvalidOperationException).GetConstructor(new[] { typeof(string) })!, Expression.Constant($"Failed to initialize '{moduleType}'.", typeof(string))));
        expressions[expressionIndex] = Expression.Label(ret, Expression.Default(moduleType));
        return Expression.Block(expressions);
    }
}
