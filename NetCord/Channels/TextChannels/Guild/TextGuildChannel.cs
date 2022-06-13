namespace NetCord;

public class TextGuildChannel : TextChannel, IGuildChannel
{
    public Snowflake? ParentId => _jsonModel.ParentId;
    public string? Topic => _jsonModel.Topic;
    public bool IsNsfw => _jsonModel.IsNsfw;
    public int Slowmode => _jsonModel.Slowmode!.Value;

    public string Name => _jsonModel.Name!;

    public virtual int Position => _jsonModel.Position.GetValueOrDefault();

    public IReadOnlyDictionary<Snowflake, PermissionOverwrite> PermissionOverwrites { get; }

    public TextGuildChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        PermissionOverwrites = jsonModel.PermissionOverwrites.ToDictionaryOrEmpty(p => p.Id, p => new PermissionOverwrite(p));
    }

    public async Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? options = null) => (IGuildChannel)await _client.ModifyGuildChannelAsync(Id, action, options).ConfigureAwait(false);
}