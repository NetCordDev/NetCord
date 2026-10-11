namespace NetCord.Gateway;

public class MessageDeleteBulkEventArgs(JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs jsonModel)
{
    public IReadOnlyList<ulong> MessageIds { get; } = jsonModel.MessageIds;

    public ulong ChannelId { get; } = jsonModel.ChannelId;

    public ulong? GuildId { get; } = jsonModel.GuildId;
}
