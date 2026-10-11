using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GuildOnboarding(JsonGuildOnboarding jsonModel, RestClient client)
{
    /// <summary>
    /// ID of the guild this onboarding is part of.
    /// </summary>
    public ulong GuildId { get; } = jsonModel.GuildId;

    /// <summary>
    /// Prompts shown during onboarding and in customize community.
    /// </summary>
    public IReadOnlyList<GuildOnboardingPrompt> Prompts { get; } = [.. jsonModel.Prompts.Select(p => new GuildOnboardingPrompt(p, jsonModel.GuildId, client))];

    /// <summary>
    /// Channel Ids that users get opted into automatically.
    /// </summary>
    public IReadOnlyList<ulong> DefaultChannelIds { get; } = jsonModel.DefaultChannelIds;

    /// <summary>
    /// Whether onboarding is enabled in the guild.
    /// </summary>
    public bool Enabled { get; } = jsonModel.Enabled;

    /// <summary>
    /// Current mode of onboarding.
    /// </summary>
    public GuildOnboardingMode Mode { get; } = jsonModel.Mode;
}
