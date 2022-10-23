namespace NetCord.Gateway;

public class MessageDeleteEventArgs : IJsonModel<JsonModels.EventArgs.JsonMessageDeleteEventArgs>
{
    JsonModels.EventArgs.JsonMessageDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageDeleteEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonMessageDeleteEventArgs _jsonModel;

    public MessageDeleteEventArgs(JsonModels.EventArgs.JsonMessageDeleteEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public ulong MessageId => _jsonModel.MessageId;

    public ulong ChannelId => _jsonModel.ChannelId;

    public ulong? GuildId => _jsonModel.GuildId;
}
