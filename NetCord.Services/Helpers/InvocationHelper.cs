using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetCord.Services.Helpers;

internal class InvocationHelper
{
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "The method is called only in dynamic code")]
    public static Func<object?[]?, TContext, IServiceProvider?, Task> CreateDelegate<TContext>(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType, IEnumerable<Type> parameterTypes)
    {
        return RuntimeFeature.IsDynamicCodeCompiled
            ? GetCompiledExpressionTrees<TContext>(method, declaringType, parameterTypes)
            : GetReflectionBasedDelegate<TContext>(method, declaringType);
    }

    [RequiresUnreferencedCode("Compiling expression trees requires dynamic code")]
    private static Func<object?[]?, TContext, IServiceProvider?, Task> GetCompiledExpressionTrees<TContext>(MethodInfo method, Type declaringType, IEnumerable<Type> parameterTypes)
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
        var lambda = Expression.Lambda(call, parameters, context, serviceProvider);
        return (Func<object?[]?, TContext, IServiceProvider?, Task>)lambda.Compile();
    }

    private static Func<object?[]?, TContext, IServiceProvider?, Task> GetReflectionBasedDelegate<TContext>(MethodInfo method, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type declaringType)
    {
        if (method.IsStatic)
        {
            return (object?[]? parameters, TContext context, IServiceProvider? serviceProvider) =>
            {
                return Unsafe.As<Task>(method.Invoke(null, BindingFlags.DoNotWrapExceptions, null, parameters, null)!);
            };
        }
        else
        {
            var createModule = TypeHelper.GetCreateInstanceReflectionDelegate<IBaseModule<TContext>>(declaringType);
            return (object?[]? parameters, TContext context, IServiceProvider? serviceProvider) =>
            {
                var module = createModule(serviceProvider);
                module.SetContext(context);
                return Unsafe.As<Task>(method.Invoke(module, BindingFlags.DoNotWrapExceptions, null, parameters, null)!);
            };
        }
    }
}
