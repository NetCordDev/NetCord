using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class TextDisplayProperties(string content) : IComponentProperties
{
    /// <summary>
    /// Unique identifier for the component. Auto populated through increment if not provided.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.TextDisplay;

    [JsonPropertyName("content")]
    public string Content { get; set; } = content;

    public void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.TextDisplayProperties);
    }
}
