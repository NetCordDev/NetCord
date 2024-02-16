namespace NetCord.Gateway;

public class GuildThreadCreateEventArgs(GuildThread thread, bool newlyCreated)
{
    public GuildThread Thread { get; } = thread;

    public bool NewlyCreated { get; } = newlyCreated;
}
