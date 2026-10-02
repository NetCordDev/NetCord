using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest.JsonModels;

public class JsonGuildOnboardingPromptOption : JsonEntity
{
    [JsonPropertyName("channel_ids")]
    public required ulong[] ChannelIds { get; set; }

    [JsonPropertyName("role_ids")]
    public required ulong[] RoleIds { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
