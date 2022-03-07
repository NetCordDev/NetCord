namespace NetCord;

public class CategoryChannel : Channel, IGuildChannel
{
    internal CategoryChannel(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
    {
        PermissionOverwrites = jsonEntity.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public string Name => _jsonEntity.Name!;

    public int Position => (int)_jsonEntity.Position!;

    public IReadOnlyDictionary<DiscordId, PermissionOverwrite> PermissionOverwrites { get; }

    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? options = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, options).ConfigureAwait(false);
}