namespace NetCord.Rest;

public class GuildOnboardingPromptOption(JsonModels.JsonGuildOnboardingPromptOption jsonModel, ulong guildId, RestClient client) : Entity
{
    public override ulong Id { get; } = jsonModel.Id;

    /// <summary>
    /// Ids for channels an user is added to when the option is selected.
    /// </summary>
    public IReadOnlyList<ulong> ChannelIds { get; } = jsonModel.ChannelIds;

    /// <summary>
    /// Ids for roles assigned to an user when the option is selected.
    /// </summary>
    public IReadOnlyList<ulong> RoleIds { get; } = jsonModel.RoleIds;

    /// <summary>
    /// Emoji of the option.
    /// </summary>
    public Emoji? Emoji { get; } = jsonModel.Emoji is { } emoji ? Emoji.Create(emoji, guildId, client) : null;

    /// <summary>
    /// Title of the option.
    /// </summary>
    public string Title { get; } = jsonModel.Title;

    /// <summary>
    /// Description of the option.
    /// </summary>
    public string? Description { get; } = jsonModel.Description;
}
