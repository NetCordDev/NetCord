using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonGuildWidget : JsonEntity
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
}
