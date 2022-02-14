namespace NetCord;

public class UserActivity
{
    private readonly JsonModels.JsonUserActivity _jsonEntity;

    public string Name => _jsonEntity.Name;
    public UserActivityType Type => _jsonEntity.Type;
    public string? Url => _jsonEntity.Url;
    public DateTimeOffset CreatedAt => _jsonEntity.CreatedAt;
    public UserActivityTimestamps? Timestamps { get; }
    public DiscordId? ApplicationId => _jsonEntity.ApplicationId;
    public string? Details => _jsonEntity.Details;
    public string? State => _jsonEntity.State;
    public Emoji? Emoji { get; }
    public Party? Party { get; }
    public UserActivityAssets? Assets { get; }
    public UserActivitySecrets? Secrets { get; }
    /// <summary>
    /// whether or not the activity is an instanced game session
    /// </summary>
    public bool? Instance => _jsonEntity.Instance;
    public int? Flags => _jsonEntity.Flags;
    public IEnumerable<UserActivityButton> Buttons { get; }

    internal UserActivity(JsonModels.JsonUserActivity jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.Timestamps != null)
            Timestamps = new(jsonEntity.Timestamps);
        if (jsonEntity.Emoji != null)
            Emoji = new(jsonEntity.Emoji, client);
        if (jsonEntity.Party != null)
            Party = new(jsonEntity.Party);
        if (jsonEntity.Assets != null)
            Assets = new(jsonEntity.Assets);
        if (jsonEntity.Secrets != null)
            Secrets = new(jsonEntity.Secrets);
        Buttons = jsonEntity.ButtonsLabels.SelectOrEmpty(b => new UserActivityButton(b));
    }
}