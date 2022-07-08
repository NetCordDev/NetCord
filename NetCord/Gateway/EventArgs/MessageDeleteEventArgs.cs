namespace NetCord.Gateway;

public class MessageDeleteEventArgs : IJsonModel<JsonModels.EventArgs.JsonMessageDeleteEventArgs>
{
    JsonModels.EventArgs.JsonMessageDeleteEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageDeleteEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonMessageDeleteEventArgs _jsonModel;

    public MessageDeleteEventArgs(JsonModels.EventArgs.JsonMessageDeleteEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public Snowflake MessageId => _jsonModel.MessageId;

    public Snowflake ChannelId => _jsonModel.ChannelId;

    public Snowflake? GuildId => _jsonModel.GuildId;
}