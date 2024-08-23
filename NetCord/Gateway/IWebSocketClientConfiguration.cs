﻿using NetCord.Gateway.LatencyTimers;
using NetCord.Gateway.ReconnectStrategies;
using NetCord.Gateway.WebSockets;

namespace NetCord.Gateway;

internal interface IWebSocketClientConfiguration
{
    public IWebSocketConnectionProvider? WebSocketConnectionProvider { get; }
    public IReconnectStrategy? ReconnectStrategy { get; }
    public ILatencyTimer? LatencyTimer { get; }
    public IRateLimiter? RateLimiter { get; }
    public WebSocketPayloadProperties? DefaultPayloadProperties { get; }
}
