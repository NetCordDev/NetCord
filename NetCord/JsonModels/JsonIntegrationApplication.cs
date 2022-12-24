using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonIntegrationApplication : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("icon")]
    public string? IconHash { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [JsonPropertyName("bot")]
    public JsonUser? Bot { get; set; }

    [JsonSerializable(typeof(JsonIntegrationApplication))]
    public partial class JsonIntegrationApplicationSerializerContext : JsonSerializerContext
    {
        public static JsonIntegrationApplicationSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
