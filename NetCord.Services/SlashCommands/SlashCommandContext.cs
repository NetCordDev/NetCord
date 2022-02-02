namespace NetCord.Services.SlashCommands;

public class SlashCommandContext : BaseSlashCommandContext
{
    public ApplicationCommandInteractionData Data => Interaction.Data;
    public Guild? Guild => Interaction.Guild;

    public SlashCommandContext(ApplicationCommandInteraction interaction, GatewayClient client) : base(interaction, client)
    {
    }
}