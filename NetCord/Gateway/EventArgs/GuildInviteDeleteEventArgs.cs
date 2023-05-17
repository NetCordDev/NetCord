namespace NetCord.Gateway;

public class GuildInviteDeleteEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs>
{
    JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs _jsonModel;

    public GuildInviteDeleteEventArgs(JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public ulong InviteChannelId => _jsonModel.InviteChannelId;

    public ulong? GuildId => _jsonModel.GuildId;

    public string InviteCode => _jsonModel.InviteCode;
}
