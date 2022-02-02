namespace NetCord.Services.SlashCommands;

public class SlashCommandContext : BaseSlashCommandContext
{
    public ApplicationCommandInteractionData Data => Interaction.Data;
    public Guild? Guild => Interaction.Guild;
    public GatewayClient Client { get; }

    public SlashCommandContext(ApplicationCommandInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;
    }
}