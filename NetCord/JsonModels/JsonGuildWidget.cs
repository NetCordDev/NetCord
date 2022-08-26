using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonGuildWidget : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("instant_invite")]
    public string? InstantInvite { get; init; }

    [JsonPropertyName("channels")]
    public JsonGuildWidgetChannel[] Channels { get; init; }

    [JsonPropertyName("members")]
    public JsonUser[] Users { get; init; }

    [JsonPropertyName("presence_count")]
    public int PresenceCount { get; init; }
}
