namespace NetCord;

public interface ITextGuildChannel :
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
    int? DefaultThreadSlowmode { get; }
}
