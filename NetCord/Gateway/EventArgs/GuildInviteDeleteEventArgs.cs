namespace NetCord.Gateway;

public class GuildInviteDeleteEventArgs(JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs>
{
    JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs>.JsonModel => jsonModel;

    public ulong InviteChannelId => jsonModel.InviteChannelId;

    public ulong? GuildId => jsonModel.GuildId;

    public string InviteCode => jsonModel.InviteCode;
}
