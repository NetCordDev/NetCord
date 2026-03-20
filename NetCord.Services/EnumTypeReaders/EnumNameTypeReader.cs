using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetCord.Services.EnumTypeReaders;

internal class EnumNameTypeReader : IEnumTypeReader
{
    private readonly FrozenDictionary<ReadOnlyMemory<char>, object> _namesDictionary;

    [UnconditionalSuppressMessage("Trimming", "IL2070:'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.", Justification = "Literal fields on enums can never be trimmed")]
    internal EnumNameTypeReader(Type enumType, bool ignoreCase) : this(enumType, enumType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), ignoreCase)
    {
    }

    internal EnumNameTypeReader(Type enumType, FieldInfo[] fields, bool ignoreCase)
    {
        int length = fields.Length;
        var pairs = new KeyValuePair<ReadOnlyMemory<char>, object>[length];

        for (var i = 0; i < length; i++)
        {
            var field = fields[i];
            var rawValue = field.GetRawConstantValue()!;
            var value = Enum.ToObject(enumType, rawValue);
            pairs[i] = new(field.Name.AsMemory(), value);
        }

        _namesDictionary = pairs.ToFrozenDictionary(ignoreCase ? ReadOnlyMemoryCharComparer.InvariantCultureIgnoreCase
                                                               : ReadOnlyMemoryCharComparer.InvariantCulture);
    }

    public bool TryRead(ReadOnlyMemory<char> input, [MaybeNullWhen(false)] out object value)
    {
        return _namesDictionary.TryGetValue(input, out value);
    }
}
