namespace NetCord.Gateway;

public class ChannelPinsUpdateEventArgs : IJsonModel<JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs>
{
    JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs _jsonModel;

    public ulong? GuildId => _jsonModel.GuildId;
    public ulong ChannelId => _jsonModel.ChannelId;
    public DateTimeOffset? LastPinTimestamp => _jsonModel.LastPinTimestamp;

    public ChannelPinsUpdateEventArgs(JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs jsonModel)
    {
        _jsonModel = jsonModel;
    }
}
