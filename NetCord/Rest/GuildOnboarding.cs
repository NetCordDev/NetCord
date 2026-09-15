using NetCord.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents onboarding flow for a guild.
/// </summary>
public class GuildOnboarding(JsonGuildOnboarding jsonModel, RestClient client) : IJsonModel<JsonGuildOnboarding>
{
    JsonGuildOnboarding IJsonModel<JsonGuildOnboarding>.JsonModel => jsonModel;

    /// <summary>
    /// ID of the guild this onboarding is part of.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId;

    /// <summary>
    /// Prompts shown during onboarding and in customize community.
    /// </summary>
    public IReadOnlyList<GuildOnboardingPrompt> Prompts { get; } = jsonModel.Prompts.Select(p => new GuildOnboardingPrompt(p, jsonModel.GuildId, client)).ToArray();

    /// <summary>
    /// Channel IDs that users get opted into automatically.
    /// </summary>
    public IReadOnlyList<ulong> DefaultChannelIds => jsonModel.DefaultChannelIds;

    /// <summary>
    /// Whether onboarding is enabled in the guild.
    /// </summary>
    public bool Enabled => jsonModel.Enabled;

    /// <summary>
    /// Current mode of onboarding.
    /// </summary>
    public GuildOnboardingMode Mode => jsonModel.Mode;
}
