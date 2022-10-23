namespace NetCord.Gateway;

public class MessageReactionRemoveAllEventArgs : IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs>
{
    JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs _jsonModel;

    public MessageReactionRemoveAllEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveAllEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public ulong ChannelId => _jsonModel.ChannelId;

    public ulong MessageId => _jsonModel.MessageId;

    public ulong? GuildId => _jsonModel.GuildId;
}
