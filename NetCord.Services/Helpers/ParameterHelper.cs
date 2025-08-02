using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NetCord.Services.Helpers;

internal static class ParameterHelper
{
    public static object? GetNonUnderlyingTypeDefaultValue(Type type, ParameterInfo parameter)
    {
        if (!type.IsValueType)
            return GetNonUnderlyingTypeRawDefaultValue(parameter, out _);
        else if (type.IsPrimitive)
        {
            var defaultValue = GetNonUnderlyingTypeRawDefaultValue(parameter, out bool mightNeedConversion);

            if (mightNeedConversion)
            {
                if (type == typeof(nint))
                    return (nint)(int)defaultValue!;
                else if (type == typeof(nuint))
                    return (nuint)(uint)defaultValue!;
            }

            return defaultValue;
        }
        else if (type == typeof(decimal) || type.IsEnum)
            return GetNonUnderlyingTypeRawDefaultValue(parameter, out _);
        else
            return GetUninitializedObject(type);
    }

    public static object? GetUnderlyingTypeDefaultValue(Type underlyingType, ParameterInfo parameter)
    {
        if (underlyingType.IsPrimitive)
        {
            var defaultValue = GetUnderlyingTypeRawDefaultValue(parameter, out bool mightNeedConversion);

            if (mightNeedConversion)
            {
                if (underlyingType == typeof(nint))
                    return defaultValue is null ? null : (nint)(int)defaultValue;
                else if (underlyingType == typeof(nuint))
                    return defaultValue is null ? null : (nuint)(uint)defaultValue;
            }

            return defaultValue;
        }
        else if (underlyingType == typeof(decimal))
            return GetUnderlyingTypeRawDefaultValue(parameter, out _);
        else if (underlyingType.IsEnum)
            return GetUnderlyingEnumDefaultValue(underlyingType, parameter);
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
            return Expression.Constant(GetNonUnderlyingTypeRawDefaultValue(parameter, out _), type);
        else if (type.IsPrimitive)
        {
            var defaultValue = GetNonUnderlyingTypeRawDefaultValue(parameter, out bool mightNeedConversion);

            if (mightNeedConversion)
            {
                if (type == typeof(nint))
                    return Expression.Constant((nint)(int)defaultValue!, type);
                else if (type == typeof(nuint))
                    return Expression.Constant((nuint)(uint)defaultValue!, type);
            }

            return Expression.Constant(defaultValue, type);
        }
        else if (type == typeof(decimal) || type.IsEnum)
            return Expression.Constant(GetNonUnderlyingTypeRawDefaultValue(parameter, out _), type);
        else
            return Expression.Default(type);
    }

    public static Expression GetUnderlyingTypeDefaultValueExpression(Type type, Type underlyingType, ParameterInfo parameter)
    {
        if (underlyingType.IsPrimitive)
        {
            var defaultValue = GetUnderlyingTypeRawDefaultValue(parameter, out bool mightNeedConversion);

            if (mightNeedConversion)
            {
                if (underlyingType == typeof(nint))
                    return Expression.Constant(defaultValue is null ? null : (nint)(int)defaultValue, type);
                else if (underlyingType == typeof(nuint))
                    return Expression.Constant(defaultValue is null ? null : (nuint)(uint)defaultValue, type);
            }

            return Expression.Constant(defaultValue, type);
        }
        else if (underlyingType == typeof(decimal))
            return Expression.Constant(GetUnderlyingTypeRawDefaultValue(parameter, out _), type);
        else if (underlyingType.IsEnum)
            return Expression.Constant(GetUnderlyingEnumDefaultValue(underlyingType, parameter), type);
        else
            return Expression.Default(type);
    }

    public static object? GetUnderlyingEnumDefaultValue(Type type, ParameterInfo parameter)
    {
        var defaultValue = GetUnderlyingTypeRawDefaultValue(parameter, out bool mightNeedConversion);
        return mightNeedConversion && defaultValue is not null ? Enum.ToObject(type, defaultValue) : defaultValue;
    }

    private static object? GetNonUnderlyingTypeRawDefaultValue(ParameterInfo parameter, out bool mightNeedConversion)
    {
        if (parameter.HasDefaultValue)
        {
            var defaultValue = parameter.DefaultValue;

            // The default value may be null for non-nullable value types, in which case we return the default value for that type
            if (defaultValue is not null)
            {
                mightNeedConversion = true;
                return defaultValue;
            }
        }

        mightNeedConversion = false;

        var type = parameter.ParameterType;
        return type.IsValueType ? GetUninitializedObject(type) : null;
    }

    private static object? GetUnderlyingTypeRawDefaultValue(ParameterInfo parameter, out bool mightNeedConversion)
    {
        if (mightNeedConversion = parameter.HasDefaultValue)
            return parameter.DefaultValue;

        return null;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2067:Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.", Justification = "This does not actually require constructors to work")]
    private static object? GetUninitializedObject(Type type) => RuntimeHelpers.GetUninitializedObject(type);
}
