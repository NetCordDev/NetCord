using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using NetCord.Gateway.JsonModels;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

internal sealed class EmptyConcurrentGatewayClientCacheProvider : ConcurrentGatewayClientCacheProvider
{
    public static EmptyConcurrentGatewayClientCacheProvider Instance { get; } = new();

    private EmptyConcurrentGatewayClientCacheProvider()
    {
    }

    public override ConcurrentGatewayClientCache Create(ulong clientId, RestClient client) => new();
}

internal sealed class JsonConcurrentGatewayClientCacheProvider(JsonGatewayClientCache jsonModel) : ConcurrentGatewayClientCacheProvider
{
    public override ConcurrentGatewayClientCache Create(ulong clientId, RestClient client) => new(jsonModel, clientId, client);
}

public abstract class ConcurrentGatewayClientCacheProvider : IGatewayClientCacheProvider
{
    public static ConcurrentGatewayClientCacheProvider Empty => EmptyConcurrentGatewayClientCacheProvider.Instance;

    public static ConcurrentGatewayClientCacheProvider FromJson(JsonGatewayClientCache jsonModel) => new JsonConcurrentGatewayClientCacheProvider(jsonModel);

    private protected ConcurrentGatewayClientCacheProvider()
    {
    }

    IGatewayClientCache IGatewayClientCacheProvider.Create(ulong clientId, RestClient client) => Create(clientId, client);

    public abstract ConcurrentGatewayClientCache Create(ulong clientId, RestClient client);
}

public sealed class ConcurrentGatewayClientCache : IGatewayClientCache
{
    internal ConcurrentGatewayClientCache()
    {
        _guilds = [];
    }

    internal ConcurrentGatewayClientCache(JsonGatewayClientCache jsonModel, ulong clientId, RestClient client)
    {
        if (jsonModel.User is { } userModel)
            _user = new CurrentUser(userModel, client);

        _guilds = CreateConcurrentDictionary(jsonModel.Guilds, g => g.Id, g => new Guild(g, clientId, client, this));
    }

    public CurrentUser? User => _user;
    public IReadOnlyDictionary<ulong, Guild> Guilds => _guilds;

#pragma warning disable IDE0032 // Use auto property
    private CurrentUser? _user;
#pragma warning restore IDE0032 // Use auto property
    private readonly ConcurrentDictionary<ulong, Guild> _guilds;

    public JsonGatewayClientCache ToJsonModel()
    {
        JsonGatewayClientCache jsonModel = new();

        var user = _user;
        if (user is not null)
            jsonModel.User = ((IJsonModel<JsonUser>)user).JsonModel;

        jsonModel.Guilds = _guilds.Values.Select(g => ((IJsonModel<JsonGuild>)g).JsonModel).ToArray();

        return jsonModel;
    }

    public IGatewayClientCache CacheGuild(Guild guild)
    {
        _guilds[guild.Id] = guild;

        return this;
    }

    public IGatewayClientCache CacheGuildUser(GuildUser user)
    {
        if (_guilds.TryGetValue(user.GuildId, out var guild))
        {
            var users = Cast(guild.Users);
            users[user.Id] = user;
        }

        return this;
    }

    public IGatewayClientCache CacheGuildUsers(ulong guildId, IReadOnlyList<GuildUser> users)
    {
        if (_guilds.TryGetValue(guildId, out var guild))
        {
            var guildUsers = Cast(guild.Users);
            int count = users.Count;
            for (int i = 0; i < count; i++)
            {
                var user = users[i];
                guildUsers[user.Id] = user;
            }
        }

        return this;
    }

    public IGatewayClientCache CachePresences(ulong guildId, IReadOnlyList<Presence> presences)
    {
        if (_guilds.TryGetValue(guildId, out var guild))
        {
            var guildPresences = Cast(guild.Presences);
            int count = presences.Count;
            for (int i = 0; i < count; i++)
            {
                var presence = presences[i];
                guildPresences[presence.User.Id] = presence;
            }
        }

        return this;
    }

    public IGatewayClientCache CacheRole(Role role)
    {
        if (_guilds.TryGetValue(role.GuildId, out var guild))
        {
            var roles = Cast(guild.Roles);
            roles[role.Id] = role;
        }

        return this;
    }

    public IGatewayClientCache CacheGuildScheduledEvent(GuildScheduledEvent scheduledEvent)
    {
        if (_guilds.TryGetValue(scheduledEvent.GuildId, out var guild))
        {
            var scheduledEvents = Cast(guild.ScheduledEvents);
            scheduledEvents[scheduledEvent.Id] = scheduledEvent;
        }

        return this;
    }

    public IGatewayClientCache CacheGuildThread(GuildThread thread)
    {
        if (_guilds.TryGetValue(thread.GuildId, out var guild))
        {
            var threads = Cast(guild.ActiveThreads);
            threads[thread.Id] = thread;
        }

        return this;
    }

    public IGatewayClientCache CacheGuildChannel(IGuildChannel channel)
    {
        if (_guilds.TryGetValue(channel.GuildId, out var guild))
        {
            var channels = Cast(guild.Channels);
            channels[channel.Id] = channel;
        }

        return this;
    }

