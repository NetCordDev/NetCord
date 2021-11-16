using System.Diagnostics.CodeAnalysis;

namespace NetCord;

public class DMChannel : TextChannel
{
    internal Dictionary<DiscordId, User> _users;
    public IEnumerable<User> Users
    {
        get
        {
            lock (_users)
                return _users.Values.AsEnumerable();
        }
    }

    internal DMChannel(JsonModels.JsonChannel jsonEntity, BotClient client) : base(jsonEntity, client)
    {
        _users = jsonEntity.Users.ToDictionaryOrEmpty(u => u.Id, u => new User(u, client));
    }

    public bool TryGetUser(DiscordId id, [NotNullWhen(true)] out User? user)
    {
        lock (_users)
        {
            if (_users.TryGetValue(id, out user))
                return true;
            return false;
        }
    }

    public User GetUser(DiscordId id)
    {
        if (TryGetUser(id, out var user))
            return user;
        else
            throw new EntityNotFoundException("The user was not found");
    }
}
