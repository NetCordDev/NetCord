using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.JsonConverters;

internal class PermissionConverter : JsonConverter<Permission>
{
    public override Permission Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => (Permission)ulong.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, Permission value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(((ulong)value).ToString());
    }
}
