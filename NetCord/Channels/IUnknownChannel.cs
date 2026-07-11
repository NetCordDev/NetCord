namespace NetCord;

/// <summary>
/// Represents a channel of an unresolved type.
/// </summary>
public interface IUnknownChannel : IEntity, ISpanFormattable
{
    /// <summary>
    /// The type of the unresolved channel.
    /// </summary>
    public ChannelType Type { get; }
}
