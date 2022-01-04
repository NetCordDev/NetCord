using System.Collections.Immutable;

namespace NetCord;

public class DMChannel : TextChannel
{
    private readonly ImmutableDictionary<DiscordId, User> _users;
    public IReadOnlyDictionary<DiscordId, User> Users
    {
        get
        {
            lock (_users)
                return new Dictionary<DiscordId, User>(_users);
        }
    }

    internal DMChannel(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
    {
        _users = jsonEntity.Users.ToImmutableDictionaryOrEmpty(u => u.Id, u => new User(u, client));
    }
}