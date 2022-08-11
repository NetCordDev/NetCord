namespace NetCord.Gateway;

[Flags]
public enum GatewayIntent : uint
{
    Guilds = 1 << 0,
    /// <summary>
    /// Privileged
    /// </summary>
    GuildUsers = 1 << 1,
    GuildBans = 1 << 2,
    GuildEmojisAndStickers = 1 << 3,
    GuildIntegrations = 1 << 4,
    GuildWebhooks = 1 << 5,
    GuildInvites = 1 << 6,
    GuildVoiceStates = 1 << 7,
    /// <summary>
    /// Privileged
    /// </summary>
    GuildPresences = 1 << 8,
    GuildMessages = 1 << 9,
    GuildMessageReactions = 1 << 10,
    GuildMessageTyping = 1 << 11,
    DirectMessages = 1 << 12,
    DirectMessageReactions = 1 << 13,
    DirectMessageTyping = 1 << 14,
    /// <summary>
    /// Privileged
    /// </summary>
    MessageContent = 1 << 15,
    GuildScheduledEvents = 1 << 16,
    AutoModerationConfiguration = 1 << 20,
    AutoModerationExecution = 1 << 21,
    All = Guilds
        | GuildUsers
        | GuildBans
        | GuildEmojisAndStickers
        | GuildIntegrations
        | GuildWebhooks
        | GuildInvites
        | GuildVoiceStates
        | GuildPresences
        | GuildMessages
        | GuildMessageReactions
        | GuildMessageTyping
        | DirectMessages
        | DirectMessageReactions
        | DirectMessageTyping
        | MessageContent
        | GuildScheduledEvents
        | AutoModerationConfiguration
        | AutoModerationExecution,
    AllNonPrivileged = Guilds
        | GuildBans
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
        | AutoModerationExecution,
}