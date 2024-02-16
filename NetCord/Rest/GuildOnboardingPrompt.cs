namespace NetCord.Rest;

public class GuildOnboardingPrompt(JsonModels.JsonGuildOnboardingPrompt jsonModel, ulong guildId, RestClient client) : Entity, IJsonModel<JsonModels.JsonGuildOnboardingPrompt>
{
    JsonModels.JsonGuildOnboardingPrompt IJsonModel<JsonModels.JsonGuildOnboardingPrompt>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// Type of prompt.
    /// </summary>
    public GuildOnboardingPromptType Type => jsonModel.Type;

    /// <summary>
    /// Options available within the prompt.
    /// </summary>
    public IReadOnlyList<GuildOnboardingPromptOption> Options { get; } = jsonModel.Options.Select(o => new GuildOnboardingPromptOption(o, guildId, client)).ToArray();

    /// <summary>
    /// Title of the prompt.
    /// </summary>
    public string Title => jsonModel.Title;

    /// <summary>
    /// Indicates whether users are limited to selecting one option for the prompt.
    /// </summary>
    public bool SingleSelect => jsonModel.SingleSelect;

    /// <summary>
    /// Indicates whether the prompt is required before a user completes the onboarding flow.
    /// </summary>
    public bool Required => jsonModel.Required;

    /// <summary>
    /// Indicates whether the prompt is present in the onboarding flow. If false, the prompt will only appear in the Channels &#38; Roles tab.
    /// </summary>
    public bool InOnboarding => jsonModel.InOnboarding;
}
