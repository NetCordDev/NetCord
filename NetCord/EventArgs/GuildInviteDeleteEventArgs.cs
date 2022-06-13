using NetCord.JsonModels.EventArgs;

namespace NetCord;

public class GuildInviteDeleteEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs>
{
    JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildInviteDeleteEventArgs _jsonModel;

    public GuildInviteDeleteEventArgs(JsonGuildInviteDeleteEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public Snowflake InviteChannelId => _jsonModel.InviteChannelId;

    public Snowflake? GuildId => _jsonModel.GuildId;

    public string InviteCode => _jsonModel.InviteCode;
}
