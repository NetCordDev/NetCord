using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

public class JsonSerializableConverter<TValue> : JsonConverter<TValue> where TValue : IJsonSerializable
{
    public override TValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();

    public override void Write(Utf8JsonWriter writer, TValue value, JsonSerializerOptions options)
    {
        value.WriteTo(writer);
    }
}
