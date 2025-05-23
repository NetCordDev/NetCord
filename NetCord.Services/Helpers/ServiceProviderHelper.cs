using System.Linq.Expressions;
using System.Reflection;

namespace NetCord.Services.Helpers;

internal static class ServiceProviderHelper
{
    public static Expression GetGetServiceExpression(ParameterInfo parameter, ParameterExpression serviceProvider, IServiceResolverProvider serviceResolverProvider, Expression serviceNotFound)
    {
        return parameter.IsOptional ? GetOptionalGetServiceExpression(parameter, serviceProvider, serviceResolverProvider) : GetRequiredGetServiceExpression(parameter, serviceProvider, serviceResolverProvider, serviceNotFound);
    }

    private static BlockExpression GetOptionalGetServiceExpression(ParameterInfo parameter, ParameterExpression serviceProvider, IServiceResolverProvider serviceResolverProvider)
    {
        var parameterValueVariable = Expression.Variable(typeof(object));
        var parameterType = parameter.ParameterType;
        var argRet = Expression.Label(parameterType);

        var serviceResolver = serviceResolverProvider.GetResolver(parameter);

        return Expression.Block([parameterValueVariable],
            Expression.Assign(parameterValueVariable, Expression.Invoke(Expression.Constant(serviceResolver), serviceProvider)),
            Expression.IfThen(Expression.NotEqual(parameterValueVariable, Expression.Constant(null, typeof(object))),
                Expression.Return(argRet, Expression.Convert(parameterValueVariable, parameterType))),
            Expression.Label(argRet, ParameterHelper.GetParameterDefaultValueExpression(parameterType, parameter)));
    }

    private static BlockExpression GetRequiredGetServiceExpression(ParameterInfo parameter, ParameterExpression serviceProvider, IServiceResolverProvider serviceResolverProvider, Expression serviceNotFound)
    {
        var parameterValueVariable = Expression.Variable(typeof(object));
        var parameterType = parameter.ParameterType;
        var argRet = Expression.Label(parameterType);

        var serviceResolver = serviceResolverProvider.GetResolver(parameter);

        return Expression.Block([parameterValueVariable],
            Expression.Assign(parameterValueVariable, Expression.Invoke(Expression.Constant(serviceResolver), serviceProvider)),
            Expression.IfThenElse(Expression.Equal(parameterValueVariable, Expression.Constant(null, typeof(object))),
                serviceNotFound,
                Expression.Return(argRet, Expression.Convert(parameterValueVariable, parameterType))),
            Expression.Label(argRet, Expression.Default(parameterType)));
    }

    public static NewExpression GetServiceNotFoundExceptionExpression(ParameterInfo p)
    {
        return Expression.New(typeof(InvalidOperationException).GetConstructor([typeof(string)])!, Expression.Constant($"No service for type '{p.ParameterType}' has been registered.", typeof(string)));
    }
}
