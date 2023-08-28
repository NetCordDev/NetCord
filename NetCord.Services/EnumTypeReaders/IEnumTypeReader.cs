using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.EnumTypeReaders;

internal interface IEnumTypeReader
{
    public bool TryRead(ReadOnlyMemory<char> input, [MaybeNullWhen(false)] out object value);
}
