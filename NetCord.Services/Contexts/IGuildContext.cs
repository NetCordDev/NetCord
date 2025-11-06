using NetCord.Gateway;

namespace NetCord.Services;

/// <summary>
/// Provides access to guild information for commands and interactions.
/// </summary>
public interface IGuildContext : IContext
{
    /// <summary>
    /// Guild in which the handled command or interaction was invoked.
    /// </summary>
    /// <remarks>May be <see langword="null"/> if the handled command or interaction was invoked outside of a guild or if the guild has not been cached.</remarks>
    public Guild? Guild { get; }

    protected internal ulong? GuildId { get; }
}
