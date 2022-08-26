using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

/// <summary>
/// You must use <see cref="JsonIgnoreAttribute"/>(Condition = <see cref="JsonIgnoreCondition.WhenWritingDefault"/>)
/// </summary>
internal class NullConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => true;
    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteNullValue();
    }
}
