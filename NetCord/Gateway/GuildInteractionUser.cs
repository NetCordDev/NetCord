using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class GuildInteractionUser : GuildUser
{
    public GuildInteractionUser(JsonGuildUser jsonModel, Snowflake guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        Permissions = (Permission)ulong.Parse(jsonModel.Permissions!);
    }

    public Permission Permissions { get; }
}