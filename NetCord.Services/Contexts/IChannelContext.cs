namespace NetCord.Services;

/// <summary>
/// Provides access to channel information for commands and interactions.
/// </summary>
public interface IChannelContext : IContext
{
    /// <summary>
    /// Channel in which the handled command or interaction was invoked.
    /// </summary>
    /// <remarks>May be <see langword="null"/> if the channel has not been cached.</remarks>
    public TextChannel? Channel { get; }
}
