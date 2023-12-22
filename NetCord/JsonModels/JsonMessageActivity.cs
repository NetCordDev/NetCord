using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageActivity
{
    [JsonPropertyName("type")]
    public MessageActivityType Type { get; set; }

    [JsonPropertyName("party_id")]
    public string? PartyId { get; set; }
}
