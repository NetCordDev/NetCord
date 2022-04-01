namespace NetCord;

public interface IGuildChannel : IEntity
{
    public string Name { get; }
    public int Position { get; }
    public IReadOnlyDictionary<Snowflake, PermissionOverwrite> PermissionOverwrites { get; }

    public Task<IGuildChannel> ModifyAsync(Action<GuildChannelOptions> action, RequestProperties? options = null);
}