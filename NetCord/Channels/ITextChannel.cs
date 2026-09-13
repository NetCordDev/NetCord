namespace NetCord;

public partial interface ITextChannel : IChannel
{
    /// <summary>
    /// The ID corresponding to the last message sent within the channel. Can be <see langword="null"/> if the channel is empty.
    /// </summary>
    ulong? LastMessageId { get; }
}

/// <summary>
/// Represents a text channel that can have pins.
/// </summary>
/// <remarks>
/// This includes:
/// <list type="bullet">
/// <item>GUILD_TEXT</item>
/// <item>GUILD_ANNOUNCEMENT</item>
/// <item>ANNOUNCEMENT_THREAD</item>
/// <item>PUBLIC_THREAD</item>
/// <item>PRIVATE_THREAD</item>
/// <item>DM</item>
/// <item>GROUP_DM</item>
/// </list>
/// </remarks>
public partial interface IPinnableChannel : ITextChannel
{
    /// <summary>
    /// The timestamp of the last pinned message, if any, otherwise <see langword="null"/>.
    /// </summary>
    DateTimeOffset? LastPin { get; }
}