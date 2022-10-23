using NetCord.Rest;

namespace NetCord.Gateway;

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

    public ulong UserId => _jsonModel.UserId;

    public ulong ChannelId => _jsonModel.ChannelId;

    public ulong MessageId => _jsonModel.MessageId;

    public ulong? GuildId => _jsonModel.GuildId;

    public GuildUser? User { get; }

    public MessageReactionEmoji Emoji { get; }
}
