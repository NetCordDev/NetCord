using System.Linq.Expressions;

using NetCord.Services.Helpers;

namespace NetCord.Hosting;

internal class DelegateHandlerHelper
{
    public static TDelegate CreateHandler<TDelegate>(Delegate handler, IEnumerable<Type> values)
    {
        var parameters = values.ToDictionary(v => v, Expression.Parameter);
        var serviceProvider = Expression.Parameter(typeof(IServiceProvider));

        var method = handler.Method;
        var methodParameters = method.GetParameters();
        var arguments = methodParameters.Select(p =>
        {
            return parameters.TryGetValue(p.ParameterType, out var parameter)
                ? parameter
                : ServiceProviderHelper.GetGetServiceExpression(p, serviceProvider, Expression.Throw(ServiceProviderHelper.GetServiceNotFoundExceptionExpression(p)));
        });

        var call = Expression.Invoke(Expression.Constant(handler), arguments);
        var returnType = method.ReturnType;

        Expression @return;
        if (returnType == typeof(ValueTask))
            @return = call;
        else if (returnType == typeof(Task))
            @return = Expression.New(typeof(ValueTask).GetConstructor([typeof(Task)])!, call);
        else if (returnType == typeof(void))
            @return = Expression.Block(call, Expression.Constant(default(ValueTask), typeof(ValueTask)));
        else
            throw new InvalidOperationException($"The return type '{returnType}' is not supported. Only '{nameof(ValueTask)}', {nameof(Task)} and 'void' are supported.");

        var lambda = Expression.Lambda<TDelegate>(@return, parameters.Values.Append(serviceProvider));
        return lambda.Compile();
    }
}
