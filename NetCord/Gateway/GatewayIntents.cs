namespace NetCord.Gateway;

[Flags]
public enum GatewayIntents : uint
{
    Guilds = 1 << 0,
    /// <summary>
    /// Privileged
    /// </summary>
    GuildUsers = 1 << 1,
    GuildModeration = 1 << 2,
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
    GuildMessagePolls = 1 << 24,
    DirectMessagePolls = 1 << 25,
    All = AllNonPrivileged
        | GuildUsers
        | GuildPresences
        | MessageContent,
        
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
}
