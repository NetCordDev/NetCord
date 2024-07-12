using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

internal interface IWebSocketClientConfiguration
{
    public IWebSocketLogger Logger { get; }
    public IWebSocket? WebSocket { get; }
    public IReconnectStrategy? ReconnectStrategy { get; }
    public ILatencyTimer? LatencyTimer { get; }
}
