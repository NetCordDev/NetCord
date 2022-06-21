using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class UserCommandInteractionData : ApplicationCommandInteractionData
{
    public UserCommandInteractionData(JsonInteractionData jsonModel, Snowflake? guildId, RestClient client) : base(jsonModel, guildId, client)
    {
        if (jsonModel.ResolvedData!.GuildUsers != null)
            TargetUser = new GuildUser(jsonModel.ResolvedData.GuildUsers[jsonModel.TargetId.GetValueOrDefault()] with
            {
                User = jsonModel.ResolvedData.Users![jsonModel.TargetId.GetValueOrDefault()]
            }, guildId.GetValueOrDefault(), client);
        else
            TargetUser = new(jsonModel.ResolvedData.Users![jsonModel.TargetId.GetValueOrDefault()], client);
    }

    public User TargetUser { get; }
}
