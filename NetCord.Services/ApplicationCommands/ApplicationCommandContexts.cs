using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.Interactions;

namespace NetCord.Services.ApplicationCommands;

public abstract class ApplicationCommandContext : InteractionContext, IApplicationCommandContext, IUserContext, IGuildContext, IChannelContext
{
    public override abstract ApplicationCommandInteraction Interaction { get; }
    public Guild? Guild => Interaction.Guild;
    public TextChannel? Channel => Interaction.Channel;
    public User User => Interaction.User;

    public ApplicationCommandContext(GatewayClient client) : base(client)
    {
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
