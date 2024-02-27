using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandContext(ApplicationCommandInteraction interaction) : IApplicationCommandContext
{
    public virtual ApplicationCommandInteraction Interaction { get; } = interaction;
}

public class BaseSlashCommandContext(SlashCommandInteraction interaction) : ApplicationCommandContext(interaction)
{
    public override SlashCommandInteraction Interaction { get; } = interaction;
}

public class SlashCommandContext(SlashCommandInteraction interaction, GatewayClient client) : BaseSlashCommandContext(interaction), IGatewayClientContext, IGuildContext, IChannelContext, IUserContext
{
    public GatewayClient Client { get; } = client;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
}

public class HttpSlashCommandContext(SlashCommandInteraction interaction, RestClient client) : BaseSlashCommandContext(interaction), IRestClientContext, IChannelContext, IUserContext
{
    public RestClient Client { get; } = client;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
}

public class BaseUserCommandContext(UserCommandInteraction interaction) : ApplicationCommandContext(interaction)
{
    public override UserCommandInteraction Interaction { get; } = interaction;
}

public class UserCommandContext(UserCommandInteraction interaction, GatewayClient client) : BaseUserCommandContext(interaction), IGatewayClientContext, IGuildContext, IChannelContext, IUserContext
{
    public GatewayClient Client { get; } = client;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
    public User Target => Interaction.Data.TargetUser;
}

public class HttpUserCommandContext(UserCommandInteraction interaction, RestClient client) : BaseUserCommandContext(interaction), IRestClientContext, IChannelContext, IUserContext
{
    public RestClient Client { get; } = client;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
    public User Target => Interaction.Data.TargetUser;
}

public class BaseMessageCommandContext(MessageCommandInteraction interaction) : ApplicationCommandContext(interaction)
{
    public override MessageCommandInteraction Interaction { get; } = interaction;
}

public class MessageCommandContext(MessageCommandInteraction interaction, GatewayClient client) : BaseMessageCommandContext(interaction), IGatewayClientContext, IGuildContext, IChannelContext, IUserContext
{
    public GatewayClient Client { get; } = client;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
    public RestMessage Target => Interaction.Data.TargetMessage;
}

public class HttpMessageCommandContext(MessageCommandInteraction interaction, RestClient client) : BaseMessageCommandContext(interaction), IRestClientContext, IChannelContext, IUserContext
{
    public RestClient Client { get; } = client;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
    public RestMessage Target => Interaction.Data.TargetMessage;
}
