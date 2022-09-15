namespace NetCord.Gateway;

public class GuildThreadCreateEventArgs
{
    public GuildThreadCreateEventArgs(GuildThread thread, bool newlyCreated)
    {
        Thread = thread;
        NewlyCreated = newlyCreated;
    }

    public GuildThread Thread { get; }

    public bool NewlyCreated { get; }
}
