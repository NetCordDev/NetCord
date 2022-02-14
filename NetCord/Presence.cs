namespace NetCord;

public class Presence
{
    private readonly JsonModels.JsonPresence _jsonEntity;

    public DiscordId UserId => _jsonEntity.User.Id;
    public DiscordId? GuildId => _jsonEntity.GuildId;
    public UserStatus Status => _jsonEntity.Status;
    public IEnumerable<UserActivity> Activities { get; }
    public IReadOnlyDictionary<Platform, UserStatus> Platform => _jsonEntity.Platform;

    internal Presence(JsonModels.JsonPresence jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        Activities = jsonEntity.Activities.SelectOrEmpty(a => new UserActivity(a, client));
    }
}