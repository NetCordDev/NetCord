using NetCord.Rest;

namespace NetCord;

public class UserCommandInteractionData : ApplicationCommandInteractionData
{
    public UserCommandInteractionData(JsonModels.JsonInteractionData jsonModel, ulong? guildId, RestClient client) : base(jsonModel)
    {
        var resolvedData = jsonModel.ResolvedData!;
        var guildUsers = resolvedData.GuildUsers;
        var targetId = jsonModel.TargetId.GetValueOrDefault();
        if (guildUsers is null)
            TargetUser = new(resolvedData.Users![targetId], client);
        else
        {
            var guildUser = guildUsers[targetId];
            guildUser.User = resolvedData.Users![targetId];
            TargetUser = new GuildUser(guildUser, guildId.GetValueOrDefault(), client);
        }
    }

    public User TargetUser { get; }
}
