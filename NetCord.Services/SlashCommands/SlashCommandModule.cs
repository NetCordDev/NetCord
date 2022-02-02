namespace NetCord.Services.SlashCommands;

#nullable disable

public class SlashCommandModule<TContext> where TContext : BaseSlashCommandContext
{
    public TContext Context { get; internal set; }
}