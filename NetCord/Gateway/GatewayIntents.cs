namespace NetCord.Gateway;

/// <summary>
/// Intents are used to configure which events are passed to the bot via the gateway connection. Intents marked as privileged must be enabled in the application developer portal before being passed here, otherwise the connection will terminate with close code <c>4014</c>.
/// </summary>
[Flags]
public enum GatewayIntents : uint
{
    /// <summary>
    /// Associated with the following events:<br/>
    /// • Guild Events: <c>GUILD_CREATE, GUILD_UPDATE, GUILD_DELETE, GUILD_ROLE_CREATE, GUILD_ROLE_UPDATE, GUILD_ROLE_DELETE<br/></c>
    /// • Channel Events: <c>CHANNEL_CREATE, CHANNEL_UPDATE, CHANNEL_DELETE, CHANNEL_PINS_UPDATE<br/></c>
    /// • Thread Events: <c>THREAD_CREATE, THREAD_UPDATE, THREAD_DELETE, THREAD_LIST_SYNC, THREAD_MEMBER_UPDATE, THREAD_MEMBERS_UPDATE<br/></c>
    /// • Stage Events: <c>STAGE_INSTANCE_CREATE, STAGE_INSTANCE_UPDATE, STAGE_INSTANCE_DELETE</c>
    /// </summary>
    Guilds = 1 << 0,

    /// <summary>
    /// Privileged, associated with the following events:<br/>
    /// <c>GUILD_MEMBER_ADD, GUILD_MEMBER_UPDATE, GUILD_MEMBER_REMOVE, THREAD_MEMBERS_UPDATE</c>
    /// </summary>
    GuildUsers = 1 << 1,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>GUILD_AUDIT_LOG_ENTRY_CREATE, GUILD_BAN_ADD, GUILD_BAN_REMOVE</c>
    /// </summary>
    GuildModeration = 1 << 2,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>GUILD_EMOJIS_UPDATE, GUILD_STICKERS_UPDATE</c>
    /// </summary>
    GuildEmojisAndStickers = 1 << 3,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>GUILD_INTEGRATIONS_UPDATE, INTEGRATION_CREATE, INTEGRATION_UPDATE, INTEGRATION_DELETE</c>
    /// </summary>
    GuildIntegrations = 1 << 4,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>WEBHOOKS_UPDATE</c>
    /// </summary>
    GuildWebhooks = 1 << 5,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>INVITE_CREATE, INVITE_DELETE</c>
    /// </summary>
    GuildInvites = 1 << 6,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>VOICE_STATE_UPDATE</c>
    /// </summary>
    GuildVoiceStates = 1 << 7,

    /// <summary>
    /// Privileged, ssociated with the following events:<br/>
    /// <c>PRESENCE_UPDATE</c>
    /// </summary>
    GuildPresences = 1 << 8,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>MESSAGE_CREATE, MESSAGE_UPDATE, MESSAGE_DELETE, MESSAGE_DELETE_BULK</c>
    /// </summary>
    GuildMessages = 1 << 9,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>MESSAGE_REACTION_ADD, MESSAGE_REACTION_REMOVE, MESSAGE_REACTION_REMOVE_ALL, MESSAGE_REACTION_REMOVE_EMOJI</c>
    /// </summary>
    GuildMessageReactions = 1 << 10,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>TYPING_START</c>
    /// </summary>
    GuildMessageTyping = 1 << 11,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>MESSAGE_CREATE, MESSAGE_UPDATE, MESSAGE_DELETE, CHANNEL_PINS_UPDATE</c>
    /// </summary>
    DirectMessages = 1 << 12,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>MESSAGE_REACTION_ADD, MESSAGE_REACTION_REMOVE, MESSAGE_REACTION_REMOVE_ALL, MESSAGE_REACTION_REMOVE_EMOJI</c>
    /// </summary>
    DirectMessageReactions = 1 << 13,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>TYPING_START</c>
    /// </summary>
    DirectMessageTyping = 1 << 14,

    /// <summary>
    /// Privileged, ssociated with the following events:<br/>
    /// <c>MESSAGE_CREATE, MESSAGE_UPDATE, MESSAGE_DELETE, CHANNEL_PINS_UPDATE</c>
    /// </summary>
    MessageContent = 1 << 15,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>GUILD_SCHEDULED_EVENT_CREATE, GUILD_SCHEDULED_EVENT_UPDATE, GUILD_SCHEDULED_EVENT_DELETE, GUILD_SCHEDULED_EVENT_USER_ADD, GUILD_SCHEDULED_EVENT_USER_REMOVE</c>
    /// </summary>
    GuildScheduledEvents = 1 << 16,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>AUTO_MODERATION_RULE_CREATE, AUTO_MODERATION_RULE_UPDATE, AUTO_MODERATION_RULE_DELETE</c>
    /// </summary>
    AutoModerationConfiguration = 1 << 20,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>AUTO_MODERATION_ACTION_EXECUTION</c>
    /// </summary>
    AutoModerationExecution = 1 << 21,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>MESSAGE_POLL_VOTE_ADD, MESSAGE_POLL_VOTE_REMOVE</c>
    /// </summary>
    GuildMessagePolls = 1 << 24,

    /// <summary>
    /// Associated with the following events:<br/>
    /// <c>MESSAGE_POLL_VOTE_ADD, MESSAGE_POLL_VOTE_REMOVE</c>
    /// </summary>
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