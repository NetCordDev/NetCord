namespace NetCord;

public interface ITextChannel : IChannel
{
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
public interface IPinnableChannel : ITextChannel
{
    DateTimeOffset? LastPin { get; }
}