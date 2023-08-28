using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NetCord.Services.EnumTypeReaders;

internal class EnumNameOrValueTypeReader : IEnumTypeReader
{
    private readonly IEnumTypeReader _nameTypeReader;
    private readonly IEnumTypeReader _valueTypeReader;

    internal EnumNameOrValueTypeReader(Type enumType, bool ignoreCase, CultureInfo cultureInfo)
    {
        var fields = EnumHelper.GetFields(enumType);
        _nameTypeReader = new EnumNameTypeReader(enumType, fields, ignoreCase);
        _valueTypeReader = EnumValueTypeReader.Create(enumType, fields, cultureInfo);
    }

    public bool TryRead(ReadOnlyMemory<char> input, [MaybeNullWhen(false)] out object value)
    {
        return _nameTypeReader.TryRead(input, out value) || _valueTypeReader.TryRead(input, out value);
    }
}
