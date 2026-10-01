using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageInteraction : JsonEntity
{
    [JsonPropertyName("type")]
    public required InteractionType Type { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("user")]
    public required JsonUser User { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; set; }
}
