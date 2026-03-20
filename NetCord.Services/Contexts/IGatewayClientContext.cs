using NetCord.Gateway;

namespace NetCord.Services;

/// <summary>
/// Provides access to the <see cref="GatewayClient"/> associated with the handled command or interaction.
/// </summary>
public interface IGatewayClientContext : IContext
{
    /// <summary>
    /// The <see cref="GatewayClient"/> associated with the handled command or interaction.
    /// </summary>
    public GatewayClient Client { get; }
}
