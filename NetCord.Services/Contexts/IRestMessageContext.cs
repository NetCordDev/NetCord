using NetCord.Rest;

namespace NetCord.Services;

/// <summary>
/// Provides access to <see cref="RestMessage"/> for commands and interactions.
/// </summary>
public interface IRestMessageContext : IContext
{
    /// <summary>
    /// The message associated with the handled command or interaction.
    /// </summary>
    public RestMessage Message { get; }
}
