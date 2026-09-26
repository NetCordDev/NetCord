namespace NetCord;

/// <summary>
/// Represents a channel of an unresolved type.
/// </summary>
public interface IUnknownChannel : IEntity, ISpanFormattable
{
    /// <summary>
    /// The unresolved channel's type.
    /// </summary>
    public ChannelType Type { get; }
}
