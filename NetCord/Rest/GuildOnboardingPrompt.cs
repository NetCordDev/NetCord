namespace NetCord.Rest;

public class GuildOnboardingPrompt : Entity, IJsonModel<JsonModels.JsonGuildOnboardingPrompt>
{
    JsonModels.JsonGuildOnboardingPrompt IJsonModel<JsonModels.JsonGuildOnboardingPrompt>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildOnboardingPrompt _jsonModel;

    public GuildOnboardingPrompt(JsonModels.JsonGuildOnboardingPrompt jsonModel, ulong guildId, RestClient client)
    {
        _jsonModel = jsonModel;
        Options = jsonModel.Options.Select(o => new GuildOnboardingPromptOption(o, guildId, client)).ToArray();
    }

    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// Type of prompt.
    /// </summary>
    public GuildOnboardingPromptType Type => _jsonModel.Type;

    /// <summary>
    /// Options available within the prompt.
    /// </summary>
    public IReadOnlyList<GuildOnboardingPromptOption> Options { get; }

    /// <summary>
    /// Title of the prompt.
    /// </summary>
    public string Title => _jsonModel.Title;

    /// <summary>
    /// Indicates whether users are limited to selecting one option for the prompt.
    /// </summary>
    public bool SingleSelect => _jsonModel.SingleSelect;

    /// <summary>
    /// Indicates whether the prompt is required before a user completes the onboarding flow.
    /// </summary>
    public bool Required => _jsonModel.Required;

    /// <summary>
    /// Indicates whether the prompt is present in the onboarding flow. If false, the prompt will only appear in the Channels &#38; Roles tab.
    /// </summary>
    public bool InOnboarding => _jsonModel.InOnboarding;
}
