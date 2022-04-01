using NetCord.JsonModels;

namespace NetCord;

public class GuildInteractionUser : GuildUser
{
    internal GuildInteractionUser(JsonGuildUser jsonEntity, Snowflake guildId, RestClient client) : base(jsonEntity, guildId, client)
    {
        Permissions = (Permission)ulong.Parse(jsonEntity.Permissions!);
    }

    public Permission Permissions { get; }
}
