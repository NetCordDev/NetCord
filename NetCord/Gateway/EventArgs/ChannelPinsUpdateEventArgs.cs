namespace NetCord.Gateway;

public class ChannelPinsUpdateEventArgs(JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs>
{
    JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonChannelPinsUpdateEventArgs>.JsonModel => jsonModel;

    public ulong? GuildId => jsonModel.GuildId;
    public ulong ChannelId => jsonModel.ChannelId;
    public DateTimeOffset? LastPinTimestamp => jsonModel.LastPinTimestamp;
}
