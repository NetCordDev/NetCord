using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class MessagePollVoteEventArgs(JsonMessagePollVoteEventArgs jsonModel)
{
    public ulong UserId { get; } = jsonModel.UserId;

    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public ulong MessageId { get; } = jsonModel.MessageId;

    public ulong? GuildId { get; } = jsonModel.GuildId;

    public int AnswerId { get; } = jsonModel.AnswerId;
}
