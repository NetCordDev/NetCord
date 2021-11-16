using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonMessageActivity
{
    [JsonPropertyName("type")]
    public MessageActivityType Type { get; init; }

    [JsonPropertyName("party_id")]
    public string? PartyId { get; init; }
}
