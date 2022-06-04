namespace NetCord;

public class TextGuildChannel : TextChannel, IGuildChannel
{
    public Snowflake? ParentId => _jsonEntity.ParentId;
    public string? Topic => _jsonEntity.Topic;
    public bool IsNsfw => _jsonEntity.IsNsfw;
    public int Slowmode => _jsonEntity.Slowmode!.Value;

    public string Name => _jsonEntity.Name!;

    public virtual int Position => _jsonEntity.Position.GetValueOrDefault();

    public IReadOnlyDictionary<Snowflake, PermissionOverwrite> PermissionOverwrites { get; }

    internal TextGuildChannel(JsonModels.JsonChannel jsonEntity, RestClient client) : base(jsonEntity, client)
    {
        PermissionOverwrites = jsonEntity.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? options = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, options).ConfigureAwait(false);
}