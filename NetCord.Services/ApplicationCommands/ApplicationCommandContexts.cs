using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.Interactions;

namespace NetCord.Services.ApplicationCommands;

public class ApplicationCommandContext : InteractionContext, IApplicationCommandContext
{
    public ApplicationCommandContext(ApplicationCommandInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override ApplicationCommandInteraction Interaction { get; }
}

public class HttpApplicationCommandContext : ApplicationCommandContext, IHttpInteractionContext
{
    public HttpApplicationCommandContext(ApplicationCommandInteraction interaction) : base(interaction)
    {
    }

    public InteractionCallback? Callback { get; set; }
}

public class BaseSlashCommandContext : ApplicationCommandContext
{
    public BaseSlashCommandContext(SlashCommandInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override SlashCommandInteraction Interaction { get; }
}

public class BaseHttpSlashCommandContext : BaseSlashCommandContext, IHttpInteractionContext
{
    public BaseHttpSlashCommandContext(SlashCommandInteraction interaction) : base(interaction)
    {
    }

    public InteractionCallback? Callback { get; set; }
}

public class SlashCommandContext : BaseSlashCommandContext, IGatewayClientContext, IGuildContext, IChannelContext, IUserContext
{
    public SlashCommandContext(SlashCommandInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;
    }

    public GatewayClient Client { get; }
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
}

public class HttpSlashCommandContext : BaseHttpSlashCommandContext, IRestClientContext, IChannelContext, IUserContext
{
    public HttpSlashCommandContext(SlashCommandInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;
    }

    public RestClient Client { get; }
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
}

public class BaseUserCommandContext : ApplicationCommandContext
{
    public BaseUserCommandContext(UserCommandInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override UserCommandInteraction Interaction { get; }
}

public class BaseHttpUserCommandContext : BaseUserCommandContext, IHttpInteractionContext
{
    public BaseHttpUserCommandContext(UserCommandInteraction interaction) : base(interaction)
    {
    }

    public InteractionCallback? Callback { get; set; }
}

public class UserCommandContext : BaseUserCommandContext, IGatewayClientContext, IGuildContext, IChannelContext, IUserContext
{
    public UserCommandContext(UserCommandInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;
    }

    public GatewayClient Client { get; }
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
    public User Target => Interaction.Data.TargetUser;
}

public class HttpUserCommandContext : BaseHttpUserCommandContext, IRestClientContext, IChannelContext, IUserContext
{
    public HttpUserCommandContext(UserCommandInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;
    }

    public RestClient Client { get; }
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
    public User Target => Interaction.Data.TargetUser;
}

public class BaseMessageCommandContext : ApplicationCommandContext
{
    public BaseMessageCommandContext(MessageCommandInteraction interaction) : base(interaction)
    {
        Interaction = interaction;
    }

    public override MessageCommandInteraction Interaction { get; }
}

public class BaseHttpMessageCommandContext : BaseMessageCommandContext, IHttpInteractionContext
{
    public BaseHttpMessageCommandContext(MessageCommandInteraction interaction) : base(interaction)
    {
    }

    public InteractionCallback? Callback { get; set; }
}

public class MessageCommandContext : BaseMessageCommandContext, IGatewayClientContext, IGuildContext, IChannelContext, IUserContext
{
    public MessageCommandContext(MessageCommandInteraction interaction, GatewayClient client) : base(interaction)
    {
        Client = client;
    }

    public GatewayClient Client { get; }
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
    public RestMessage Target => Interaction.Data.TargetMessage;
}

public class HttpMessageCommandContext : BaseHttpMessageCommandContext, IRestClientContext, IChannelContext, IUserContext
{
    public HttpMessageCommandContext(MessageCommandInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;
    }

    public RestClient Client { get; }
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
    public RestMessage Target => Interaction.Data.TargetMessage;
}
