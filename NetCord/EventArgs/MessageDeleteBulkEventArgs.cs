namespace NetCord;

public class MessageDeleteBulkEventArgs
{
    private readonly JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs _jsonEntity;

    internal MessageDeleteBulkEventArgs(JsonModels.EventArgs.JsonMessageDeleteBulkEventArgs jsonEntity)
    {
        _jsonEntity = jsonEntity;
    }

    public IEnumerable<DiscordId> MessageIds => _jsonEntity.MessageIds;

    public DiscordId ChannelId => _jsonEntity.ChannelId;

    public DiscordId? GuildId => _jsonEntity.GuildId;
}
