namespace NetCord.Gateway.Voice;

public class UserDisconnectEventArgs(ulong userId)
{
    /// <summary>
    /// The ID of the user that disconnected.
    /// </summary>
    public ulong UserId => userId;
}
