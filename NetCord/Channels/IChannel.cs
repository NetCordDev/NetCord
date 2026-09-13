namespace NetCord;

/// <summary>
/// Represents any channel.
/// </summary>
/// <remarks>
/// This includes all text and voice channels, threads,
/// DMs, group DMs, categories and directory channels.
/// </remarks>
public partial interface IChannel : IEntity, ISpanFormattable
{
    /// <summary>
    /// Additional information about the channel's state.
    /// </summary>
    ChannelFlags? Flags { get; }

    public string ToString();
}

/// <summary>
/// Represents a channel that has a name.
/// </summary>
public partial interface INamedChannel : IChannel
{
    /// <summary>
    /// The name of the channel.
    /// </summary>
    string Name { get; }
}


/// <summary>
/// Represents a channel representation which came with resolved interaction permissions.
/// </summary>
public partial interface IInteractionChannel : IChannel
{
    Permissions Permissions { get; }
}
