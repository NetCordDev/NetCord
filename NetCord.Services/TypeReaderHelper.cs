using System.Reflection;

namespace NetCord.Services;

internal static class TypeReaderHelper
{
    public static (TTypeReader TypeReader, Type Type, object? DefaultValue) GetTypeInfo<TContext, TTypeReaderBase, TTypeReader>(Type type, ParameterInfo parameter, Type? typeReaderType, Dictionary<Type, TTypeReader> typeReaders, TTypeReader enumTypeReader) where TContext : IContext
    {
        TTypeReader resultTypeReader;
        Type resultType;
        object? resultDefaultValue;

        var underlyingType = Nullable.GetUnderlyingType(type);
        if (typeReaderType != null)
        {
            if (underlyingType != null)
            {
                if (parameter.HasDefaultValue)
                {
                    var defaultValue = parameter.DefaultValue;
                    resultDefaultValue = defaultValue != null && underlyingType.IsEnum ? Enum.ToObject(underlyingType, defaultValue) : defaultValue;
                }
                else
                    resultDefaultValue = null;

                resultType = underlyingType;
            }
            else
            {
                resultDefaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;
                resultType = type;
            }

            var typeReader = Activator.CreateInstance(typeReaderType)!;

            if (typeReader is not TTypeReaderBase typeReaderBase)
                throw new InvalidOperationException($"'{typeReaderType}' must inherit from '{typeof(TTypeReader)}'.");

            if (typeReaderBase is not TTypeReader castedTypeReader)
                throw new InvalidOperationException($"Context of '{typeReaderType}' is not convertible to '{typeof(TContext)}'.");

            resultTypeReader = castedTypeReader;
        }
        else if (underlyingType != null)
        {
            if (typeReaders.TryGetValue(type, out var typeReader) || typeReaders.TryGetValue(underlyingType, out typeReader))
            {
                if (parameter.HasDefaultValue)
                {
                    var defaultValue = parameter.DefaultValue;
                    resultDefaultValue = defaultValue != null && underlyingType.IsEnum ? Enum.ToObject(underlyingType, defaultValue) : defaultValue;
                }
                else
                    resultDefaultValue = null;

                resultTypeReader = typeReader;
            }
            else if (underlyingType.IsEnum)
            {
                if (parameter.HasDefaultValue)
                {
                    var defaultValue = parameter.DefaultValue;
                    resultDefaultValue = defaultValue != null ? Enum.ToObject(underlyingType, defaultValue) : defaultValue;
                }
                else
                    resultDefaultValue = null;

                resultTypeReader = enumTypeReader;
            }
            else
                throw new TypeReaderNotFoundException($"Type name: '{underlyingType}' or '{type}'.");

            resultType = underlyingType;
        }
        else
        {
            resultDefaultValue = parameter.HasDefaultValue ? parameter.DefaultValue : null;

            if (typeReaders.TryGetValue(type, out var typeReader))
                resultTypeReader = typeReader;
            else if (type.IsEnum)
                resultTypeReader = enumTypeReader;
            else
                throw new TypeReaderNotFoundException($"Type name: '{type}'.");

            resultType = type;
        }
        return (resultTypeReader, resultType, resultDefaultValue);
    }
}
