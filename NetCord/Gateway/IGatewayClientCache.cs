using System.Collections.Immutable;

namespace NetCord.Gateway;

public interface IGatewayClientCache : IDictionaryProvider, IDisposable
{
    public CurrentUser? User { get; }
    public IReadOnlyDictionary<ulong, Guild> Guilds { get; }

    public IGatewayClientCache CacheGuild(Guild guild);
    public IGatewayClientCache CacheGuildUser(GuildUser user);
    public IGatewayClientCache CacheGuildUsers(ulong guildId, IReadOnlyList<GuildUser> users);
    public IGatewayClientCache CachePresences(ulong guildId, IReadOnlyList<Presence> presences);
    public IGatewayClientCache CacheRole(Role role);
    public IGatewayClientCache CacheGuildScheduledEvent(GuildScheduledEvent scheduledEvent);
    public IGatewayClientCache CacheGuildEmojis(ulong guildId, IReadOnlyDictionary<ulong, GuildEmoji> emojis);
    public IGatewayClientCache CacheGuildStickers(ulong guildId, IReadOnlyDictionary<ulong, GuildSticker> stickers);
    public IGatewayClientCache CacheGuildThread(GuildThread thread);
    public IGatewayClientCache CacheGuildChannel(IGuildChannel channel);
    public IGatewayClientCache CacheStageInstance(StageInstance stageInstance);
    public IGatewayClientCache CacheCurrentUser(CurrentUser user);
    public IGatewayClientCache CacheVoiceState(VoiceState voiceState);
    public IGatewayClientCache CachePresence(Presence presence);

    public IGatewayClientCache SyncGuildActiveThreads(ulong guildId, IReadOnlyDictionary<ulong, GuildThread> threads);
    public IGatewayClientCache SyncGuilds(IReadOnlyList<ulong> guildIds);

    public IGatewayClientCache RemoveGuild(ulong guildId);
    public IGatewayClientCache RemoveGuildUser(ulong guildId, ulong userId);
    public IGatewayClientCache RemoveRole(ulong guildId, ulong roleId);
    public IGatewayClientCache RemoveGuildScheduledEvent(ulong guildId, ulong scheduledEventId);
    public IGatewayClientCache RemoveGuildThread(ulong guildId, ulong threadId);
    public IGatewayClientCache RemoveGuildChannel(ulong guildId, ulong channelId);
    public IGatewayClientCache RemoveStageInstance(ulong guildId, ulong stageInstanceId);
    public IGatewayClientCache RemoveVoiceState(ulong guildId, ulong userId);
}
