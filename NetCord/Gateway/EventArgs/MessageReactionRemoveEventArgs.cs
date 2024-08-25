namespace NetCord.Gateway;

public class MessageReactionRemoveEventArgs(JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs>
{
    JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageReactionRemoveEventArgs>.JsonModel => jsonModel;

    public ulong UserId => jsonModel.UserId;

    public ulong ChannelId => jsonModel.ChannelId;

    public ulong MessageId => jsonModel.MessageId;

    public ulong? GuildId => jsonModel.GuildId;

    public MessageReactionEmoji Emoji { get; } = new(jsonModel.Emoji);

    public bool Burst => jsonModel.Burst;

    public ReactionType Type => jsonModel.Type;
}
