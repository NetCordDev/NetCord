namespace NetCord.Gateway;

/// <summary>
/// Represents the status of a WebSocket connection.
/// </summary>
public enum WebSocketStatus : byte
{
    /// <summary>
    /// The connection has been established and is ready for communication.
    /// </summary>
    Ready,

    /// <summary>
    /// The connection is being established.
    /// </summary>
    Connecting,

    /// <summary>
    /// The connection is closed or not started.
    /// </summary>
    Disconnected,
}
