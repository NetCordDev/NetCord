using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(ColorConverter))]
public readonly struct Color : IEquatable<Color>
{
    private readonly int _value;

    public byte Red => (byte)(_value >> 16);

    public byte Green => (byte)(_value >> 8);

    public byte Blue => (byte)_value;

    public Color()
    {
        _value = 0;
    }

    public Color(byte r, byte g, byte b)
    {
        _value = (r << 16) | (g << 8) | b;
    }

    public Color(int rgb)
    {
        if (rgb is >= 0 and <= 16777215)
            _value = rgb;
        else
            throw new ArgumentOutOfRangeException(nameof(rgb), $"{rgb} is not >= 0 or not <= 16777215");
    }

    public bool Equals(Color other) => _value == other._value;

    public override bool Equals(object? obj) => obj is Color color && Equals(color);

    public static bool operator ==(Color left, Color right) => left.Equals(right);

    public static bool operator !=(Color left, Color right) => !(left == right);

    public override int GetHashCode() => _value.GetHashCode();

    private class ColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => new(reader.GetInt32());

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value._value);
        }
    }
}