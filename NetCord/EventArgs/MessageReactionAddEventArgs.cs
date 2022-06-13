namespace NetCord;

public class MessageReactionAddEventArgs : IJsonModel<JsonModels.EventArgs.JsonMessageReactionAddEventArgs>
{
    JsonModels.EventArgs.JsonMessageReactionAddEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageReactionAddEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonMessageReactionAddEventArgs _jsonModel;

    public MessageReactionAddEventArgs(JsonModels.EventArgs.JsonMessageReactionAddEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.User != null)
            User = new(jsonModel.User, jsonModel.GuildId.GetValueOrDefault(), client);
        Emoji = new(jsonModel.Emoji);
    }

    public Snowflake UserId => _jsonModel.UserId;

    public Snowflake ChannelId => _jsonModel.ChannelId;

    public Snowflake MessageId => _jsonModel.MessageId;

    public Snowflake? GuildId => _jsonModel.GuildId;

    public GuildUser? User { get; }

    public MessageReactionEmoji Emoji { get; }
}
