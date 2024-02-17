using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

public class StringEnumConverterWithErrorHandling<T> : JsonConverter<T> where T : struct, Enum
{
    private static readonly JsonEncodedText _unknownName = JsonEncodedText.Encode(default(ReadOnlySpan<byte>));
    private static readonly T _unknownValue = (T)(object)-1;

    private readonly Dictionary<ReadOnlyMemory<byte>, T> _namesDictionary;
    private readonly Dictionary<T, JsonEncodedText> _valuesDictionary;

    [UnconditionalSuppressMessage("Trimming", "IL2090:'this' argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The generic parameter of the source method or type does not have matching annotations.", Justification = "<Pending>")]
    public StringEnumConverterWithErrorHandling()
    {
        var enumType = typeof(T);

        var fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        int length = fields.Length;

        Dictionary<ReadOnlyMemory<byte>, T> namesDictionary = new(length, OrdinalReadOnlyMemoryByteComparer.Instance);
        Dictionary<T, JsonEncodedText> valuesDictionary = new(length);

        for (var i = 0; i < length; i++)
        {
            var field = fields[i];

            var jsonPropertyNameAttribute = field.GetCustomAttribute<JsonPropertyNameAttribute>();
            var name = jsonPropertyNameAttribute is null ? field.Name : jsonPropertyNameAttribute.Name;
            var nameBytes = Encoding.UTF8.GetBytes(name);

            var rawValue = field.GetRawConstantValue()!;
            var value = (T)rawValue;

            namesDictionary.Add(nameBytes, value);
            valuesDictionary.Add(value, JsonEncodedText.Encode(nameBytes));
        }

        _namesDictionary = namesDictionary;
        _valuesDictionary = valuesDictionary;
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return _namesDictionary.TryGetValue(reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray(), out var value)
            ? value
            : _unknownValue;
    }

    public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return _namesDictionary.TryGetValue(reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray(), out var value)
            ? value
            : _unknownValue;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(_valuesDictionary.TryGetValue(value, out var name) ? name : _unknownName);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(_valuesDictionary.TryGetValue(value, out var name) ? name : _unknownName);
    }

    private class OrdinalReadOnlyMemoryByteComparer : IComparer<ReadOnlyMemory<byte>>, IEqualityComparer<ReadOnlyMemory<byte>>
    {
        public static OrdinalReadOnlyMemoryByteComparer Instance { get; } = new();

        public int Compare(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y)
        {
            return x.Span.SequenceCompareTo(y.Span);
        }

        public bool Equals(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y)
        {
            return x.Span.SequenceEqual(y.Span);
        }

        public int GetHashCode([DisallowNull] ReadOnlyMemory<byte> obj)
        {
            HashCode hashCode = new();
            hashCode.AddBytes(obj.Span);
            return hashCode.ToHashCode();
        }
    }
}
