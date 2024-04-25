using System.Buffers;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(ColorConverter))]
public readonly struct Color : IEquatable<Color>
{
    public int RawValue => (Red << 16) | (Green << 8) | Blue;

    public byte Red { get; }

    public byte Green { get; }

    public byte Blue { get; }

    public Color(byte red, byte green, byte blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    public Color(int rgb)
    {
        Red = (byte)(rgb >> 16);
        Green = (byte)(rgb >> 8);
        Blue = (byte)rgb;
    }

    public bool Equals(Color other) => Red == other.Red && Green == other.Green && Blue == other.Blue;

    public override bool Equals(object? obj) => obj is Color color && Equals(color);

    public static bool operator ==(Color left, Color right) => left.Equals(right);

    public static bool operator !=(Color left, Color right) => !(left == right);

    public override int GetHashCode() => RawValue;

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
                var character = bytes[5 - i];

                int numericValue = character <= '9' ? character - '0' : character - ('a' - 10);

                result |= numericValue << (i * 4);
            }

            return new(result);
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.RawValue);
        }
    }
}
