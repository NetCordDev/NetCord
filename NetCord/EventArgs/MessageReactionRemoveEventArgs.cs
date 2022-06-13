namespace NetCord;

public class MessageReactionRemoveEventArgs : IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs>
{
    JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs _jsonModel;

    public MessageReactionRemoveEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
        Emoji = new(jsonModel.Emoji);
    }

    public Snowflake UserId => _jsonModel.UserId;

    public Snowflake ChannelId => _jsonModel.ChannelId;

    public Snowflake MessageId => _jsonModel.MessageId;

    public Snowflake? GuildId => _jsonModel.GuildId;

    public MessageReactionEmoji Emoji { get; }
}
