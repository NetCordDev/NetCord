namespace NetCord;

/// <summary>
/// Represents a named channel.
/// </summary>
public interface INamedChannel : IEntity, ISpanFormattable
{
    /// <summary>
    /// The name of the channel object, between 1 and 100 characters.
    /// </summary>
    public string Name { get; }

    public string ToString();
}
