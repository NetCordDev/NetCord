namespace NetCord;

public class TextGuildChannel : TextChannel, IGuildChannel
{
    public DiscordId? ParentId => _jsonEntity.ParentId;
    public string? Topic => _jsonEntity.Topic;
    public bool IsNsfw => _jsonEntity.IsNsfw;
    public int Slowmode => (int)_jsonEntity.Slowmode!;

    public string Name => _jsonEntity.Name!;

    public virtual int Position => _jsonEntity.Position.GetValueOrDefault();

    public IReadOnlyDictionary<DiscordId, PermissionOverwrite> PermissionOverwrites { get; }

    internal TextGuildChannel(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
    {
        PermissionOverwrites = jsonEntity.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? options = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, options).ConfigureAwait(false);
}