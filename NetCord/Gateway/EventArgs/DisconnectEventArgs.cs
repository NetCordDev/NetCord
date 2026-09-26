using System.Net.WebSockets;

namespace NetCord.Gateway;

public class DisconnectEventArgs(WebSocketCloseStatus? closeStatus, string? closeStatusDescription, bool reconnect)
{
    /// <summary>
    /// The close status of the disconnect.
    /// </summary>
    public WebSocketCloseStatus? CloseStatus => closeStatus;

    /// <summary>
    /// The close status description of the disconnect.
    /// </summary>
    public string? CloseStatusDescription => closeStatusDescription;

    /// <summary>
    /// Whether the client will attempt to reconnect after the disconnect.
    /// </summary>
    public bool Reconnect => reconnect;
}
