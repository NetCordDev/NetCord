namespace NetCord;

public interface IGuildChannel : IEntity
{
    public string Name { get; }
    public int Position { get; }
    public IEnumerable<PermissionOverwrite> PermissionOverwrites { get; }
}
