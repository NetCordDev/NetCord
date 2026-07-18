namespace NetCord;

/// <summary>
/// An optional interface for acquiring channel permissions.
/// </summary>
public interface IInteractionChannel : IEntity, ISpanFormattable
{
    /// <summary>
    /// The chan
    /// </summary>
    public Permissions Permissions { get; }

    public string ToString();
}
