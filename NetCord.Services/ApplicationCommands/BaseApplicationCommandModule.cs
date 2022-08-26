namespace NetCord.Services.ApplicationCommands;

#nullable disable

public class BaseApplicationCommandModule<TContext> where TContext : IApplicationCommandContext
{
    public TContext Context { get; internal set; }
}
