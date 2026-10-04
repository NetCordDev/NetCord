using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class MessageReactionRemoveAllEventArgs(JsonMessageReactionRemoveAllEventArgs jsonModel)
{
    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public ulong MessageId { get; } = jsonModel.MessageId;

    public ulong? GuildId { get; } = jsonModel.GuildId;
}
