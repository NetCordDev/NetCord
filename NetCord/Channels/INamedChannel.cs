namespace NetCord;

public interface INamedChannel : IEntity
{
    public string Name { get; }

    public string ToString();
}
