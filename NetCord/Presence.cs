namespace NetCord;

public class Presence
{
    private readonly JsonModels.JsonPresence _jsonEntity;

    public User User { get; }
    public DiscordId? GuildId => _jsonEntity.GuildId;
    public UserStatusType Status => _jsonEntity.Status;
    public IEnumerable<UserActivity> Activities { get; }
    public IReadOnlyDictionary<Platform, UserStatusType> Platform => _jsonEntity.Platform;

    internal Presence(JsonModels.JsonPresence jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        User = new(jsonEntity.User, client);
        Activities = jsonEntity.Activities.SelectOrEmpty(a => new UserActivity(a, client));
    }
}