using System.Reflection;

namespace NetCord.Services;

internal static class TypeReaderHelper
{
    public static (TTypeReader TypeReader, Type NonNullableType, object? DefaultValue) GetTypeInfo<TContext, TTypeReaderBase, TTypeReader>(Type type, ParameterInfo parameter, Type? typeReaderType, Dictionary<Type, TTypeReader> typeReaders, TTypeReader enumTypeReader) where TContext : IContext
    {
        TTypeReader resultTypeReader;
        Type resultNonNullableType;
        object? resultDefaultValue;

        var underlyingType = Nullable.GetUnderlyingType(type);
        if (typeReaderType == null)
        {
            if (underlyingType == null)
            {
                resultDefaultValue = parameter.HasDefaultValue ? GetNonUnderlyingTypeDefaultValue(type, parameter) : null;

                if (typeReaders.TryGetValue(type, out var typeReader))
                    resultTypeReader = typeReader;
                else if (type.IsEnum)
                    resultTypeReader = enumTypeReader;
                else
                    throw new TypeReaderNotFoundException(type);

                resultNonNullableType = type;
            }
            else
            {
                resultDefaultValue = parameter.HasDefaultValue ? GetUnderlyingTypeDefaultValue(underlyingType, parameter) : null;

                if (typeReaders.TryGetValue(type, out var typeReader) || typeReaders.TryGetValue(underlyingType, out typeReader))
                    resultTypeReader = typeReader;
                else if (underlyingType.IsEnum)
                    resultTypeReader = enumTypeReader;
                else
                    throw new TypeReaderNotFoundException(type, underlyingType);

                resultNonNullableType = underlyingType;
            }
        }
        else
        {
            if (underlyingType == null)
            {
                resultDefaultValue = parameter.HasDefaultValue ? GetNonUnderlyingTypeDefaultValue(type, parameter) : null;
                resultNonNullableType = type;
            }
            else
            {
                resultDefaultValue = parameter.HasDefaultValue ? GetUnderlyingTypeDefaultValue(underlyingType, parameter) : null;
                resultNonNullableType = underlyingType;
            }

            var typeReader = Activator.CreateInstance(typeReaderType)!;

            if (typeReader is not TTypeReaderBase typeReaderBase)
                throw new InvalidOperationException($"'{typeReaderType}' must inherit from '{typeof(TTypeReader)}'.");

            if (typeReaderBase is not TTypeReader castedTypeReader)
                throw new InvalidOperationException($"Context of '{typeReaderType}' is not convertible to '{typeof(TContext)}'.");

            resultTypeReader = castedTypeReader;
        }
        return (resultTypeReader, resultNonNullableType, resultDefaultValue);
    }

    private static object? GetNonUnderlyingTypeDefaultValue(Type type, ParameterInfo parameter)
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

    private static object? GetUnderlyingTypeDefaultValue(Type type, ParameterInfo parameter)
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

    private static object? GetUnderlyingEnumDefaultValue(Type type, ParameterInfo parameter)
    {
        var defaultValue = parameter.DefaultValue;
        return defaultValue is null ? null : Enum.ToObject(type, defaultValue);
    }

    private static readonly MethodInfo _getDefaultValueMethodInfo = typeof(TypeReaderHelper).GetMethod(nameof(GetDefaultValue), 1, BindingFlags.Static | BindingFlags.NonPublic, null, Type.EmptyTypes, null)!;

    private static object? GetDefaultValue(Type type) => _getDefaultValueMethodInfo.MakeGenericMethod(type).Invoke(null, null);

    private static object? GetDefaultValue<T>() => default(T);
}
