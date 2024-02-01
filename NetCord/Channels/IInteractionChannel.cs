namespace NetCord;

public interface IInteractionChannel : IEntity, ISpanFormattable
{
    public Permissions Permissions { get; }

    public string ToString();
}
