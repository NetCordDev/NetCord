using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonSku : JsonEntity
{
    [JsonPropertyName("type")]
    public required SkuType Type { get; set; }

    [JsonPropertyName("application_id")]
    public required ulong ApplicationId { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("slug")]
    public required string Slug { get; set; }

    [JsonPropertyName("flags")]
    public required SkuFlags Flags { get; set; }
}
