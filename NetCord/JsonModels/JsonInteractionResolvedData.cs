using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonInteractionResolvedData
{
    [JsonPropertyName("users")]
    public Dictionary<ulong, JsonUser>? Users { get; set; }

    [JsonPropertyName("members")]
    public Dictionary<ulong, JsonGuildUser>? GuildUsers { get; set; }

    [JsonPropertyName("roles")]
    public Dictionary<ulong, JsonRole>? Roles { get; set; }

    [JsonPropertyName("channels")]
    public Dictionary<ulong, JsonChannel>? Channels { get; set; }

    [JsonPropertyName("messages")]
    public Dictionary<ulong, JsonMessage>? Messages { get; set; }

    [JsonPropertyName("attachments")]
    public Dictionary<ulong, JsonAttachment>? Attachments { get; set; }
}
