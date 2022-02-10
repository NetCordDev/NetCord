namespace NetCord.Services.SlashCommands;

#nullable disable

public class BaseSlashCommandModule<TContext> where TContext : ISlashCommandContext
{
    public TContext Context { get; internal set; }
}