namespace NetCord;

public class MessageDeleteBulkEventArgs : IJsonModel<JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs>
{
    JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs _jsonModel;

    public MessageDeleteBulkEventArgs(JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public IEnumerable<Snowflake> MessageIds => _jsonModel.MessageIds;

    public Snowflake ChannelId => _jsonModel.ChannelId;

    public Snowflake? GuildId => _jsonModel.GuildId;
}
