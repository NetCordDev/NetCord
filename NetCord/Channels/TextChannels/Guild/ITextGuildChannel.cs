namespace NetCord;

/// <summary>
/// Represents a regular text channel in a guild.
/// </summary>
/// <remarks>
/// This only includes the <c>GUILD_TEXT</c> channel type 
/// and excludes DMs and group DMs, annoucement channels,
/// voice and stage channels, and threads.
/// </remarks>
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
