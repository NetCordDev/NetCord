using NetCord.Rest;

namespace NetCord.Gateway;

public class UserCommandInteractionData : ApplicationCommandInteractionData
{
    public UserCommandInteractionData(JsonModels.JsonInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel)
    {
        if (jsonModel.ResolvedData!.GuildUsers != null)
        {
            var guildUser = jsonModel.ResolvedData.GuildUsers[jsonModel.TargetId.GetValueOrDefault()];
            guildUser.User = jsonModel.ResolvedData.Users![jsonModel.TargetId.GetValueOrDefault()];
            TargetUser = new GuildUser(guildUser, guildId.GetValueOrDefault(), client);
        }
        else
            TargetUser = new(jsonModel.ResolvedData.Users![jsonModel.TargetId.GetValueOrDefault()], client);
    }

    public User TargetUser { get; }
}
