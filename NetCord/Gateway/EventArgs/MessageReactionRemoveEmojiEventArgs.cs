namespace NetCord.Gateway;

public class MessageReactionRemoveEmojiEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs>
{
    JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveEmojiEventArgs>.JsonModel => jsonModel;

    public ulong ChannelId => jsonModel.ChannelId;

    public ulong? GuildId => jsonModel.GuildId;

    public ulong MessageId => jsonModel.MessageId;

    public MessageReactionEmoji Emoji { get; } = new(jsonModel.Emoji);
}
