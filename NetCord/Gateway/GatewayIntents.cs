namespace NetCord.Gateway;

/// <summary>
/// Intents are used to configure which events are passed to the bot via the gateway connection. Intents marked as privileged must be enabled in the application developer portal before being passed here, otherwise the connection will terminate with close code 4014.
/// </summary>
[Flags]
public enum GatewayIntents : uint
{
    /// <summary>
    /// Associated with the following events:
    /// <list type="bullet">
    ///		<item>
    ///			<description>
    ///				Guild Events: <see cref="GatewayClient.GuildCreate"/>, <see cref="GatewayClient.GuildUpdate"/>, <see cref="GatewayClient.GuildDelete"/><br/>
    ///			</description>
    ///		</item>
    ///		<item>
    ///			<description>
    ///				Role Events: <see cref="GatewayClient.RoleCreate"/>, <see cref="GatewayClient.RoleUpdate"/>, <see cref="GatewayClient.RoleDelete"/><br/>
    ///			</description>
    ///		</item>
    ///		<item>
    ///			<description>
    ///				Channel Events: <see cref="GatewayClient.GuildChannelCreate"/>, <see cref="GatewayClient.GuildChannelUpdate"/>, <see cref="GatewayClient.GuildChannelDelete"/>, <see cref="GatewayClient.ChannelPinsUpdate"/><br/>
    ///			</description>
    ///		</item>
    ///		<item>
    ///			<description>
    ///				Thread Events: <see cref="GatewayClient.GuildThreadCreate"/>, <see cref="GatewayClient.GuildThreadUpdate"/>, <see cref="GatewayClient.GuildThreadDelete"/>, <see cref="GatewayClient.GuildThreadListSync"/><br/>
    ///			</description>
    ///		</item>
    ///		<item>
    ///			<description>
    ///				Thread User Events: <see cref="GatewayClient.GuildThreadUserUpdate"/>, <see cref="GatewayClient.GuildThreadUsersUpdate"/><br/>
    ///			</description>
    ///		</item>
    ///		<item>
    ///			<description>
    ///				Stage Events: <see cref="GatewayClient.StageInstanceCreate"/>, <see cref="GatewayClient.StageInstanceUpdate"/>, <see cref="GatewayClient.StageInstanceDelete"/>
    ///			</description>
    ///		</item>
    /// </list>
    /// </summary>
    Guilds = 1 << 0,

    /// <summary>
    /// Privileged, associated with the following events:<br/>
    /// <see cref="GatewayClient.GuildUserAdd"/>, <see cref="GatewayClient.GuildUserUpdate"/>, <see cref="GatewayClient.GuildUserRemove"/>, <see cref="GatewayClient.GuildThreadUsersUpdate"/>
    /// </summary>
    GuildUsers = 1 << 1,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.GuildAuditLogEntryCreate"/>, <see cref="GatewayClient.GuildBanAdd"/>, <see cref="GatewayClient.GuildBanRemove"/>
    /// </summary>
    GuildModeration = 1 << 2,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.GuildEmojisUpdate"/>, <see cref="GatewayClient.GuildStickersUpdate"/>
    /// </summary>
    GuildEmojisAndStickers = 1 << 3,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.GuildIntegrationsUpdate"/>, <see cref="GatewayClient.GuildIntegrationCreate"/>, <see cref="GatewayClient.GuildIntegrationUpdate"/>, <see cref="GatewayClient.GuildIntegrationDelete"/>
    /// </summary>
    GuildIntegrations = 1 << 4,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.WebhooksUpdate"/>
    /// </summary>
    GuildWebhooks = 1 << 5,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.GuildInviteCreate"/>, <see cref="GatewayClient.GuildInviteDelete"/>
    /// </summary>
    GuildInvites = 1 << 6,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.VoiceStateUpdate"/>
    /// </summary>
    GuildVoiceStates = 1 << 7,

    /// <summary>
    /// Privileged, associated with the following events:<br/>
    /// <see cref="GatewayClient.PresenceUpdate"/>
    /// </summary>
    GuildPresences = 1 << 8,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.MessageCreate"/>, <see cref="GatewayClient.MessageUpdate"/>, <see cref="GatewayClient.MessageDelete"/>, <see cref="GatewayClient.MessageDeleteBulk"/>
    /// </summary>
    GuildMessages = 1 << 9,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.MessageReactionAdd"/>, <see cref="GatewayClient.MessageReactionRemove"/>, <see cref="GatewayClient.MessageReactionRemoveAll"/>, <see cref="GatewayClient.MessageReactionRemoveEmoji"/>
    /// </summary>
    GuildMessageReactions = 1 << 10,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.TypingStart"/>
    /// </summary>
    GuildMessageTyping = 1 << 11,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.MessageCreate"/>, <see cref="GatewayClient.MessageUpdate"/>, <see cref="GatewayClient.MessageDelete"/>, <see cref="GatewayClient.ChannelPinsUpdate"/>
    /// </summary>
    DirectMessages = 1 << 12,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.MessageReactionAdd"/>, <see cref="GatewayClient.MessageReactionRemove"/>, <see cref="GatewayClient.MessageReactionRemoveAll"/>, <see cref="GatewayClient.MessageReactionRemoveEmoji"/>
    /// </summary>
    DirectMessageReactions = 1 << 13,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.TypingStart"/>
    /// </summary>
    DirectMessageTyping = 1 << 14,

    /// <summary>
    /// Privileged, ssociated with the following events:<br/>
    /// <see cref="GatewayClient.MessageCreate"/>, <see cref="GatewayClient.MessageUpdate"/>, <see cref="GatewayClient.MessageDelete"/>, <see cref="GatewayClient.ChannelPinsUpdate"/>
    /// </summary>
    MessageContent = 1 << 15,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.GuildScheduledEventCreate"/>, <see cref="GatewayClient.GuildScheduledEventUpdate"/>, <see cref="GatewayClient.GuildScheduledEventDelete"/>, <see cref="GatewayClient.GuildScheduledEventUserAdd"/>, <see cref="GatewayClient.GuildScheduledEventUserRemove"/>
    /// </summary>
    GuildScheduledEvents = 1 << 16,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.AutoModerationRuleCreate"/>, <see cref="GatewayClient.AutoModerationRuleUpdate"/>, <see cref="GatewayClient.AutoModerationRuleDelete"/>
    /// </summary>
    AutoModerationConfiguration = 1 << 20,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <see cref="GatewayClient.AutoModerationActionExecution"/>
    /// </summary>
    AutoModerationExecution = 1 << 21,

    GuildMessagePolls = 1 << 24,

    DirectMessagePolls = 1 << 25,

    /// <summary>
    /// Implies all available intents, excluding privileged intents.
    /// </summary>
    AllNonPrivileged = Guilds
        | GuildModeration
        | GuildEmojisAndStickers
        | GuildIntegrations
        | GuildWebhooks
        | GuildInvites
        | GuildVoiceStates
        | GuildMessages
        | GuildMessageReactions
        | GuildMessageTyping
        | DirectMessages
        | DirectMessageReactions
        | DirectMessageTyping
        | GuildScheduledEvents
        | AutoModerationConfiguration
        | AutoModerationExecution
        | GuildMessagePolls
        | DirectMessagePolls,

    /// <summary>
    /// Implies all available intents, including privileged intents.
    /// </summary>
    All = AllNonPrivileged
        | GuildUsers
        | GuildPresences
        | MessageContent,
}
