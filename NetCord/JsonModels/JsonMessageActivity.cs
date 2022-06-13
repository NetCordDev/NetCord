using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public record JsonMessageActivity
{
    [JsonPropertyName("type")]
    public MessageActivityType Type { get; init; }

    [JsonPropertyName("party_id")]
    public string? PartyId { get; init; }
}
