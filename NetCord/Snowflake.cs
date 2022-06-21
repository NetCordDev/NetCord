using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(DiscordIdConverter))]
public readonly struct Snowflake : IConvertible, IEquatable<Snowflake>
{
    private readonly string? _value;

    public override string? ToString() => _value;

    public static bool TryCreate(string id, out Snowflake result)
    {
        if (ulong.TryParse(id, out _))
        {
            result = new(id);
            return true;
        }
        else
        {
            result = default;
            return false;
        }
    }

    public static Snowflake Create(DateTimeOffset createdAt)
    {
        return new((ulong)((createdAt.ToUnixTimeMilliseconds() - Discord.Epoch) << 22));
    }

    public static Snowflake Create(DateTimeOffset createdAt, byte internalWorkerId, byte internalProcessId, ushort increment)
    {
        var c = (ulong)((createdAt.ToUnixTimeMilliseconds() - Discord.Epoch) << 22);
        var w = (ulong)((internalWorkerId << 17) & 0x3E0000);
        var p = (ulong)((internalProcessId << 12) & 0x1F000);
        var i = (ulong)(increment & 0xFFF);
        return new(c | w | p | i);
    }

    private Snowflake(string id, object? obj)
    {
        _value = id;
    }

    public Snowflake(string s)
    {
        if (ulong.TryParse(s, out _))
            _value = s;
        else
            throw new FormatException($"'{nameof(s)}' must consist of decimal digits and cannot be too large");
    }

    public Snowflake(ulong u)
    {
        _value = u.ToString();
    }

    public static bool operator ==(Snowflake left, Snowflake right) => left.Equals(right);

    public static bool operator !=(Snowflake left, Snowflake right) => !(left == right);

    public override bool Equals(object? obj) => obj is Snowflake id && Equals(id);
    public bool Equals(Snowflake id) => _value == id._value;

    public override int GetHashCode() => HashCode.Combine(_value);

#pragma warning disable CS8604 // Possible null reference argument.
    public DateTimeOffset CreatedAt => DateTimeOffset.FromUnixTimeMilliseconds((long)((ulong.Parse(_value) >> 22) + Discord.Epoch));
    public byte InternalWorkerId => (byte)((ulong.Parse(_value) & 0x3E0000) >> 17);
    public byte InternalProcessId => (byte)((ulong.Parse(_value) & 0x1F000) >> 12);
    public ushort Increment => (ushort)(ulong.Parse(_value) & 0xFFF);
#pragma warning restore CS8604 // Possible null reference argument.

    TypeCode IConvertible.GetTypeCode() => TypeCode.String;
    bool IConvertible.ToBoolean(IFormatProvider? provider) => Convert.ToBoolean(_value, provider);
    byte IConvertible.ToByte(IFormatProvider? provider) => Convert.ToByte(_value, provider);
#pragma warning disable CS8604 // Possible null reference argument.
    char IConvertible.ToChar(IFormatProvider? provider) => Convert.ToChar(_value, provider);
#pragma warning restore CS8604 // Possible null reference argument.
    DateTime IConvertible.ToDateTime(IFormatProvider? provider) => CreatedAt.UtcDateTime;
    decimal IConvertible.ToDecimal(IFormatProvider? provider) => Convert.ToDecimal(_value, provider);
    double IConvertible.ToDouble(IFormatProvider? provider) => Convert.ToDouble(_value, provider);
    short IConvertible.ToInt16(IFormatProvider? provider) => Convert.ToInt16(_value, provider);
    int IConvertible.ToInt32(IFormatProvider? provider) => Convert.ToInt32(_value, provider);
    long IConvertible.ToInt64(IFormatProvider? provider) => Convert.ToInt64(_value, provider);
#pragma warning disable CS8604 // Possible null reference argument.
    sbyte IConvertible.ToSByte(IFormatProvider? provider) => Convert.ToSByte(_value, provider);
#pragma warning restore CS8604 // Possible null reference argument.
    float IConvertible.ToSingle(IFormatProvider? provider) => Convert.ToSingle(_value, provider);
#pragma warning disable CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
    string? IConvertible.ToString(IFormatProvider? provider) => _value;
    object? IConvertible.ToType(Type conversionType, IFormatProvider? provider) => Convert.ChangeType(_value, conversionType, provider);
#pragma warning restore CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
    ushort IConvertible.ToUInt16(IFormatProvider? provider) => Convert.ToUInt16(_value, provider);
    uint IConvertible.ToUInt32(IFormatProvider? provider) => Convert.ToUInt32(_value, provider);
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => Convert.ToUInt64(_value, provider);

    public static implicit operator Snowflake(ulong u) => new(u);
    public static explicit operator Snowflake(string s) => new(s);

    private class DiscordIdConverter : JsonConverter<Snowflake>
    {
        public override Snowflake Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => new(reader.TokenType is JsonTokenType.String ? reader.GetString()! : reader.GetUInt32().ToString(), null);

        public override void Write(Utf8JsonWriter writer, Snowflake value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value._value);
        }

        public override Snowflake ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => new(reader.GetString()!, null);

        public override void WriteAsPropertyName(Utf8JsonWriter writer, Snowflake value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value._value!);
        }
    }
}
