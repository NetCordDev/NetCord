namespace NetCord.Commands;

public abstract class BaseCommandModule<TContext> where TContext : ICommandContext
{
    public TContext Context { get; internal set; }
}