    public IGatewayClientCache CacheStageInstance(StageInstance stageInstance)
    {
        if (_guilds.TryGetValue(stageInstance.GuildId, out var guild))
        {
            var stageInstances = Cast(guild.StageInstances);
            stageInstances[stageInstance.Id] = stageInstance;
        }

        return this;
    }

    public IGatewayClientCache CacheCurrentUser(CurrentUser user)
    {
        _user = user;

        return this;
    }

    public IGatewayClientCache CacheVoiceState(VoiceState voiceState)
    {
        if (_guilds.TryGetValue(voiceState.GuildId, out var guild))
        {
            var voiceStates = Cast(guild.VoiceStates);
            voiceStates[voiceState.UserId] = voiceState;
        }

        return this;
    }

    public IGatewayClientCache CachePresence(Presence presence)
    {
        if (_guilds.TryGetValue(presence.GuildId, out var guild))
        {
            var presences = Cast(guild.Presences);
            presences[presence.User.Id] = presence;
        }

        return this;
    }

    public IGatewayClientCache SyncGuildEmojis(ulong guildId, IReadOnlyDictionary<ulong, GuildEmoji> emojis)
    {
        if (_guilds.TryGetValue(guildId, out var guild))
            guild.Emojis = emojis;

        return this;
    }

    public IGatewayClientCache SyncGuildStickers(ulong guildId, IReadOnlyDictionary<ulong, GuildSticker> stickers)
    {
        if (_guilds.TryGetValue(guildId, out var guild))
            guild.Stickers = stickers;

        return this;
    }

    public IGatewayClientCache SyncGuildActiveThreads(ulong guildId, IReadOnlyDictionary<ulong, GuildThread> threads)
    {
        if (_guilds.TryGetValue(guildId, out var guild))
            guild.ActiveThreads = threads;

        return this;
    }

    public IGatewayClientCache SyncGuilds(IReadOnlyList<ulong> guildIds)
    {
        foreach (var guildId in _guilds.Keys.Except(guildIds))
            _guilds.TryRemove(guildId, out _);

        return this;
    }

    public IGatewayClientCache RemoveGuild(ulong guildId)
    {
        _guilds.TryRemove(guildId, out _);

        return this;
    }

    public IGatewayClientCache RemoveGuildUser(ulong guildId, ulong userId)
    {
        if (_guilds.TryGetValue(guildId, out var guild))
        {
            var users = Cast(guild.Users);
            users.TryRemove(userId, out _);
        }

        return this;
    }

    public IGatewayClientCache RemoveRole(ulong guildId, ulong roleId)
    {
        if (_guilds.TryGetValue(guildId, out var guild))
        {
            var roles = Cast(guild.Roles);
            roles.TryRemove(roleId, out _);
        }

        return this;
    }

    public IGatewayClientCache RemoveGuildScheduledEvent(ulong guildId, ulong scheduledEventId)
    {
        if (_guilds.TryGetValue(guildId, out var guild))
        {
            var scheduledEvents = Cast(guild.ScheduledEvents);
            scheduledEvents.TryRemove(scheduledEventId, out _);
        }

        return this;
    }

    public IGatewayClientCache RemoveGuildThread(ulong guildId, ulong threadId)
    {
        if (_guilds.TryGetValue(guildId, out var guild))
        {
            var threads = Cast(guild.ActiveThreads);
            threads.TryRemove(threadId, out _);
        }

        return this;
    }

    public IGatewayClientCache RemoveGuildChannel(ulong guildId, ulong channelId)
    {
        if (_guilds.TryGetValue(guildId, out var guild))
        {
            var channels = Cast(guild.Channels);
            channels.TryRemove(channelId, out _);
        }

        return this;
    }

    public IGatewayClientCache RemoveStageInstance(ulong guildId, ulong stageInstanceId)
    {
        if (_guilds.TryGetValue(guildId, out var guild))
        {
            var stageInstances = Cast(guild.StageInstances);
            stageInstances.TryRemove(stageInstanceId, out _);
        }

        return this;
    }

    public IGatewayClientCache RemoveVoiceState(ulong guildId, ulong userId)
    {
        if (_guilds.TryGetValue(guildId, out var guild))
        {
            var voiceStates = Cast(guild.VoiceStates);
            voiceStates.TryRemove(userId, out _);
        }

        return this;
    }

    public IReadOnlyDictionary<TKey, TValue> CreateDictionary<TSource, TKey, TValue>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
        where TKey : notnull
        where TValue : class
    {
        return CreateConcurrentDictionary(source, keySelector, elementSelector);
    }

    private static ConcurrentDictionary<TKey, TValue> CreateConcurrentDictionary<TSource, TKey, TValue>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector)
        where TKey : notnull
        where TValue : class
    {
        return new(source.Select(s => new KeyValuePair<TKey, TValue>(keySelector(s), elementSelector(s))));
    }

    private static ConcurrentDictionary<TKey, TValue> Cast<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary)
        where TKey : notnull
        where TValue : class
    {
        if (dictionary is ConcurrentDictionary<TKey, TValue> concurrentDictionary)
            return concurrentDictionary;

        ThrowInvalidDictionary();
        return null;
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowInvalidDictionary()
    {
        throw new InvalidOperationException("The dictionary must be a 'System.Collections.Concurrent.ConcurrentDictionary<TKey, TValue>'. It should be created using a 'CreateDictionary<TSource, TKey, TValue>' method.");
    }

    public void Dispose()
    {
    }
}
