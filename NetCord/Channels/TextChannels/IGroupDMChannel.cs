namespace NetCord;

public interface IGroupDMChannel :
    ITextChannel,
    IPinnableChannel,
    INamedChannel
{
    /// <summary>
    /// The group channel's icon hash.
    /// </summary>
    string? IconHash { get; }

    /// <summary>
    /// The ID corresponding to the group channel's owner.
    /// </summary>
    ulong OwnerId { get; }

    /// <summary>
    /// The ID corresponding to the application managing the group channel, if any, otherwise <see langword="null"/>.
    /// </summary>
    ulong? ApplicationId { get; }

    /// <summary>
    /// Whether the group channel is managed by an application with <see cref="ApplicationFlags.GroupDMCreate"/> set.
    /// </summary>
    bool Managed { get; }
}
