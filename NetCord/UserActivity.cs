namespace NetCord;

public class UserActivity : IJsonModel<JsonModels.JsonUserActivity>
{
    JsonModels.JsonUserActivity IJsonModel<JsonModels.JsonUserActivity>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonUserActivity _jsonModel;

    public string Name => _jsonModel.Name;
    public UserActivityType Type => _jsonModel.Type;
    public string? Url => _jsonModel.Url;
    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;
    public UserActivityTimestamps? Timestamps { get; }
    public Snowflake? ApplicationId => _jsonModel.ApplicationId;
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
    public IEnumerable<UserActivityButton> Buttons { get; }
    public Snowflake GuildId { get; }

    public UserActivity(JsonModels.JsonUserActivity jsonModel, Snowflake guildId, RestClient client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.Timestamps != null)
            Timestamps = new(jsonModel.Timestamps);
        if (jsonModel.Emoji != null)
            Emoji = Emoji.CreateFromJson(jsonModel.Emoji, guildId, client);
        if (jsonModel.Party != null)
            Party = new(jsonModel.Party);
        if (jsonModel.Assets != null)
            Assets = new(jsonModel.Assets);
        if (jsonModel.Secrets != null)
            Secrets = new(jsonModel.Secrets);
        Buttons = jsonModel.ButtonsLabels.SelectOrEmpty(b => new UserActivityButton(b));
        GuildId = guildId;
    }
}