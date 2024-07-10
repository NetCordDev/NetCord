using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="type">Type of the prompt.</param>
/// <param name="options">Options available within the prompt.</param>
/// <param name="title">Title of the prompt.</param>
public partial class GuildOnboardingPromptProperties(GuildOnboardingPromptType type, IEnumerable<GuildOnboardingPromptOptionProperties> options, string title)
{
    /// <summary>
    /// ID of the prompt.
    /// </summary>
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    /// <summary>
    /// Type of the prompt.
    /// </summary>
    [JsonPropertyName("type")]
    public GuildOnboardingPromptType Type { get; set; } = type;

    /// <summary>
    /// Options available within the prompt.
    /// </summary>
    [JsonPropertyName("options")]
    public IEnumerable<GuildOnboardingPromptOptionProperties> Options { get; set; } = options;

    /// <summary>
    /// Title of the prompt.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = title;

    /// <summary>
    /// Indicates whether users are limited to selecting one option for the prompt.
    /// </summary>
    [JsonPropertyName("single_select")]
    public bool SingleSelect { get; set; }

    /// <summary>
    /// Indicates whether the prompt is required before a user completes the onboarding flow.
    /// </summary>
    [JsonPropertyName("required")]
    public bool Required { get; set; }

    /// <summary>
    /// Indicates whether the prompt is present in the onboarding flow. If false, the prompt will only appear in the Channels &#38; Roles tab.
    /// </summary>
    [JsonPropertyName("in_onboarding")]
    public bool InOnboarding { get; set; }
}
