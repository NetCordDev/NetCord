using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildOnboardingOptions
{
    internal GuildOnboardingOptions()
    {
    }

    /// <summary>
    /// Prompts shown during onboarding and in customize community.
    /// </summary>
    [JsonPropertyName("prompts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<GuildOnboardingPromptProperties>? Prompts { get; set; }

    /// <summary>
    /// Channel ids that members get opted into automatically.
    /// </summary>
    [JsonPropertyName("default_channel_ids")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<ulong>? DefaultChannelIds { get; set; }

    /// <summary>
    /// Whether onboarding is enabled in the guild.
    /// </summary>
    [JsonPropertyName("enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Enabled { get; set; }

    /// <summary>
    /// Current mode of onboarding.
    /// </summary>
    [JsonPropertyName("mode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GuildOnboardingMode? Mode { get; set; }
}
