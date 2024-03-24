using System.Text.Json.Serialization;

namespace NetCord.Gateway.JsonModels;

public partial class JsonGuildJoinRequestFormResponse
{
    [JsonPropertyName("field_type")]
    public GuildJoinRequestFormResponseFieldType FieldType { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; }

    [JsonPropertyName("response")]
    public bool Response { get; set; }

    [JsonPropertyName("values")]
    public IReadOnlyList<string> Values { get; set; }
}
