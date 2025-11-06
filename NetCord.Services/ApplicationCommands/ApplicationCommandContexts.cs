using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

/// <summary>
/// Base context for handling application command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseApplicationCommandContext(ApplicationCommandInteraction interaction) : IApplicationCommandContext
{
    public ApplicationCommandInteraction Interaction => interaction;
}

/// <summary>
/// Context for handling application command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseApplicationCommandContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class ApplicationCommandContext(ApplicationCommandInteraction interaction, GatewayClient client)
    : BaseApplicationCommandContext(interaction),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;

    public Guild? Guild => Interaction.Guild;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    public User User => Interaction.User;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based application command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseApplicationCommandContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpApplicationCommandContext(ApplicationCommandInteraction interaction, RestClient client)
    : BaseApplicationCommandContext(interaction),
      IRestClientContext,
      IChannelContext,
      IUserContext
{
    public RestClient Client => client;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    public User User => Interaction.User;
}

/// <summary>
/// Base context for handling slash command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseSlashCommandContext(SlashCommandInteraction interaction) : IApplicationCommandContext
{
    /// <inheritdoc cref="IApplicationCommandContext.Interaction" />
    public SlashCommandInteraction Interaction => interaction;

    ApplicationCommandInteraction IApplicationCommandContext.Interaction => interaction;
}

/// <summary>
/// Context for handling slash command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseSlashCommandContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class SlashCommandContext(SlashCommandInteraction interaction, GatewayClient client)
    : BaseSlashCommandContext(interaction),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;

    public Guild? Guild => Interaction.Guild;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    public User User => Interaction.User;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based slash command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseSlashCommandContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpSlashCommandContext(SlashCommandInteraction interaction, RestClient client)
    : BaseSlashCommandContext(interaction),
      IRestClientContext,
      IChannelContext,
      IUserContext
{
    public RestClient Client => client;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    public User User => Interaction.User;
}

/// <summary>
/// Base context for handling user command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseUserCommandContext(UserCommandInteraction interaction) : IApplicationCommandContext
{
    /// <inheritdoc cref="IApplicationCommandContext.Interaction" />
    public UserCommandInteraction Interaction => interaction;

    ApplicationCommandInteraction IApplicationCommandContext.Interaction => interaction;
}

/// <summary>
/// Context for handling user command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseUserCommandContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class UserCommandContext(UserCommandInteraction interaction, GatewayClient client)
    : BaseUserCommandContext(interaction),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;

    public Guild? Guild => Interaction.Guild;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    public User User => Interaction.User;

    /// <summary>
    /// The target user of the user command.
    /// </summary>
    public User Target => Interaction.Data.TargetUser;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based user command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseUserCommandContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpUserCommandContext(UserCommandInteraction interaction, RestClient client)
    : BaseUserCommandContext(interaction),
      IRestClientContext,
      IChannelContext,
      IUserContext
{
    public RestClient Client => client;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    public User User => Interaction.User;

    /// <summary>
    /// The target user of the user command.
    /// </summary>
    public User Target => Interaction.Data.TargetUser;
}

/// <summary>
/// Base context for handling message command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseMessageCommandContext(MessageCommandInteraction interaction) : IApplicationCommandContext
{
    /// <inheritdoc cref="IApplicationCommandContext.Interaction" />
    public MessageCommandInteraction Interaction => interaction;

    ApplicationCommandInteraction IApplicationCommandContext.Interaction => interaction;
}

/// <summary>
/// Context for handling message command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseMessageCommandContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class MessageCommandContext(MessageCommandInteraction interaction, GatewayClient client)
    : BaseMessageCommandContext(interaction),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;

    public Guild? Guild => Interaction.Guild;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    public User User => Interaction.User;

    /// <summary>
    /// The target message of the message command.
    /// </summary>
    public RestMessage Target => Interaction.Data.TargetMessage;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based message command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseMessageCommandContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpMessageCommandContext(MessageCommandInteraction interaction, RestClient client)
    : BaseMessageCommandContext(interaction),
      IRestClientContext,
      IChannelContext,
      IUserContext
{
    public RestClient Client => client;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    public User User => Interaction.User;

    /// <summary>
    /// The target message of the message command.
    /// </summary>
    public RestMessage Target => Interaction.Data.TargetMessage;
}

/// <summary>
/// Base context for handling entry point command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="Interaction" path="/summary"/></param>
public class BaseEntryPointCommandContext(EntryPointCommandInteraction interaction) : IApplicationCommandContext
{
    /// <inheritdoc cref="IApplicationCommandContext.Interaction" />
    public EntryPointCommandInteraction Interaction => interaction;

    ApplicationCommandInteraction IApplicationCommandContext.Interaction => interaction;
}

/// <summary>
/// Context for handling entry point command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseEntryPointCommandContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class EntryPointCommandContext(EntryPointCommandInteraction interaction, GatewayClient client)
    : BaseEntryPointCommandContext(interaction),
      IGatewayClientContext,
      IGuildContext,
      IChannelContext,
      IUserContext
{
    public GatewayClient Client => client;

    public Guild? Guild => Interaction.Guild;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    public User User => Interaction.User;

    ulong? IGuildContext.GuildId => Interaction.GuildId;
}

/// <summary>
/// Context for handling HTTP-based entry point command interactions.
/// </summary>
/// <param name="interaction"><inheritdoc cref="BaseEntryPointCommandContext.Interaction" path="/summary"/></param>
/// <param name="client"><inheritdoc cref="Client" path="/summary"/></param>
public class HttpEntryPointCommandContext(EntryPointCommandInteraction interaction, RestClient client)
    : BaseEntryPointCommandContext(interaction),
      IRestClientContext,
      IChannelContext,
      IUserContext
{
    public RestClient Client => client;

    /// <inheritdoc cref="IChannelContext.Channel" path="/summary" />
    public TextChannel Channel => Interaction.Channel;

    public User User => Interaction.User;
}
