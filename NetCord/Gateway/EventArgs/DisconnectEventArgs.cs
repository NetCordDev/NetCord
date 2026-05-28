namespace NetCord.Gateway;

public class DisconnectEventArgs(bool reconnect)
{
    /// <summary>
    /// Whether the client will attempt to reconnect after the disconnect.
    /// </summary>
    public bool Reconnect => reconnect;
}
