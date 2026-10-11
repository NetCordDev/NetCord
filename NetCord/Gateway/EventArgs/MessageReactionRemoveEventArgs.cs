using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class MessageReactionRemoveEventArgs(JsonMessageReactionRemoveEventArgs jsonModel)
{
    public ulong UserId { get; } = jsonModel.UserId;

    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public ulong MessageId { get; } = jsonModel.MessageId;

    public ulong? GuildId { get; } = jsonModel.GuildId;

    public MessageReactionEmoji Emoji { get; } = new(jsonModel.Emoji);

    public bool Burst { get; } = jsonModel.Burst;

    public ReactionType Type { get; } = jsonModel.Type;
}
