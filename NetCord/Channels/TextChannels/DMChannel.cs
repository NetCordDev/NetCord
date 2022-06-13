using System.Collections.Immutable;

namespace NetCord;

public class DMChannel : TextChannel
{
    public ImmutableDictionary<Snowflake, User> Users { get; }

    public DMChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        Users = jsonModel.Users.ToImmutableDictionaryOrEmpty(u => u.Id, u => new User(u, client));
    }
}