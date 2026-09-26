using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace NetCord.Services.Helpers;

file static class InvocationHelper<TContext>
{
    internal static readonly MethodInfo _baseModuleSetContextMethod = typeof(IBaseModule<TContext>).GetMethod(nameof(IBaseModule<>.SetContext), BindingFlags.Instance | BindingFlags.NonPublic)!;
}

internal static class InvocationHelper
{
    private static readonly MethodInfo _valueTaskAsTaskMethod = typeof(ValueTask).GetMethod(nameof(ValueTask.AsTask), BindingFlags.Instance | BindingFlags.Public)!;
    private static readonly MethodInfo _valueTaskOfTAsTaskMethodUnbound = typeof(ValueTask<>).GetMethod(nameof(ValueTask<>.AsTask), BindingFlags.Instance | BindingFlags.Public)!;

    public static Func<object?[]?, TContext, IServiceProvider?, ValueTask> CreateModuleDelegate<TContext>(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, IEnumerable<Type> parameterTypes, IResultResolverProvider<TContext> resultResolverProvider, IServiceResolverProvider serviceResolverProvider)
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
                                        Expression.Assign(module, TypeHelper.GetCreateInstanceExpression(declaringType, serviceProvider, serviceResolverProvider)),
                                        Expression.Call(module, InvocationHelper<TContext>._baseModuleSetContextMethod, context),
                                        module);
        }

        var call = Expression.Call(instance,
                                   method,
                                   parameterTypes.Select((p, i) => Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i, typeof(int))), p)));

        var invokeResolver = GetInvokeResolverExpression(method, context, call, resultResolverProvider);

        var lambda = Expression.Lambda<Func<object?[]?, TContext, IServiceProvider?, ValueTask>>(invokeResolver, parameters, context, serviceProvider);
        return lambda.Compile();
    }

    public static Func<object?[]?, TContext, IServiceProvider?, ValueTask> CreateHandlerDelegate<TContext>(Delegate handler, IEnumerable<ParameterInfo> serviceParameters, bool hasContext, IEnumerable<Type> parameterTypes, IResultResolverProvider<TContext> resultResolverProvider, IServiceResolverProvider serviceResolverProvider)
    {
        var parameters = Expression.Parameter(typeof(object?[]));
        var context = Expression.Parameter(typeof(TContext));
        var serviceProvider = Expression.Parameter(typeof(IServiceProvider));

        var serviceExpressions = serviceParameters.Select(p => ServiceProviderHelper.GetGetServiceExpression(p, serviceProvider, serviceResolverProvider, Expression.Throw(ServiceProviderHelper.GetServiceNotFoundExceptionExpression(p))));
        var parameterExpressions = parameterTypes.Select((p, i) => Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i, typeof(int))), p));

        var arguments = hasContext ? serviceExpressions.Append(context).Concat(parameterExpressions) : serviceExpressions.Concat(parameterExpressions);

        var invoke = Expression.Invoke(Expression.Constant(handler), arguments);

        var method = handler.Method;

        var invokeResolver = GetInvokeResolverExpression(method, context, invoke, resultResolverProvider);

        var lambda = Expression.Lambda<Func<object?[]?, TContext, IServiceProvider?, ValueTask>>(invokeResolver, parameters, context, serviceProvider);
        return lambda.Compile();
    }

    [DoesNotReturn]
    private static void ThrowResolverTypeNotSupportedException<TContext>(MethodInfo method, Type returnType, IResultResolverProvider<TContext> resultResolverProvider)
    {
        throw new InvalidDefinitionException($"The return type '{returnType}' is not supported by '{resultResolverProvider.GetType()}'.", method);
    }

    [DoesNotReturn]
    private static void ThrowResolverBothTypesNotSupportedException<TContext>(MethodInfo method, Type returnType1, Type returnType2, IResultResolverProvider<TContext> resultResolverProvider)
    {
        throw new InvalidDefinitionException($"The return types '{returnType1}' and '{returnType2}' are not supported by '{resultResolverProvider.GetType()}'.", method);
    }

    private static Expression GetInvokeResolverExpression<TContext>(MethodInfo method, ParameterExpression context, Expression call, IResultResolverProvider<TContext> resultResolverProvider)
    {
        var returnType = method.ReturnType;

        if (resultResolverProvider.TryGetResolver(returnType, out var resolver))
        {
            if (returnType == typeof(void))
                return Expression.Block(call,
                                        Expression.Invoke(Expression.Constant(resolver, typeof(Func<object?, TContext, ValueTask>)),
                                                          Expression.Constant(null, typeof(object)),
                                                          context));

            return CreateResultResolverCallExpression(context, call, resolver);
        }

        return GetAlternativeInvokeResolverExpression(method, returnType, context, call, resultResolverProvider);
    }

    private static InvocationExpression GetAlternativeInvokeResolverExpression<TContext>(MethodInfo method, Type returnType, ParameterExpression context, Expression call, IResultResolverProvider<TContext> resultResolverProvider)
    {
        Func<object?, TContext, ValueTask>? resolver = null;

        if (returnType == typeof(ValueTask))
        {
            var alternativeReturnType = typeof(Task);

            if (!resultResolverProvider.TryGetResolver(alternativeReturnType, out resolver))
                ThrowResolverBothTypesNotSupportedException(method, returnType, alternativeReturnType, resultResolverProvider);

            call = Expression.Call(call, _valueTaskAsTaskMethod);
        }
        else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(ValueTask<>))
        {
            var asTaskMethod = (MethodInfo)returnType.GetMemberWithSameMetadataDefinitionAs(_valueTaskOfTAsTaskMethodUnbound);

            var alternativeReturnType = asTaskMethod.ReturnType;

            if (!resultResolverProvider.TryGetResolver(alternativeReturnType, out resolver))
                ThrowResolverBothTypesNotSupportedException(method, returnType, alternativeReturnType, resultResolverProvider);

            call = Expression.Call(call, asTaskMethod);
        }
        else
            ThrowResolverTypeNotSupportedException(method, returnType, resultResolverProvider);

        return CreateResultResolverCallExpression(context, call, resolver);
    }

    private static InvocationExpression CreateResultResolverCallExpression<TContext>(ParameterExpression context, Expression call, Func<object?, TContext, ValueTask> resolver)
    {
        return Expression.Invoke(Expression.Constant(resolver, typeof(Func<object?, TContext, ValueTask>)),
                                 Expression.Convert(call, typeof(object)),
                                 context);
    }
}
