namespace NetCord;

/// <summary>
/// Contains additional metadata for threads, irrelevant for standard channels.
/// </summary>
public class GuildThreadMetadata(JsonModels.JsonGuildThreadMetadata jsonModel)
{
    /// <summary>
    /// Whether the thread is currently archived.
    /// </summary>
    public bool Archived { get; } = jsonModel.Archived;

    /// <summary>
    /// How long the thread must be inactive before auto-archiving occurs.
    /// </summary>
    public ThreadArchiveDuration AutoArchiveDuration { get; } = jsonModel.AutoArchiveDuration;

    /// <summary>
    /// When the thread's activity status was last updated.
    /// </summary>
    public DateTimeOffset ArchiveTimestamp { get; } = jsonModel.ArchiveTimestamp;

    /// <summary>
    /// Whether the thread has been locked.
    /// </summary>
    /// <remarks>
    /// Locked threads can only be unlocked using the <see cref="Permissions.ManageThreads"/> permission.
    /// </remarks>
    public bool Locked { get; } = jsonModel.Locked;

    /// <summary>
    /// Whether non-moderators can add other non-moderators to a thread.
    /// </summary>
    /// <remarks>
    /// Only available for private threads.
    /// </remarks>
    public bool? Invitable { get; } = jsonModel.Invitable;

    /// <summary>
    /// When the thread was initially created.
    /// </summary>
    /// <remarks>
    /// Only populated for threads created after 2022-01-09.
    /// </remarks>
    public DateTimeOffset? CreateTimestamp { get; } = jsonModel.CreateTimestamp;
}
