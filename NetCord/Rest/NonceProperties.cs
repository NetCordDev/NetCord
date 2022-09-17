using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[JsonConverter(typeof(NoncePropertiesConverter))]
public class NonceProperties
{
    private readonly long? _l;
    private readonly string? _s;

    public NonceProperties(long l)
    {
        _l = l;
    }

    public NonceProperties(string s)
    {
        _s = s;
    }

    public static implicit operator NonceProperties(long l) => new(l);

    public static implicit operator NonceProperties(string s) => new(s);

    private class NoncePropertiesConverter : JsonConverter<NonceProperties>
    {
        public override NonceProperties? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, NonceProperties value, JsonSerializerOptions options)
        {
            if (value._l.HasValue)
                writer.WriteNumberValue(value._l.GetValueOrDefault());
            else
                writer.WriteStringValue(value._s!);
        }
    }
}
