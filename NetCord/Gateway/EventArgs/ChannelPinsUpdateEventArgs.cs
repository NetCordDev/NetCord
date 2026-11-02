using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class ChannelPinsUpdateEventArgs(JsonChannelPinsUpdateEventArgs jsonModel)
{
    public ulong? GuildId { get; } = jsonModel.GuildId;

    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public DateTimeOffset? LastPinTimestamp { get; } = jsonModel.LastPinTimestamp;
}
