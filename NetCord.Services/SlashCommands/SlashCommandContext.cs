namespace NetCord.Services.SlashCommands;

public class SlashCommandContext : ISlashCommandContext, IUserContext, IGuildContext, IChannelContext
{
    public ApplicationCommandInteraction Interaction { get; }
    public GatewayClient Client { get; }
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel!;
    public User User => Interaction.User;

    public SlashCommandContext(ApplicationCommandInteraction interaction, GatewayClient client)
    {
        Interaction = interaction;
        Client = client;
    }
}