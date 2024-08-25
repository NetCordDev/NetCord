namespace NetCord.Gateway;

public class InviteDeleteEventArgs(JsonModels.EventArgs.JsonInviteDeleteEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonInviteDeleteEventArgs>
{
    JsonModels.EventArgs.JsonInviteDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonInviteDeleteEventArgs>.JsonModel => jsonModel;

    public ulong InviteChannelId => jsonModel.InviteChannelId;

    public ulong? GuildId => jsonModel.GuildId;

    public string InviteCode => jsonModel.InviteCode;
}
