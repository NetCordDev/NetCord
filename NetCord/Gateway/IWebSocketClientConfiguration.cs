using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

internal interface IWebSocketClientConfiguration
{
    /// <summary>
    /// The provider for creating WebSocket connections. Defaults to <see cref="WebSockets.WebSocketConnectionProvider"/>.
    /// </summary>
    public IWebSocketConnectionProvider? WebSocketConnectionProvider { get; }

    /// <summary>
    /// The strategy for reconnecting the WebSocket. Defaults to <see cref="ReconnectStrategies.ReconnectStrategy"/>.
    /// </summary>
    public IReconnectStrategy? ReconnectStrategy { get; }

    /// <summary>
    /// The latency timer for tracking latency of the WebSocket connection. Defaults to <see cref="LatencyTimers.LatencyTimer"/>.
    /// </summary>
    public ILatencyTimer? LatencyTimer { get; }

    /// <summary>
    /// The provider for WebSocket rate limiters.
    /// </summary>
    public IRateLimiterProvider? RateLimiterProvider { get; }

    /// <summary>
    /// The default payload properties for WebSocket payloads. Defaults to <see langword="null"/>.
    /// </summary>
    public WebSocketPayloadProperties? DefaultPayloadProperties { get; }
}
