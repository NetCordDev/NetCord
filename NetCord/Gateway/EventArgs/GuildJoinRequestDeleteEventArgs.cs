namespace NetCord.Gateway;

public class GuildJoinRequestDeleteEventArgs(JsonModels.EventArgs.JsonGuildJoinRequestDeleteEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonGuildJoinRequestDeleteEventArgs>
{
    JsonModels.EventArgs.JsonGuildJoinRequestDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildJoinRequestDeleteEventArgs>.JsonModel => jsonModel;

    public ulong JoinRequestId => jsonModel.JoinRequestId;

    public ulong UserId => jsonModel.UserId;

    public ulong GuildId => jsonModel.GuildId;
}
