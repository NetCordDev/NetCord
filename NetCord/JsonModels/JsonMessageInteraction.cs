using System.Text.Json.Serialization;

using JsonGuard;

namespace NetCord.JsonModels;

[JsonGuard]
public partial class JsonMessageInteraction : JsonEntity
{
    [JsonPropertyName("type")]
    public InteractionType Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("user")]
    public JsonUser User { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; set; }
}
