namespace NetCord.Gateway;

public class MessageReactionRemoveEmojiEventArgs : IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs>
{
    JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs _jsonModel;

    public MessageReactionRemoveEmojiEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
        Emoji = new(jsonModel.Emoji);
    }

    public Snowflake ChannelId => _jsonModel.ChannelId;

    public Snowflake? GuildId => _jsonModel.GuildId;

    public Snowflake MessageId => _jsonModel.MessageId;

    public MessageReactionEmoji Emoji { get; }
}
