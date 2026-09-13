namespace NetCord;

public interface IChannel : IEntity, ISpanFormattable
{
    ChannelType Type { get; }

    // Null means Discord did not provide flags.
    ChannelFlags? Flags { get; }

    public string ToString();
}

public interface INamedChannel : IChannel
{
    string Name { get; }
}

public interface IInteractionChannel : IChannel
{
    Permissions Permissions { get; }
}
