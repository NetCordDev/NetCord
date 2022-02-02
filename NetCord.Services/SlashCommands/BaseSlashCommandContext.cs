namespace NetCord.Services.SlashCommands;

public class BaseSlashCommandContext
{
    public ApplicationCommandInteraction Interaction { get; }
    public GatewayClient Client { get; }

    public BaseSlashCommandContext(ApplicationCommandInteraction interaction, GatewayClient client)
    {
        Interaction = interaction;
        Client = client;
    }
}