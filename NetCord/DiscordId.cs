using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(DiscordIdConverter))]
public class DiscordId : IConvertible
{
    private readonly string _value;

    public override string ToString() => _value;

    public static DiscordId Parse(string id)
    {
        if (TryParse(id, out var result))
            return result;
        else
            throw new FormatException($"{nameof(id)} must consist of decimal digits and cannot be too large");
    }

    public static bool TryParse(string id, [NotNullWhen(true)] out DiscordId? result)
    {
        if (ulong.TryParse(id, out _))
        {
            result = new(id);
            return true;
        }
        else
        {
            result = null;
            return false;
        }
    }

    private DiscordId(string id)
    {
        _value = id;
    }

    public static bool operator ==(DiscordId left, DiscordId right) => left?._value == right?._value;

    public static bool operator !=(DiscordId left, DiscordId right) => !(left == right);

    public override bool Equals(object obj) => obj is DiscordId id && _value == id._value;

    public override int GetHashCode() => _value.GetHashCode();

    public DateTimeOffset CreatedAt => DateTimeOffset.FromUnixTimeMilliseconds((long)((ulong.Parse(_value) >> 22) + Discord.Epoch));
    public byte InternalWorkerId => (byte)((ulong.Parse(_value) & 0x3E0000) >> 17);
    public byte InternalProcessId => (byte)((ulong.Parse(_value) & 0x1F000) >> 12);
    public ushort Increment => (ushort)(ulong.Parse(_value) & 0xFFF);


    TypeCode IConvertible.GetTypeCode() => _value.GetTypeCode();
    bool IConvertible.ToBoolean(IFormatProvider provider) => Convert.ToBoolean(_value, provider);
    byte IConvertible.ToByte(IFormatProvider provider) => Convert.ToByte(_value, provider);
    char IConvertible.ToChar(IFormatProvider provider) => Convert.ToChar(_value, provider);
    DateTime IConvertible.ToDateTime(IFormatProvider provider) => CreatedAt.UtcDateTime;
    decimal IConvertible.ToDecimal(IFormatProvider provider) => Convert.ToDecimal(_value, provider);
    double IConvertible.ToDouble(IFormatProvider provider) => Convert.ToDouble(_value, provider);
    short IConvertible.ToInt16(IFormatProvider provider) => Convert.ToInt16(_value, provider);
    int IConvertible.ToInt32(IFormatProvider provider) => Convert.ToInt32(_value, provider);
    long IConvertible.ToInt64(IFormatProvider provider) => Convert.ToInt64(_value, provider);
    sbyte IConvertible.ToSByte(IFormatProvider provider) => Convert.ToSByte(_value, provider);
    float IConvertible.ToSingle(IFormatProvider provider) => Convert.ToSingle(_value, provider);
    string IConvertible.ToString(IFormatProvider provider) => _value;
    object IConvertible.ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)_value).ToType(conversionType, provider);
    ushort IConvertible.ToUInt16(IFormatProvider provider) => Convert.ToUInt16(_value, provider);
    uint IConvertible.ToUInt32(IFormatProvider provider) => Convert.ToUInt32(_value, provider);
    ulong IConvertible.ToUInt64(IFormatProvider provider) => Convert.ToUInt64(_value, provider);

    private class DiscordIdConverter : JsonConverter<DiscordId>
    {
        public override DiscordId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => new(reader.GetString());

        public override void Write(Utf8JsonWriter writer, DiscordId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
