namespace NetCord.Services;

/// <summary>
/// Provides access to user information for commands and interactions.
/// </summary>
public interface IUserContext : IContext
{
    /// <summary>
    /// User who invoked the handled command or interaction.
    /// </summary>
    public User User { get; }
}
