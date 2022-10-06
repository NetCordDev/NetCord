using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonGuildWidget : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("instant_invite")]
    public string? InstantInvite { get; set; }

    [JsonPropertyName("channels")]
    public JsonGuildWidgetChannel[] Channels { get; set; }

    [JsonPropertyName("members")]
    public JsonUser[] Users { get; set; }

    [JsonPropertyName("presence_count")]
    public int PresenceCount { get; set; }

    [JsonSerializable(typeof(JsonGuildWidget))]
    public partial class JsonGuildWidgetSerializerContext : JsonSerializerContext
    {
        public static JsonGuildWidgetSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
