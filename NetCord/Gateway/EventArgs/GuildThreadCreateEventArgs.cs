namespace NetCord.Gateway;

public class GuildThreadCreateEventArgs(IGuildThread thread, bool newlyCreated)
{
    public IGuildThread Thread { get; } = thread;

    public bool NewlyCreated { get; } = newlyCreated;
}
