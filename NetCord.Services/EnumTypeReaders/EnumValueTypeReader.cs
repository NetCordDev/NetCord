using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace NetCord.Services.EnumTypeReaders;

internal static class EnumValueTypeReader
{
    public unsafe static IEnumTypeReader Create(Type enumType, CultureInfo cultureInfo)
    {
        return Create(enumType, EnumHelper.GetFields(enumType), cultureInfo);
    }

    public unsafe static IEnumTypeReader Create(Type enumType, FieldInfo[] fields, CultureInfo cultureInfo)
    {
        return Type.GetTypeCode(enumType) switch
        {
            TypeCode.Int32 => new EnumValueTypeReader<int>(enumType, fields, TryParseDelegateHelper.Create<int>(&int.TryParse, NumberStyles.AllowLeadingSign, cultureInfo)),
            TypeCode.SByte => new EnumValueTypeReader<sbyte>(enumType, fields, TryParseDelegateHelper.Create<sbyte>(&sbyte.TryParse, NumberStyles.AllowLeadingSign, cultureInfo)),
            TypeCode.Int16 => new EnumValueTypeReader<short>(enumType, fields, TryParseDelegateHelper.Create<short>(&short.TryParse, NumberStyles.AllowLeadingSign, cultureInfo)),
            TypeCode.Int64 => new EnumValueTypeReader<long>(enumType, fields, TryParseDelegateHelper.Create<long>(&long.TryParse, NumberStyles.AllowLeadingSign, cultureInfo)),
            TypeCode.UInt32 => new EnumValueTypeReader<uint>(enumType, fields, TryParseDelegateHelper.Create<uint>(&uint.TryParse, NumberStyles.None, cultureInfo)),
            TypeCode.Byte => new EnumValueTypeReader<byte>(enumType, fields, TryParseDelegateHelper.Create<byte>(&byte.TryParse, NumberStyles.None, cultureInfo)),
            TypeCode.UInt16 => new EnumValueTypeReader<ushort>(enumType, fields, TryParseDelegateHelper.Create<ushort>(&ushort.TryParse, NumberStyles.None, cultureInfo)),
            TypeCode.UInt64 => new EnumValueTypeReader<ulong>(enumType, fields, TryParseDelegateHelper.Create<ulong>(&ulong.TryParse, NumberStyles.None, cultureInfo)),
            TypeCode.Char => new EnumValueTypeReader<char>(enumType, fields, TryParseChar),
            TypeCode.Boolean => new EnumValueTypeReader<bool>(enumType, fields, bool.TryParse),
            _ => throw new InvalidOperationException($"Enums of type '{enumType.GetEnumUnderlyingType()}' are not supported."),
        };

        static bool TryParseChar(ReadOnlySpan<char> input, out char value)
        {
            if (input.Length == 1)
            {
                value = input[0];
                return true;
            }

            value = default;
            return false;
        }
    }
}

internal class EnumValueTypeReader<T> : IEnumTypeReader where T : struct
{
    private readonly TryParseDelegate<T> _tryParse;
    private protected readonly IReadOnlyDictionary<T, object> _valuesDictionary;

    [UnconditionalSuppressMessage("Trimming", "IL2070:'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.", Justification = "Literal fields on enums can never be trimmed")]
    internal EnumValueTypeReader(Type enumType, TryParseDelegate<T> tryParse) : this(enumType, enumType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), tryParse)
    {
    }

    internal EnumValueTypeReader(Type enumType, FieldInfo[] fields, TryParseDelegate<T> tryParse)
    {
        int length = fields.Length;
        Dictionary<T, object> valuesDictionary = new(length);

        for (var i = 0; i < length; i++)
        {
            var field = fields[i];
            var rawValue = field.GetRawConstantValue()!;
            var rawValueUnboxed = (T)rawValue;
            var value = Enum.ToObject(enumType, rawValue);
            valuesDictionary[rawValueUnboxed] = value;
        }

        _tryParse = tryParse;
        _valuesDictionary = valuesDictionary;
    }

    public bool TryRead(ReadOnlyMemory<char> input, [MaybeNullWhen(false)] out object value)
    {
        if (_tryParse(input.Span, out var parsed))
            return _valuesDictionary.TryGetValue(parsed, out value);

        value = null;
        return false;
    }
}
