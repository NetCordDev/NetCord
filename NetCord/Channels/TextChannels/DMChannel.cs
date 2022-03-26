using System.Collections.Immutable;

namespace NetCord;

public class DMChannel : TextChannel
{
    public ImmutableDictionary<DiscordId, User> Users { get; }

    internal DMChannel(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
    {
        Users = jsonEntity.Users.ToImmutableDictionaryOrEmpty(u => u.Id, u => new User(u, client));
    }
}