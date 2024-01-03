using System.Linq.Expressions;
using System.Reflection;

namespace NetCord.Services.Helpers;

internal static class ServiceProviderHelper
{
    private static readonly MethodInfo _getServiceMethod = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService), BindingFlags.Instance | BindingFlags.Public)!;

    public static Expression GetGetServiceExpression(ParameterInfo parameter, ParameterExpression serviceProvider, Expression serviceNotFound)
    {
        var parameterValueVariable = Expression.Variable(typeof(object));
        var parameterType = parameter.ParameterType;
        var argRet = Expression.Label(parameterType);
        if (parameter.IsOptional)
            return Expression.Block(new[] { parameterValueVariable },
                Expression.Assign(parameterValueVariable, Expression.Call(serviceProvider, _getServiceMethod, Expression.Constant(parameterType, typeof(Type)))),
                Expression.IfThen(Expression.NotEqual(parameterValueVariable, Expression.Constant(null, typeof(object))),
                    Expression.Return(argRet, Expression.Convert(parameterValueVariable, parameterType))),
                Expression.Label(argRet, ParametersHelper.GetParameterDefaultValueExpression(parameterType, parameter)));
        else
            return Expression.Block(new[] { parameterValueVariable },
                Expression.Assign(parameterValueVariable, Expression.Call(serviceProvider, _getServiceMethod, Expression.Constant(parameterType, typeof(Type)))),
                Expression.IfThenElse(Expression.Equal(parameterValueVariable, Expression.Constant(null, typeof(object))),
                    serviceNotFound,
                    Expression.Return(argRet, Expression.Convert(parameterValueVariable, parameterType))),
                Expression.Label(argRet, Expression.Default(parameterType)));
    }
}
