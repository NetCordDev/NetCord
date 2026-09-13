namespace NetCord;

public interface IGuildChannel : IChannel
{
    ulong GuildId { get; }
}

/// <summary>
/// Represents a guild channel which directly contains Discord messages.
/// </summary>
/// <remarks>
/// This includes:
/// <list type="bullet">
/// <item>GUILD_TEXT</item>
/// <item>GUILD_ANNOUNCEMENT</item>
/// <item>GUILD_VOICE</item>
/// <item>GUILD_STAGE_VOICE</item>
/// <item>ANNOUNCEMENT_THREAD</item>
/// <item>PUBLIC_THREAD</item>
/// <item>PRIVATE_THREAD</item>
/// </list>
/// </remarks>
public interface IGuildMessageChannel : ITextChannel, IGuildChannel
{
}

/// <summary>
/// Represents a guild channel that has a position and can have a parent category.
/// </summary>
public interface IPositionedGuildChannel : IGuildChannel
{
    /// <summary>
    /// The channel's position within the guild channel list.
    /// </summary>
    /// <remarks>
    /// If two or more channels share a position, they are instead sorted by their ID.
    /// </remarks>
    int Position { get; }

    /// <summary>
    /// The ID of the channel's parent category, if it has one.
    /// </summary>
    ulong? ParentId { get; }
}

/// <summary>
/// Represents a guild channel that has permission overwrites.
/// </summary>
public interface IPermissionOverwriteChannel : IGuildChannel
{
    IReadOnlyDictionary<ulong, PermissionOverwrite> PermissionOverwrites { get; }
}

/// <summary>
/// Represents a guild channel that can have webhooks.
/// </summary>
/// <remarks>
/// This includes:
/// <list type="bullet">
/// <item>GUILD_TEXT</item>
/// <item>GUILD_ANNOUNCEMENT</item>
/// <item>GUILD_FORUM</item>
/// <item>GUILD_MEDIA</item>
/// </list>
/// For threads, use the parent channel's webhooks.
/// </remarks>
public interface IWebhookChannel : IGuildChannel
{
}

/// <summary>
/// Represents a guild channel that can be invited to.
/// </summary>
/// <remarks>
/// This includes:
/// <list type="bullet">
/// <item>GUILD_TEXT</item>
/// <item>GUILD_ANNOUNCEMENT</item>
/// <item>GUILD_VOICE</item>
/// <item>GUILD_STAGE_VOICE</item>
/// <item>GUILD_FORUM</item>
/// <item>GUILD_MEDIA</item>
/// </list>
/// </remarks>
public interface IInvitableGuildChannel : IGuildChannel
{
}
