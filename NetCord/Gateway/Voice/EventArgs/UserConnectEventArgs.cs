namespace NetCord.Gateway.Voice;

public class UserConnectEventArgs(IReadOnlyList<ulong> userIds)
{
    /// <summary>
    /// The IDs of the users that connected.
    /// </summary>
    public IReadOnlyList<ulong> UserIds => userIds;
}
