namespace NetCord;

public interface INamedChannel : IEntity, ISpanFormattable
{
    public string Name { get; }

    public string ToString();
}
