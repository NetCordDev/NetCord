using NetCord.Rest;

namespace NetCord;

public class DMChannel : TextChannel
{
    public IReadOnlyDictionary<Snowflake, User> Users { get; }

    public DMChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        Users = jsonModel.Users.ToDictionaryOrEmpty(u => u.Id, u => new User(u, client));
    }
}
