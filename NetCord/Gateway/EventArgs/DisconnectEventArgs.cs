namespace NetCord.Gateway;

public class DisconnectEventArgs(int? closeStatus, string? closeStatusDescription, bool reconnect)
{
    /// <summary>
    /// The close status code of the disconnect.
    /// </summary>
    public int? CloseStatus => closeStatus;

    /// <summary>
    /// The close status description of the disconnect.
    /// </summary>
    public string? CloseStatusDescription => closeStatusDescription;

    /// <summary>
    /// Whether the client will attempt to reconnect after the disconnect.
    /// </summary>
    public bool Reconnect => reconnect;
}
