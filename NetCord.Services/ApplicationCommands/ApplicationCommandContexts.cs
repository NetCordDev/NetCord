using NetCord.Gateway;

namespace NetCord.Services.ApplicationCommands;

public abstract class ApplicationCommandContext : IApplicationCommandContext, IUserContext, IGuildContext, IChannelContext
{
    public abstract ApplicationCommandInteraction Interaction { get; }
    public GatewayClient Client { get; }
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel!;
    public User User => Interaction.User;

    public ApplicationCommandContext(GatewayClient client)
    {
        Client = client;
    }
}

public class SlashCommandContext : ApplicationCommandContext
{
    public SlashCommandContext(SlashCommandInteraction interaction, GatewayClient client) : base(client)
    {
        Interaction = interaction;
    }

    public override SlashCommandInteraction Interaction { get; }
}

public class UserCommandContext : ApplicationCommandContext
{
    public UserCommandContext(UserCommandInteraction interaction, GatewayClient client) : base(client)
    {
        Interaction = interaction;
    }

    public override UserCommandInteraction Interaction { get; }
    public User Target => Interaction.Data.TargetUser;
}

public class MessageCommandContext : ApplicationCommandContext
{
    public MessageCommandContext(MessageCommandInteraction interaction, GatewayClient client) : base(client)
    {
        Interaction = interaction;
    }

    public override MessageCommandInteraction Interaction { get; }
    public RestMessage Target => Interaction.Data.TargetMessage;
}