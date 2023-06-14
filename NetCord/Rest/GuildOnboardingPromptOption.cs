namespace NetCord.Rest;

public class GuildOnboardingPromptOption : Entity, IJsonModel<JsonModels.JsonGuildOnboardingPromptOption>
{
    JsonModels.JsonGuildOnboardingPromptOption IJsonModel<JsonModels.JsonGuildOnboardingPromptOption>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGuildOnboardingPromptOption _jsonModel;

    public GuildOnboardingPromptOption(JsonModels.JsonGuildOnboardingPromptOption jsonModel, ulong guildId, RestClient client)
    {
        _jsonModel = jsonModel;
        var emoji = jsonModel.Emoji;
        if (emoji.Name is not null)
            Emoji = Emoji.CreateFromJson(emoji, guildId, client);
    }

    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// Ids for channels an user is added to when the option is selected.
    /// </summary>
    public IReadOnlyList<ulong> ChannelIds => _jsonModel.ChannelIds;

    /// <summary>
    /// Ids for roles assigned to an user when the option is selected.
    /// </summary>
    public IReadOnlyList<ulong> RoleIds => _jsonModel.RoleIds;

    /// <summary>
    /// Emoji of the option.
    /// </summary>
    public Emoji? Emoji { get; }

    /// <summary>
    /// Title of the option.
    /// </summary>
    public string Title => _jsonModel.Title;

    /// <summary>
    /// Description of the option.
    /// </summary>
    public string? Description => _jsonModel.Description;
}
