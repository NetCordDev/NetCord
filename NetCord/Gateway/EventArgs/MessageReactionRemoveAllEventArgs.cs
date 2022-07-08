namespace NetCord.Gateway;

public class MessageReactionRemoveAllEventArgs : IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs>
{
    JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs _jsonModel;

    public MessageReactionRemoveAllEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public Snowflake ChannelId => _jsonModel.ChannelId;

    public Snowflake MessageId => _jsonModel.MessageId;

    public Snowflake? GuildId => _jsonModel.GuildId;
}