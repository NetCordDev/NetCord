using System.Text.Json.Serialization;

namespace NetCord.Rest.JsonModels;

public partial class JsonGuildOnboarding
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("prompts")]
    public JsonGuildOnboardingPrompt[] Prompts { get; set; }

    [JsonPropertyName("default_channel_ids")]
    public ulong[] DefaultChannelIds { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonSerializable(typeof(JsonGuildOnboarding))]
    public partial class JsonGuildOnboardingSerializerContext : JsonSerializerContext
    {
        public static JsonGuildOnboardingSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
