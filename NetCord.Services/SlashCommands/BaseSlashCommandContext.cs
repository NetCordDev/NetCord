namespace NetCord.Services.SlashCommands;

public class BaseSlashCommandContext
{
    public ApplicationCommandInteraction Interaction { get; }

    public BaseSlashCommandContext(ApplicationCommandInteraction interaction)
    {
        Interaction = interaction;
    }
}