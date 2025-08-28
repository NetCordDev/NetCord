using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ComponentSectionThumbnailProperties(ComponentMediaProperties media) : IComponentSectionAccessoryComponentProperties
{
    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.Thumbnail;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("media")]
    public ComponentMediaProperties Media { get; set; } = media;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("spoiler")]
    public bool Spoiler { get; set; }

    void IJsonSerializable<IComponentSectionAccessoryComponentProperties>.WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.ComponentSectionThumbnailProperties);
    }
}
