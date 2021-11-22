namespace NetCord;

[Flags]
public enum Intents : uint
{
    Guilds = 1 << 0,
    /// <summary>
    /// Privileged
    /// </summary>
    GuildUsers = 1 << 1,
    GuildBans = 1 << 2,
    GuildEmojisAndStickers = 1 << 3,
    GuildInteractions = 1 << 4,
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
    GuildScheduledEvents = 1 << 16,
    All = Guilds
        | GuildUsers
        | GuildBans
        | GuildEmojisAndStickers
        | GuildInteractions
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
        | GuildScheduledEvents,
    AllNonPrivileged = Guilds
        | GuildBans
        | GuildEmojisAndStickers
        | GuildInteractions
        | GuildWebhooks
        | GuildInvites
        | GuildVoiceStates
        | GuildMessages
        | GuildMessageReactions
        | GuildMessageTyping
        | DirectMessages
        | DirectMessageReactions
        | DirectMessageTyping
        | GuildScheduledEvents,
}