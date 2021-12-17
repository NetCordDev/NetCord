namespace NetCord;

public class DMChannel : TextChannel
{
    private readonly Dictionary<DiscordId, User> _users;
    public IReadOnlyDictionary<DiscordId, User> Users
    {
        get
        {
            lock (_users)
                return new Dictionary<DiscordId, User>(_users);
        }
    }

    internal DMChannel(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
    {
        _users = jsonEntity.Users.ToDictionaryOrEmpty(u => u.Id, u => new User(u, client));
    }
}