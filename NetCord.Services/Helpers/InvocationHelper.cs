using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetCord.Services.Helpers;

internal class InvocationHelper
{
    public static Func<object?[]?, TContext, IServiceProvider?, Task> CreateDelegate<TContext>(MethodInfo method, Type declaringType, IEnumerable<Type> parameterTypes)
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
                                        Expression.Assign(Expression.Property(module, declaringType.GetProperty("Context", contextType)!), context),
                                        module);
        }
        var call = Expression.Call(instance,
                                   method,
                                   parameterTypes.Select((p, i) => Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i, typeof(int))), p)));
        var lambda = Expression.Lambda(call, parameters, context, serviceProvider);
        return (Func<object?[]?, TContext, IServiceProvider?, Task>)lambda.Compile();
    }
}
