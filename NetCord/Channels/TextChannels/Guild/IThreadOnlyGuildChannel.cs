namespace NetCord;

public interface IThreadOnlyGuildChannel :
    IGuildChannel,
    INamedChannel,
    IPositionedGuildChannel,
    IPermissionOverwriteChannel,
    IWebhookChannel,
    IInvitableGuildChannel
{
    string? Topic { get; }
    bool? Nsfw { get; }

    ulong? LastThreadId { get; }

    int? Slowmode { get; }
    DateTimeOffset? LastPin { get; }

    ThreadArchiveDuration? DefaultAutoArchiveDuration { get; }
    int? DefaultThreadSlowmode { get; }

    IReadOnlyList<ForumTag> AvailableTags { get; }
    Emoji? DefaultReactionEmoji { get; }
    SortOrderType? DefaultSortOrder { get; }
}

public interface IForumGuildChannel : IThreadOnlyGuildChannel
{
    ForumLayoutType DefaultForumLayout { get; }
}

public interface IMediaGuildChannel : IThreadOnlyGuildChannel
{
}
