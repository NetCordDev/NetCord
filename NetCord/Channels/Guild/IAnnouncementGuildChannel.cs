namespace NetCord;

public partial interface IAnnouncementGuildChannel :
    IGuildMessageChannel,
    INamedChannel,
    IPositionedGuildChannel,
    IPermissionOverwriteChannel,
    IPinnableChannel,
    IWebhookChannel,
    IInvitableGuildChannel
{
    string? Topic { get; }
    bool? Nsfw { get; }
    int? Slowmode { get; }

    ThreadArchiveDuration? DefaultAutoArchiveDuration { get; }
}
