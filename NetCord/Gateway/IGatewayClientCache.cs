using System.Collections.Immutable;

namespace NetCord.Gateway;

public interface IGatewayClientCache : IDisposable
{
    public SelfUser? User { get; }
    public IReadOnlyDictionary<ulong, DMChannel> DMChannels { get; }
    public IReadOnlyDictionary<ulong, GroupDMChannel> GroupDMChannels { get; }
    public IReadOnlyDictionary<ulong, Guild> Guilds { get; }

    public IGatewayClientCache CacheDMChannel(DMChannel dMChannel);
    public IGatewayClientCache CacheGroupDMChannel(GroupDMChannel groupDMChannel);
    public IGatewayClientCache CacheGuild(Guild guild);
    public IGatewayClientCache CacheGuildUser(GuildUser user);
    public IGatewayClientCache CacheGuildUsers(ulong guildId, IEnumerable<KeyValuePair<ulong, GuildUser>> users);
    public IGatewayClientCache CachePresences(ulong guildId, IEnumerable<KeyValuePair<ulong, Presence>> presences);
    public IGatewayClientCache CacheRole(ulong guildId, Role role);
    public IGatewayClientCache CacheGuildScheduledEvent(GuildScheduledEvent scheduledEvent);
    public IGatewayClientCache CacheGuildEmojis(ulong guildId, ImmutableDictionary<ulong, GuildEmoji> emojis);
    public IGatewayClientCache CacheGuildStickers(ulong guildId, ImmutableDictionary<ulong, GuildSticker> stickers);
    public IGatewayClientCache CacheGuildThread(GuildThread thread);
    public IGatewayClientCache SyncGuildActiveThreads(ulong guildId, ImmutableDictionary<ulong, GuildThread> threads);
    public IGatewayClientCache CacheGuildChannel(ulong guildId, IGuildChannel channel);
    public IGatewayClientCache CacheStageInstance(StageInstance stageInstance);
    public IGatewayClientCache CacheSelfUser(SelfUser user);
    public IGatewayClientCache CacheVoiceState(ulong guildId, VoiceState voiceState);
    public IGatewayClientCache CachePresence(Presence presence);

    public IGatewayClientCache RemoveGuild(ulong guildId);
    public IGatewayClientCache RemoveGuildUser(ulong guildId, ulong userId);
    public IGatewayClientCache RemoveRole(ulong guildId, ulong roleId);
    public IGatewayClientCache RemoveGuildScheduledEvent(ulong guildId, ulong scheduledEventId);
    public IGatewayClientCache RemoveGuildThread(ulong guildId, ulong threadId);
    public IGatewayClientCache RemoveGuildChannel(ulong guildId, ulong channelId);
    public IGatewayClientCache RemoveStageInstance(ulong guildId, ulong stageInstanceId);
    public IGatewayClientCache RemoveVoiceState(ulong guildId, ulong userId);
}
