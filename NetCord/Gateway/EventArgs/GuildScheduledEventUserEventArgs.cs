namespace NetCord.Gateway;

public class GuildScheduledEventUserEventArgs : IJsonModel<JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs>
{
    JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs _jsonModel;

    public GuildScheduledEventUserEventArgs(JsonModels.EventArgs.JsonGuildScheduledEventUserEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public ulong GuildScheduledEventId => _jsonModel.GuildScheduledEventId;

    public ulong UserId => _jsonModel.UserId;

    public ulong GuildId => _jsonModel.GuildId;
}
