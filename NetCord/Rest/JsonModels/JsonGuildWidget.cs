using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonGuildWidget : JsonEntity
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("instant_invite")]
    public string? InstantInvite { get; set; }

    [JsonPropertyName("channels")]
    public required JsonGuildWidgetChannel[] Channels { get; set; }

    [JsonPropertyName("members")]
    public required JsonUser[] Users { get; set; }

    [JsonPropertyName("presence_count")]
    public required int PresenceCount { get; set; }
}
