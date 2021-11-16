using System.Text.Json.Serialization;

namespace NetCord;

[JsonConverter(typeof(JsonConverters.ColorConverter))]
public readonly struct Color : IEquatable<Color>
{
    internal readonly int _value;

    public byte R => (byte)(_value >> 16);

    public byte G => (byte)(_value >> 8);

    public byte B => (byte)_value;

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
        _value = rgb;
    }

    public bool Equals(Color other) => _value == other._value;

    public override bool Equals(object obj) => obj is Color color && Equals(color);

    public static bool operator ==(Color left, Color right) => left.Equals(right);

    public static bool operator !=(Color left, Color right) => !(left == right);

    public override int GetHashCode() => HashCode.Combine(_value, R, G, B);
}