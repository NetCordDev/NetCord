using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetCord.Services;

internal static class TypeHelper
{
    [RequiresUnreferencedCode("This method requires dynamic code")]
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

    public static Func<IServiceProvider?, T> GetCreateInstanceReflectionDelegate<T>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type moduleType) where T : class
    {
        var constructors = moduleType.GetConstructors();
        if (constructors.Length == 0)
            throw new InvalidOperationException($"No public constructors found for '{moduleType}'.");

        var constructorsAndParameters = constructors.Select(c =>
        {
            var parameters = c.GetParameters().Select(p =>
            {
                var parameterType = p.ParameterType;
                var isOptional = p.IsOptional;
                return (ParameterType: parameterType, IsOptional: isOptional, DefaultValue: isOptional ? ParameterHelper.GetParameterDefaultValue(parameterType, p) : null);
            }).ToArray();

            return (Constructor: c, Parameters: parameters);
        }).OrderByDescending(t => t.Parameters.Length).ToArray();

        return (IServiceProvider? serviceProvider) =>
        {
            foreach (var (constructor, parameters) in constructorsAndParameters)
            {
                var length = parameters.Length;
                var arguments = new object?[length];
                for (int i = 0; i < length; i++)
                {
                    var parameter = parameters[i];
                    var parameterType = parameter.ParameterType;
                    var argument = serviceProvider!.GetService(parameterType);
                    if (argument is null)
                    {
                        if (parameter.IsOptional)
                            argument = parameter.DefaultValue;
                        else
                            goto Next;
                    }
                    arguments[i] = argument;
                }

                return Unsafe.As<T>(constructor.Invoke(BindingFlags.DoNotWrapExceptions, null, arguments, null));

                Next:;
            }

            throw new InvalidOperationException($"Failed to initialize '{moduleType}'.");
        };
    }
}
