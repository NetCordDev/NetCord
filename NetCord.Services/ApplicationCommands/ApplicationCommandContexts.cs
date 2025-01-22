using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public class BaseApplicationCommandContext(ApplicationCommandInteraction interaction) : IApplicationCommandContext
{
    public ApplicationCommandInteraction Interaction => interaction;
}

public class ApplicationCommandContext(ApplicationCommandInteraction interaction, GatewayClient client)
    : BaseApplicationCommandContext(interaction),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

public class HttpApplicationCommandContext(ApplicationCommandInteraction interaction, RestClient client)
    : BaseApplicationCommandContext(interaction),
      IRestClientContext,
      IChannelContext,
      IUserContext
{
    public RestClient Client => client;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
}

public class BaseSlashCommandContext(SlashCommandInteraction interaction) : IApplicationCommandContext
{
    public SlashCommandInteraction Interaction => interaction;

    ApplicationCommandInteraction IApplicationCommandContext.Interaction => interaction;
}

public class SlashCommandContext(SlashCommandInteraction interaction, GatewayClient client)
    : BaseSlashCommandContext(interaction),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

public class HttpSlashCommandContext(SlashCommandInteraction interaction, RestClient client)
    : BaseSlashCommandContext(interaction),
      IRestClientContext,
      IChannelContext,
      IUserContext
{
    public RestClient Client => client;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
}

public class BaseUserCommandContext(UserCommandInteraction interaction) : IApplicationCommandContext
{
    public UserCommandInteraction Interaction => interaction;

    ApplicationCommandInteraction IApplicationCommandContext.Interaction => interaction;
}

public class UserCommandContext(UserCommandInteraction interaction, GatewayClient client)
    : BaseUserCommandContext(interaction),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
    public User Target => Interaction.Data.TargetUser;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

public class HttpUserCommandContext(UserCommandInteraction interaction, RestClient client)
    : BaseUserCommandContext(interaction),
      IRestClientContext,
      IChannelContext,
      IUserContext
{
    public RestClient Client => client;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
    public User Target => Interaction.Data.TargetUser;
}

public class BaseMessageCommandContext(MessageCommandInteraction interaction) : IApplicationCommandContext
{
    public MessageCommandInteraction Interaction => interaction;

    ApplicationCommandInteraction IApplicationCommandContext.Interaction => interaction;
}

public class MessageCommandContext(MessageCommandInteraction interaction, GatewayClient client)
    : BaseMessageCommandContext(interaction),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;
    public Guild? Guild => Interaction.Guild;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
    public RestMessage Target => Interaction.Data.TargetMessage;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

public class HttpMessageCommandContext(MessageCommandInteraction interaction, RestClient client)
    : BaseMessageCommandContext(interaction),
      IRestClientContext,
      IChannelContext,
      IUserContext
{
    public RestClient Client => client;
    public TextChannel Channel => Interaction.Channel;
    public User User => Interaction.User;
    public RestMessage Target => Interaction.Data.TargetMessage;
}
