using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetCord.Services.Helpers;

internal static class ParametersHelper
{
    public readonly ref struct SplitParameters(IEnumerable<ParameterInfo> services, bool hasContext, ReadOnlySpan<ParameterInfo> parameters)
    {
        public IEnumerable<ParameterInfo> Services { get; } = services;
        public bool HasContext { get; } = hasContext;
        public ReadOnlySpan<ParameterInfo> Parameters { get; } = parameters;
    }

    public static SplitParameters SplitHandlerParameters<TContext>(MethodInfo method)
    {
        var methodParameters = method.GetParameters();

        var contextType = typeof(TContext);
        var index = Array.FindIndex(methodParameters, p => p.ParameterType == contextType);
        bool context = index >= 0;

        return new(methodParameters.Take(index), context, methodParameters.AsSpan(index + 1));
    }

    public static (TTypeReader TypeReader, Type NonNullableType, object? DefaultValue) GetParameterInfo<TContext, TTypeReaderBase, TTypeReader>(Type type, ParameterInfo parameter, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type? typeReaderType, ImmutableDictionary<Type, TTypeReader> typeReaders, TTypeReader enumTypeReader)
    {
        TTypeReader resultTypeReader;
        Type resultNonNullableType;
        object? resultDefaultValue;

        var underlyingType = Nullable.GetUnderlyingType(type);
        if (typeReaderType is null)
        {
            if (underlyingType is null)
            {
                resultDefaultValue = parameter.HasDefaultValue ? ParameterHelper.GetNonUnderlyingTypeDefaultValue(type, parameter) : null;

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
                resultDefaultValue = parameter.HasDefaultValue ? ParameterHelper.GetUnderlyingTypeDefaultValue(underlyingType, parameter) : null;

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
            if (underlyingType is null)
            {
                resultDefaultValue = parameter.HasDefaultValue ? ParameterHelper.GetNonUnderlyingTypeDefaultValue(type, parameter) : null;
                resultNonNullableType = type;
            }
            else
            {
                resultDefaultValue = parameter.HasDefaultValue ? ParameterHelper.GetUnderlyingTypeDefaultValue(underlyingType, parameter) : null;
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
}
