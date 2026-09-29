using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GuildOnboardingPrompt(JsonGuildOnboardingPrompt jsonModel, ulong guildId, RestClient client) : Entity
{
    public override ulong Id { get; } = jsonModel.Id;

    /// <summary>
    /// Type of prompt.
    /// </summary>
    public GuildOnboardingPromptType Type { get; } = jsonModel.Type;

    /// <summary>
    /// Options available within the prompt.
    /// </summary>
    public IReadOnlyList<GuildOnboardingPromptOption> Options { get; } = [.. jsonModel.Options.Select(o => new GuildOnboardingPromptOption(o, guildId, client))];

    /// <summary>
    /// Title of the prompt.
    /// </summary>
    public string Title { get; } = jsonModel.Title;

    /// <summary>
    /// Indicates whether users are limited to selecting one option for the prompt.
    /// </summary>
    public bool SingleSelect { get; } = jsonModel.SingleSelect;

    /// <summary>
    /// Indicates whether the prompt is required before a user completes the onboarding flow.
    /// </summary>
    public bool Required { get; } = jsonModel.Required;

    /// <summary>
    /// Indicates whether the prompt is present in the onboarding flow. If false, the prompt will only appear in the Channels &#38; Roles tab.
    /// </summary>
    public bool InOnboarding { get; } = jsonModel.InOnboarding;
}
