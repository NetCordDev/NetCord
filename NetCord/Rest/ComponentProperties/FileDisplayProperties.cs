using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class FileDisplayProperties(ComponentMediaProperties file) : IComponentProperties
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.File;

    [JsonPropertyName("file")]
    public ComponentMediaProperties File { get; set; } = file;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("spoiler")]
    public bool Spoiler { get; set; }

    public void WriteTo(Utf8JsonWriter writer)
    {
        JsonSerializer.Serialize(writer, this, Serialization.Default.FileDisplayProperties);
    }
}
