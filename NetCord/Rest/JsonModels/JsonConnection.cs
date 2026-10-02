using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonConnection
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public required ConnectionType Type { get; set; }

    [JsonPropertyName("revoked")]
    public bool? Revoked { get; set; }

    [JsonPropertyName("integrations")]
    public JsonIntegration[]? Integrations { get; set; }

    [JsonPropertyName("verified")]
    public required bool Verified { get; set; }

    [JsonPropertyName("friend_sync")]
    public required bool FriendSync { get; set; }

    [JsonPropertyName("show_activity")]
    public required bool ShowActivity { get; set; }

    [JsonPropertyName("two_way_link")]
    public required bool TwoWayLink { get; set; }

    [JsonPropertyName("visibility")]
    public required ConnectionVisibility Visibility { get; set; }
}
