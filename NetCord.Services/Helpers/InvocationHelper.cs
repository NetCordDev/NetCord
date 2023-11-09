using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetCord.Services.Helpers;

internal class InvocationHelper
{
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "The method is called only in dynamic code")]
    public static Func<object?[]?, TContext, IServiceProvider?, ValueTask> CreateDelegate<TContext>(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, IEnumerable<Type> parameterTypes, IResultResolverProvider<TContext> resultResolverProvider)
    {
        return RuntimeFeature.IsDynamicCodeCompiled
            ? GetCompiledExpressionTrees(method, declaringType, parameterTypes, resultResolverProvider)
            : GetReflectionBasedDelegate(method, declaringType, resultResolverProvider);
    }

    [RequiresUnreferencedCode("Compiling expression trees requires dynamic code")]
    private static Func<object?[]?, TContext, IServiceProvider?, ValueTask> GetCompiledExpressionTrees<TContext>(MethodInfo method, Type declaringType, IEnumerable<Type> parameterTypes, IResultResolverProvider<TContext> resultResolverProvider)
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
                                        Expression.Call(module, typeof(IBaseModule<TContext>).GetMethod(nameof(IBaseModule<TContext>.SetContext), BindingFlags.Instance | BindingFlags.NonPublic)!, context),
                                        module);
        }

        var call = Expression.Call(instance,
                                   method,
                                   parameterTypes.Select((p, i) => Expression.Convert(Expression.ArrayIndex(parameters, Expression.Constant(i, typeof(int))), p)));

        var resolver = GetResolver(method, resultResolverProvider);

        Expression invokeResolver;
        if (method.ReturnType == typeof(void))
        {
            invokeResolver = Expression.Block(call,
                                              Expression.Invoke(Expression.Constant(resolver),
                                                                Expression.Constant(null, typeof(object)),
                                                                context));
        }
        else
        {
            invokeResolver = Expression.Invoke(Expression.Constant(resolver),
                                               Expression.Convert(call, typeof(object)),
                                               context);
        }

        var lambda = Expression.Lambda(invokeResolver, parameters, context, serviceProvider);
        return (Func<object?[]?, TContext, IServiceProvider?, ValueTask>)lambda.Compile();
    }

    private static Func<object?[]?, TContext, IServiceProvider?, ValueTask> GetReflectionBasedDelegate<TContext>(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, IResultResolverProvider<TContext> resultResolverProvider)
    {
        var resolver = GetResolver(method, resultResolverProvider);

        if (method.IsStatic)
        {
            return (object?[]? parameters, TContext context, IServiceProvider? serviceProvider) =>
            {
                return resolver(method.Invoke(null, BindingFlags.DoNotWrapExceptions, null, parameters, null), context);
            };
        }
        else
        {
            var createModule = TypeHelper.GetCreateInstanceReflectionDelegate<IBaseModule<TContext>>(declaringType);
            return (object?[]? parameters, TContext context, IServiceProvider? serviceProvider) =>
            {
                var module = createModule(serviceProvider);
                module.SetContext(context);
                return resolver(method.Invoke(module, BindingFlags.DoNotWrapExceptions, null, parameters, null), context);
            };
        }
    }

    private static Func<object?, TContext, ValueTask> GetResolver<TContext>(MethodInfo method, IResultResolverProvider<TContext> resultResolverProvider)
    {
        var type = method.ReturnType;
        if (resultResolverProvider.TryGetResolver(type, out var resolver))
            return resolver;

        throw new InvalidDefinitionException($"The return type '{type}' is not supported by '{resultResolverProvider.GetType()}'.", method);
    }
}
