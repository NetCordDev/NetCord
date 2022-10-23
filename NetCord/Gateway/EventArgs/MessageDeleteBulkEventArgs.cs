namespace NetCord.Gateway;

public class MessageDeleteBulkEventArgs : IJsonModel<JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs>
{
    JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs _jsonModel;

    public MessageDeleteBulkEventArgs(JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public IReadOnlyList<ulong> MessageIds => _jsonModel.MessageIds;

    public ulong ChannelId => _jsonModel.ChannelId;

    public ulong? GuildId => _jsonModel.GuildId;
}
