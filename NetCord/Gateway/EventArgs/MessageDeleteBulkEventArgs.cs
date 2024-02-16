namespace NetCord.Gateway;

public class MessageDeleteBulkEventArgs(JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs jsonModel) : IJsonModel<JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs>
{
    JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs IJsonModel<JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs>.JsonModel => jsonModel;

    public IReadOnlyList<ulong> MessageIds => jsonModel.MessageIds;

    public ulong ChannelId => jsonModel.ChannelId;

    public ulong? GuildId => jsonModel.GuildId;
}
