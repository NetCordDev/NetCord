namespace NetCord.Gateway;

public class MessageReactionRemoveEventArgs : IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs>
{
    JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs _jsonModel;

    public MessageReactionRemoveEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
        Emoji = new(jsonModel.Emoji);
    }

    public ulong UserId => _jsonModel.UserId;

    public ulong ChannelId => _jsonModel.ChannelId;

    public ulong MessageId => _jsonModel.MessageId;

    public ulong? GuildId => _jsonModel.GuildId;

    public MessageReactionEmoji Emoji { get; }
}
