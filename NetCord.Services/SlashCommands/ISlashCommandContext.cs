namespace NetCord.Services.SlashCommands;

public interface ISlashCommandContext : IContext
{
    public ApplicationCommandInteraction Interaction { get; }
}