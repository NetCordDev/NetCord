using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class MessageReactionRemoveEmojiEventArgs(JsonMessageReactionRemoveEmojiEventArgs jsonModel)
{
    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public ulong? GuildId { get; } = jsonModel.GuildId;

    public ulong MessageId { get; } = jsonModel.MessageId;

    public MessageReactionEmoji Emoji { get; } = new(jsonModel.Emoji);
}
