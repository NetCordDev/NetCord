namespace NetCord.Rest;

public class GuildOnboarding : IJsonModel<JsonModels.JsonGuildOnboarding>
{
    JsonModels.JsonGuildOnboarding IJsonModel<JsonModels.JsonGuildOnboarding>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildOnboarding _jsonModel;

    public GuildOnboarding(JsonModels.JsonGuildOnboarding jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        var guildId = jsonModel.GuildId;
        Prompts = jsonModel.Prompts.Select(p => new GuildOnboardingPrompt(p, guildId, client)).ToArray();
    }

    /// <summary>
    /// Id of the guild this onboarding is part of.
    /// </summary>
    public ulong GuildId => _jsonModel.GuildId;

    /// <summary>
    /// Prompts shown during onboarding and in customize community.
    /// </summary>
    public IReadOnlyList<GuildOnboardingPrompt> Prompts { get; }

    /// <summary>
    /// Channel Ids that users get opted into automatically.
    /// </summary>
    public IReadOnlyList<ulong> DefaultChannelIds => _jsonModel.DefaultChannelIds;

    /// <summary>
    /// Whether onboarding is enabled in the guild.
    /// </summary>
    public bool Enabled => _jsonModel.Enabled;

    /// <summary>
    /// Current mode of onboarding.
    /// </summary>
    public GuildOnboardingMode Mode => _jsonModel.Mode;
}
