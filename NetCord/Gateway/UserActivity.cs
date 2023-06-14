using NetCord.Rest;

namespace NetCord.Gateway;

public class UserActivity : IJsonModel<JsonModels.JsonUserActivity>
{
    JsonModels.JsonUserActivity IJsonModel<JsonModels.JsonUserActivity>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonUserActivity _jsonModel;

    public string Name => _jsonModel.Name;
    public UserActivityType Type => _jsonModel.Type;
    public string? Url => _jsonModel.Url;
    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;
    public UserActivityTimestamps? Timestamps { get; }
    public ulong? ApplicationId => _jsonModel.ApplicationId;
    public string? Details => _jsonModel.Details;
    public string? State => _jsonModel.State;
    public Emoji? Emoji { get; }
    public Party? Party { get; }
    public UserActivityAssets? Assets { get; }
    public UserActivitySecrets? Secrets { get; }
    /// <summary>
    /// whether or not the activity is an instanced game session
    /// </summary>
    public bool? Instance => _jsonModel.Instance;
    public UserActivityFlags? Flags => _jsonModel.Flags;
    public IReadOnlyList<UserActivityButton> Buttons { get; }
    public ulong GuildId { get; }

    public UserActivity(JsonModels.JsonUserActivity jsonModel, ulong guildId, RestClient client)
    {
        _jsonModel = jsonModel;

        var timestamps = jsonModel.Timestamps;
        if (timestamps is not null)
            Timestamps = new(timestamps);

        var emoji = jsonModel.Emoji;
        if (emoji is not null)
            Emoji = Emoji.CreateFromJson(emoji, guildId, client);

        var party = jsonModel.Party;
        if (party is not null)
            Party = new(party);

        var assets = jsonModel.Assets;
        if (assets is not null)
            Assets = new(assets);

        var secrets = jsonModel.Secrets;
        if (secrets is not null)
            Secrets = new(secrets);

        Buttons = jsonModel.ButtonsLabels.SelectOrEmpty(b => new UserActivityButton(b)).ToArray();
        GuildId = guildId;
    }
}
