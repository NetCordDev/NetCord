using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public class JsonGuildOnboarding
{
    [JsonPropertyName("guild_id")]
    public required ulong GuildId { get; set; }

    [JsonPropertyName("prompts")]
    public required JsonGuildOnboardingPrompt[] Prompts { get; set; }

    [JsonPropertyName("default_channel_ids")]
    public required ulong[] DefaultChannelIds { get; set; }

    [JsonPropertyName("enabled")]
    public required bool Enabled { get; set; }

    [JsonPropertyName("mode")]
    public required GuildOnboardingMode Mode { get; set; }
}
