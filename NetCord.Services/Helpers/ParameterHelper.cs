using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetCord.Services.Helpers;

internal static class ParameterHelper
{
    public static object? GetParameterDefaultValue(Type type, ParameterInfo parameter)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        return underlyingType is null
            ? GetNonUnderlyingTypeDefaultValue(type, parameter)
            : GetUnderlyingTypeDefaultValue(underlyingType, parameter);
    }

    public static object? GetNonUnderlyingTypeDefaultValue(Type type, ParameterInfo parameter)
    {
        if (!type.IsValueType)
            return parameter.DefaultValue;
        else if (type.IsPrimitive)
        {
            var defaultValue = parameter.DefaultValue;
            if (type == typeof(nint))
                return (nint)(int)defaultValue!;
            else if (type == typeof(nuint))
                return (nuint)(uint)defaultValue!;
            else
                return defaultValue;
        }
        else if (type == typeof(decimal) || type.IsEnum)
            return parameter.DefaultValue;
        else
            return GetDefaultValue(type);
    }

    public static object? GetUnderlyingTypeDefaultValue(Type type, ParameterInfo parameter)
    {
        if (type.IsPrimitive)
        {
            var defaultValue = parameter.DefaultValue;
            if (type == typeof(nint))
                return defaultValue is null ? null : (nint)(int)defaultValue;
            else if (type == typeof(nuint))
                return defaultValue is null ? null : (nuint)(uint)defaultValue;
            else
                return defaultValue;
        }
        else if (type == typeof(decimal))
            return parameter.DefaultValue;
        else if (type.IsEnum)
            return GetUnderlyingEnumDefaultValue(type, parameter);
        else
            return null;
    }

    public static Expression GetParameterDefaultValueExpression(Type type, ParameterInfo parameter)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        return underlyingType is null
            ? GetNonUnderlyingTypeDefaultValueExpression(type, parameter)
            : GetUnderlyingTypeDefaultValueExpression(type, underlyingType, parameter);
    }

    public static Expression GetNonUnderlyingTypeDefaultValueExpression(Type type, ParameterInfo parameter)
    {
        if (!type.IsValueType)
            return Expression.Constant(parameter.DefaultValue, type);
        else if (type.IsPrimitive)
        {
            var defaultValue = parameter.DefaultValue;
            if (type == typeof(nint))
                return Expression.Constant((nint)(int)defaultValue!, type);
            else if (type == typeof(nuint))
                return Expression.Constant((nuint)(uint)defaultValue!, type);
            else
                return Expression.Constant(defaultValue, type);
        }
        else if (type == typeof(decimal) || type.IsEnum)
            return Expression.Constant(parameter.DefaultValue, type);
        else
            return Expression.Default(type);
    }

    public static Expression GetUnderlyingTypeDefaultValueExpression(Type type, Type underlyingType, ParameterInfo parameter)
    {
        if (underlyingType.IsPrimitive)
        {
            var defaultValue = parameter.DefaultValue;
            if (underlyingType == typeof(nint))
                return Expression.Constant(defaultValue is null ? null : (nint)(int)defaultValue, type);
            else if (underlyingType == typeof(nuint))
                return Expression.Constant(defaultValue is null ? null : (nuint)(uint)defaultValue, type);
            else
                return Expression.Constant(defaultValue, type);
        }
        else if (underlyingType == typeof(decimal))
            return Expression.Constant(parameter.DefaultValue, type);
        else if (underlyingType.IsEnum)
            return Expression.Constant(GetUnderlyingEnumDefaultValue(underlyingType, parameter), type);
        else
            return Expression.Default(type);
    }

    public static object? GetUnderlyingEnumDefaultValue(Type type, ParameterInfo parameter)
    {
        var defaultValue = parameter.DefaultValue;
        return defaultValue is null ? null : Enum.ToObject(type, defaultValue);
    }

    [UnconditionalSuppressMessage("Trimming", "IL2067:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.", Justification = "This does not actually require constructors to work")]
    private static object? GetDefaultValue(Type type) => RuntimeHelpers.GetUninitializedObject(type);
}
