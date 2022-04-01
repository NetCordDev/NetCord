using NetCord.JsonModels;

namespace NetCord;

public class UserCommandInteractionData : ApplicationCommandInteractionData
{
    internal UserCommandInteractionData(JsonInteractionData jsonEntity, Snowflake? guildId, RestClient client) : base(jsonEntity, guildId, client)
    {
        if (jsonEntity.ResolvedData!.GuildUsers != null)
        {
            TargetUser = new GuildUser(jsonEntity.ResolvedData.GuildUsers[jsonEntity.TargetId.GetValueOrDefault()] with
            {
                User = jsonEntity.ResolvedData.Users![jsonEntity.TargetId.GetValueOrDefault()]
            }, guildId.GetValueOrDefault(), client);
        }
        else
            TargetUser = new(jsonEntity.ResolvedData.Users![jsonEntity.TargetId.GetValueOrDefault()], client);
    }

    public User TargetUser { get; }
}
