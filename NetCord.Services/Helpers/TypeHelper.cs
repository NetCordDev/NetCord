using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace NetCord.Services.Helpers;

internal static class TypeHelper
{
    public static Expression GetCreateInstanceExpression([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type moduleType, ParameterExpression serviceProvider)
    {
        var constructors = moduleType.GetConstructors();
        if (constructors.Length is 0)
            throw new InvalidOperationException($"No public constructors found for '{moduleType}'.");

        using var constructorsEnumerator = constructors.Select(c => (Constructor: c, Parameters: c.GetParameters())).OrderByDescending(t => t.Parameters.Length).GetEnumerator();

        constructorsEnumerator.MoveNext();

        var (constructor, parameters) = constructorsEnumerator.Current;

        List<Expression> expressions = [];

        var ret = Expression.Label(moduleType);

        if (parameters.Length is 0)
            expressions.Add(Expression.Return(ret, Expression.New(constructor), moduleType));
        else
        {
            var noServiceProvider = Expression.Label();
            expressions.Add(Expression.IfThen(Expression.Equal(serviceProvider, Expression.Constant(null, typeof(object))), Expression.Goto(noServiceProvider)));

            while (true)
            {
                if (parameters.All(p => p.IsOptional))
                {
                    expressions.Add(Expression.Return(ret, Expression.New(constructor, GetArguments(serviceProvider, parameters, Expression.Empty())), moduleType));

                    expressions.Add(Expression.Label(noServiceProvider));
                    expressions.Add(Expression.Return(ret, Expression.New(constructor, GetDefaultArguments(parameters)), moduleType));

                    break;
                }
                else if (constructorsEnumerator.MoveNext())
                {
                    var (nextConstructor, nextParameters) = constructorsEnumerator.Current;

                    if (nextParameters.Length is 0)
                    {
                        var next = noServiceProvider;

                        expressions.Add(Expression.Return(ret, Expression.New(constructor, GetArguments(serviceProvider, parameters, Expression.Goto(next))), moduleType));
                        expressions.Add(Expression.Label(next));

                        expressions.Add(Expression.Return(ret, Expression.New(nextConstructor), moduleType));

                        break;
                    }
                    else
                    {
                        var next = Expression.Label();

                        expressions.Add(Expression.Return(ret, Expression.New(constructor, GetArguments(serviceProvider, parameters, Expression.Goto(next))), moduleType));
                        expressions.Add(Expression.Label(next));

                        (constructor, parameters) = (nextConstructor, nextParameters);
                    }
                }
                else
                {
                    var next = noServiceProvider;

                    expressions.Add(Expression.Return(ret, Expression.New(constructor, GetArguments(serviceProvider, parameters, Expression.Goto(next))), moduleType));
                    expressions.Add(Expression.Label(next));

                    expressions.Add(Expression.Throw(Expression.New(typeof(InvalidOperationException).GetConstructor([typeof(string)])!, Expression.Constant($"Failed to initialize '{moduleType}'.", typeof(string)))));

                    break;
                }
            }
        }

        expressions.Add(Expression.Label(ret, Expression.Default(moduleType)));
        var block = Expression.Block(expressions);
        return block;
    }

    private static Expression[] GetArguments(ParameterExpression serviceProvider, ParameterInfo[] parameters, Expression notFound)
    {
        var parametersLength = parameters.Length;
        var arguments = new Expression[parametersLength];
        var argIndex = 0;
        for (int i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            arguments[argIndex++] = ServiceProviderHelper.GetGetServiceExpression(parameter, serviceProvider, notFound);
        }

        return arguments;
    }

    private static Expression[] GetDefaultArguments(ParameterInfo[] parameters)
    {
        var parametersLength = parameters.Length;
        var arguments = new Expression[parametersLength];
        for (int i = 0; i < parametersLength; i++)
        {
            var parameter = parameters[i];
            arguments[i] = ParameterHelper.GetParameterDefaultValueExpression(parameter.ParameterType, parameter);
        }

        return arguments;
    }
}
