using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class MessageDeleteEventArgs(JsonMessageDeleteEventArgs jsonModel)
{
    public ulong MessageId { get; } = jsonModel.MessageId;

    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public ulong? GuildId { get; } = jsonModel.GuildId;
}
