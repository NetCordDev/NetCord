using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents an option in a guild onboarding prompt.
/// </summary>
public class GuildOnboardingPromptOption(JsonGuildOnboardingPromptOption jsonModel, ulong guildId, RestClient client) : Entity, IJsonModel<JsonGuildOnboardingPromptOption>
{
    JsonGuildOnboardingPromptOption IJsonModel<JsonGuildOnboardingPromptOption>.JsonModel => jsonModel;

    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// IDs for channels a user is added to when the option is selected.
    /// </summary>
    public IReadOnlyList<ulong> ChannelIds => jsonModel.ChannelIds;

    /// <summary>
    /// IDs for roles assigned to a user when the option is selected.
    /// </summary>
    public IReadOnlyList<ulong> RoleIds => jsonModel.RoleIds;

    /// <summary>
    /// Emoji of the option.
    /// </summary>
    public Emoji? Emoji { get; } = jsonModel.Emoji.Name is not null ? Emoji.CreateFromJson(jsonModel.Emoji, guildId, client) : null;

    /// <summary>
    /// Title of the option.
    /// </summary>
    public string Title => jsonModel.Title;

    /// <summary>
    /// Description of the option.
    /// </summary>
    public string? Description => jsonModel.Description;
}
