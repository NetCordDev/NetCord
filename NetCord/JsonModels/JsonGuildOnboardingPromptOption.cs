using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonGuildOnboardingPromptOption : JsonEntity
{
    [JsonPropertyName("channel_ids")]
    public ulong[] ChannelIds { get; set; }

    [JsonPropertyName("role_ids")]
    public ulong[] RoleIds { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
