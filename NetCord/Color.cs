using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
        [DoesNotReturn]
        [StackTraceHidden]
        private static void ThrowFormatException()
        {
            throw new FormatException("Either the JSON value is not in a supported format, or is out of bounds for a Int32.");
        }

        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int value;
            if (!(reader.TokenType switch
            {
                JsonTokenType.String => TryReadHex(ref reader, out value),
                _ => reader.TryGetInt32(out value)
            }))
                ThrowFormatException();

            return new(value);
        }

        private static bool TryReadHex(ref Utf8JsonReader reader, out int value)
        {
            var span = reader.HasValueSequence
                ? reader.ValueSequence.ToArray()
                : reader.ValueSpan;

            if (span is [(byte)'#', ..])
                span = span[1..];

            return Utf8Parser.TryParse(span, out value, out int bytesConsumed, 'x') && span.Length == bytesConsumed;
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.RawValue);
        }
    }
}
