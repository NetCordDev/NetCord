using NetCord.Rest;

namespace NetCord.Gateway;

public class MessageReactionAddEventArgs : IJsonModel<JsonModels.EventArgs.JsonMessageReactionAddEventArgs>
{
    JsonModels.EventArgs.JsonMessageReactionAddEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageReactionAddEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonMessageReactionAddEventArgs _jsonModel;

    public MessageReactionAddEventArgs(JsonModels.EventArgs.JsonMessageReactionAddEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;

        var user = jsonModel.User;
        if (user is not null)
            User = new(user, jsonModel.GuildId.GetValueOrDefault(), client);

        Emoji = new(jsonModel.Emoji);
    }

    public ulong UserId => _jsonModel.UserId;

    public ulong ChannelId => _jsonModel.ChannelId;

    public ulong MessageId => _jsonModel.MessageId;

    public ulong? GuildId => _jsonModel.GuildId;

    public GuildUser? User { get; }

    public MessageReactionEmoji Emoji { get; }

    public ulong? MessageAuthorId => _jsonModel.MessageAuthorId;

    public bool Burst => _jsonModel.Burst;

    public IReadOnlyList<Color> BurstColors => _jsonModel.BurstColors;

    public ReactionType Type => _jsonModel.Type;
}
