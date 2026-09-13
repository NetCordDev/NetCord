namespace NetCord;

/// <summary>
/// Represents any channel.
/// </summary>
/// <remarks>
/// This includes all text and voice channels, threads,
/// DMs, group DMs, categories and directory channels.
/// </remarks>
public interface IChannel : IEntity, ISpanFormattable
{
    ChannelType Type { get; }

    // Null means Discord did not provide flags.
    ChannelFlags? Flags { get; }

    public string ToString();
}

/// <summary>
/// Represents a channel that has a name.
/// </summary>
public interface INamedChannel : IChannel
{
    string Name { get; }
}


/// <summary>
/// Represents a channel representation which came with resolved interaction permissions.
/// </summary>
public interface IInteractionChannel : IChannel
{
    Permissions Permissions { get; }
}
