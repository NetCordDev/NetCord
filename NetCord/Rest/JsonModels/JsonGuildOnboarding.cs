using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonGuildOnboarding
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("prompts")]
    public JsonGuildOnboardingPrompt[] Prompts { get; set; }

    [JsonPropertyName("default_channel_ids")]
    public ulong[] DefaultChannelIds { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("mode")]
    public GuildOnboardingMode Mode { get; set; }
}
