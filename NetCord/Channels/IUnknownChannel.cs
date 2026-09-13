namespace NetCord;

/// <summary>
/// Represents a channel of an unresolved type.
/// </summary>
public partial interface IUnknownChannel : IChannel
{
    /// <summary>
    /// The unresolved channel's type.
    /// </summary>
    ChannelType Type { get; }
}
