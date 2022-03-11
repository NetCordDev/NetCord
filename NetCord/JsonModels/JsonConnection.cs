using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonConnection : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("type")]
    public ConnectionType Type { get; init; }

    [JsonPropertyName("revoked")]
    public bool? Revoked { get; init; }

    [JsonPropertyName("integrations")]
    public JsonIntegration[]? Integrations { get; init; }

    [JsonPropertyName("verified")]
    public bool Verified { get; init; }

    [JsonPropertyName("friend_sync")]
    public bool FriendSync { get; init; }

    [JsonPropertyName("show_activity")]
    public bool ShowActivity { get; init; }

    [JsonPropertyName("visibility")]
    public ConnectionVisibility Visibility { get; init; }
}