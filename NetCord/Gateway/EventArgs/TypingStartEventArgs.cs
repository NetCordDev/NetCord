using NetCord.Rest;

namespace NetCord.Gateway;

public class TypingStartEventArgs : IJsonModel<JsonModels.EventArgs.JsonTypingStartEventArgs>
{
    JsonModels.EventArgs.JsonTypingStartEventArgs IJsonModel<JsonModels.EventArgs.JsonTypingStartEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonTypingStartEventArgs _jsonModel;

    public TypingStartEventArgs(JsonModels.EventArgs.JsonTypingStartEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;

        var user = jsonModel.User;
        if (user is not null)
            User = new(user, _jsonModel.GuildId.GetValueOrDefault(), client);
    }

    public ulong ChannelId => _jsonModel.ChannelId;

    public ulong? GuildId => _jsonModel.GuildId;

    public ulong UserId => _jsonModel.UserId;

    public DateTimeOffset Timestamp => _jsonModel.Timestamp;

    public GuildUser? User { get; }
}
