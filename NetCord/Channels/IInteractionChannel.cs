namespace NetCord;

/// <summary>
/// An optional interface for acquiring channel permissions.
/// </summary>
public interface IInteractionChannel : IEntity, ISpanFormattable
{
    public Permissions Permissions { get; }

    public string ToString();
}
