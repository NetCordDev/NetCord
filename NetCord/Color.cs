using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(ColorConverter))]
public readonly struct Color : IEquatable<Color>
{
    public int RawValue { get; }

    public byte Red => (byte)(RawValue >> 16);

    public byte Green => (byte)(RawValue >> 8);

    public byte Blue => (byte)RawValue;

    public Color(byte r, byte g, byte b)
    {
        RawValue = (r << 16) | (g << 8) | b;
    }

    public Color(int rgb)
    {
        RawValue = rgb;
    }

    public bool Equals(Color other) => RawValue == other.RawValue;

    public override bool Equals(object? obj) => obj is Color color && Equals(color);

    public static bool operator ==(Color left, Color right) => left.Equals(right);

    public static bool operator !=(Color left, Color right) => !(left == right);

    public override int GetHashCode() => RawValue.GetHashCode();

    public override string ToString() => $"#{RawValue:X6}";

    public class ColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
                return ReadFromHex(reader.HasValueSequence ? reader.ValueSequence.Slice(1).ToArray() : reader.ValueSpan[1..]);

            var span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
            if (Utf8Parser.TryParse(span, out int value, out int bytesConsumed) && span.Length == bytesConsumed)
                return new(value);

            throw new FormatException("Either the JSON value is not in a supported format, or is out of bounds for a Int32.");
        }

        private static Color ReadFromHex(ReadOnlySpan<byte> bytes)
        {
            int result = 0;

            for (int i = 5; i >= 0; i--)
            {
                int shift = i * 4;
                int byteValue = GetByteValue(bytes[5 - i]);
                result |= byteValue << shift;
            }

            return new(result);

            static int GetByteValue(byte b) => b <= '9' ? b - '0' : b - 'a' + 10;
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.RawValue);
        }
    }
}
