using NetCord.Rest;

namespace NetCord;

public partial interface IGuildThread :
    IGuildMessageChannel,
    INamedChannel,
    IPinnableChannel
{
    ulong ParentId { get; }
    ulong OwnerId { get; }
    int MessageCount { get; }
    int UserCount { get; }
    GuildThreadMetadata Metadata { get; }
    ThreadCurrentUser? CurrentUser { get; }
    int TotalMessageSent { get; }
}

public partial interface IAnnouncementGuildThread : IGuildThread
{
}

public partial interface IUnknownGuildThread :
    IUnknownGuildChannel,
    IGuildThread
{
}

/// <summary>
/// Represents a private thread channel that is only viewale by those invited and those with the <c>MANAGE_THREADS</c> permission.
/// </summary>
public partial interface IPrivateGuildThread : IGuildThread
{
}

/// <summary>
/// Represents a public guild thread channel, which is a specialized <see cref="IGuildThread"/>.
/// </summary>
/// <remarks>
/// This also includes threads in forum and media channels.
/// </remarks>
public partial interface IPublicGuildThread : IGuildThread
{
    IReadOnlyList<ulong>? AppliedTags { get; }
}

public sealed class CreateGuildThreadResult
{
    public required IPublicGuildThread Thread { get; init; }
    public required RestMessage Message { get; init; }
}
