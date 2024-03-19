using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace NetCord.Services.Helpers;

internal class InvocationHelper
{
    public static Func<object?[]?, TContext, IServiceProvider?, ValueTask> CreateModuleDelegate<TContext>(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, IEnumerable<Type> parameterTypes, IResultResolverProvider<TContext> resultResolverProvider)
    {
        var parameters = Expression.Parameter(typeof(object?[]));
        var context = Expression.Parameter(typeof(TContext));
        var serviceProvider = Expression.Parameter(typeof(IServiceProvider));
        Expression? instance;
        if (method.IsStatic)
            instance = null;
        else
        {
            var module = Expression.Variable(declaringType);
            instance = Expression.Block([module],
                                        Expression.Assign(module, TypeHelper.GetCreateInstanceExpression(declaringType, serviceProvider)),
                                        Expression.Call(module, typeof(IBaseModule<TContext>).GetMethod(nameof(IBaseModule<TContext>.SetContext), BindingFlags.Instance | BindingFlags.NonPublic)!, context),
                                        module);
        }

        var call = Expression.Call(instance,
                                   method,
                                   parameterTypes.Select((p, i) => Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i, typeof(int))), p)));

        var resolver = GetResolver(method, resultResolverProvider);
        var invokeResolver = GetInvokeResolverExpression(method, context, call, resolver);

        var lambda = Expression.Lambda<Func<object?[]?, TContext, IServiceProvider?, ValueTask>>(invokeResolver, parameters, context, serviceProvider);
        return lambda.Compile();
    }

    public static Func<object?[]?, TContext, IServiceProvider?, ValueTask> CreateHandlerDelegate<TContext>(Delegate handler, IEnumerable<ParameterInfo> serviceParameters, bool hasContext, IEnumerable<Type> parameterTypes, IResultResolverProvider<TContext> resultResolverProvider)
    {
        var parameters = Expression.Parameter(typeof(object?[]));
        var context = Expression.Parameter(typeof(TContext));
        var serviceProvider = Expression.Parameter(typeof(IServiceProvider));

        var serviceExpressions = serviceParameters.Select(p => ServiceProviderHelper.GetGetServiceExpression(p, serviceProvider, Expression.Throw(ServiceProviderHelper.GetServiceNotFoundExceptionExpression(p))));
        var parameterExpressions = parameterTypes.Select((p, i) => Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i, typeof(int))), p));

        var arguments = hasContext ? serviceExpressions.Append(context).Concat(parameterExpressions) : serviceExpressions.Concat(parameterExpressions);

        var invoke = Expression.Invoke(Expression.Constant(handler), arguments);

        var method = handler.Method;

        var resolver = GetResolver(method, resultResolverProvider);
        var invokeResolver = GetInvokeResolverExpression(method, context, invoke, resolver);

        var lambda = Expression.Lambda<Func<object?[]?, TContext, IServiceProvider?, ValueTask>>(invokeResolver, parameters, context, serviceProvider);
        return lambda.Compile();
    }

    private static Func<object?, TContext, ValueTask> GetResolver<TContext>(MethodInfo method, IResultResolverProvider<TContext> resultResolverProvider)
    {
        var type = method.ReturnType;
        if (resultResolverProvider.TryGetResolver(type, out var resolver))
            return resolver;

        throw new InvalidDefinitionException($"The return type '{type}' is not supported by '{resultResolverProvider.GetType()}'.", method);
    }

    private static Expression GetInvokeResolverExpression<TContext>(MethodInfo method, ParameterExpression context, Expression call, Func<object?, TContext, ValueTask> resolver)
    {
        if (method.ReturnType == typeof(void))
        {
            return Expression.Block(call,
                                    Expression.Invoke(Expression.Constant(resolver),
                                                      Expression.Constant(null, typeof(object)),
                                                      context));
        }
        else
        {
            return Expression.Invoke(Expression.Constant(resolver),
                                     Expression.Convert(call, typeof(object)),
                                     context);
        }
    }
}
