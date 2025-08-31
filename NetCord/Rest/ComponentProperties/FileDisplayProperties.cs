using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

[GenerateMethodsForProperties]
public partial class FileDisplayProperties(ComponentMediaProperties file) : IMessageComponentProperties, IComponentContainerComponentProperties
{
    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.File;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("file")]
    public ComponentMediaProperties File { get; set; } = file;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("spoiler")]
    public bool Spoiler { get; set; }

    private void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.FileDisplayProperties);
    }

    void IJsonSerializable<IMessageComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteTo(writer);
    }

    void IJsonSerializable<IComponentContainerComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        WriteTo(writer);
    }
}
