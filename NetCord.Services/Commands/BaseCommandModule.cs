namespace NetCord.Services.Commands;

#nullable disable

public abstract class BaseCommandModule<TContext> where TContext : ICommandContext
{
    public TContext Context { get; internal set; }
}