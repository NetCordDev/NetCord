using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

using NetCord.Gateway.JsonModels;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public sealed class ImmutableGatewayClientCache : IGatewayClientCache
{
    public ImmutableGatewayClientCache()
    {
        _guilds = CollectionsUtils.CreateImmutableDictionary<ulong, Guild>();
    }

    public ImmutableGatewayClientCache(JsonGatewayClientCache jsonModel, ulong clientId, RestClient client)
    {
        var userModel = jsonModel.User;
        if (userModel is not null)
            _user = new(userModel, client);

        _guilds = jsonModel.Guilds.ToImmutableDictionary(g => g.Id, g => new Guild(g, clientId, client, this));
    }

    private ImmutableGatewayClientCache(CurrentUser? user, ImmutableDictionary<ulong, Guild> guilds)
    {
        _user = user;
        _guilds = guilds;
    }

    private static ImmutableGatewayClientCache Create(CurrentUser? user, ImmutableDictionary<ulong, Guild> guilds)
    {
        return new(user, guilds);
    }

    public CurrentUser? User => _user;
    public IReadOnlyDictionary<ulong, Guild> Guilds => _guilds;

#pragma warning disable IDE0032 // Use auto property
    private CurrentUser? _user;
#pragma warning restore IDE0032 // Use auto property
    private ImmutableDictionary<ulong, Guild> _guilds;

    public JsonGatewayClientCache ToJsonModel()
    {
        return new()
        {
            User = _user is null ? null : ((IJsonModel<JsonUser>)_user).JsonModel,
            Guilds = _guilds.Values.Select(g => ((IJsonModel<JsonGuild>)g).JsonModel).ToArray(),
        };
    }

    public IGatewayClientCache CacheGuild(Guild guild)
    {
        return Create(_user,
                      _guilds.SetItem(guild.Id, guild));
    }

    public IGatewayClientCache CacheGuildUser(GuildUser user)
    {
        var guildId = user.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.Users = Cast(guild.Users).SetItem(user.Id, user);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache CacheGuildUsers(ulong guildId, IReadOnlyList<GuildUser> users)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.Users = Cast(guild.Users).SetItems(users.Select(u => new KeyValuePair<ulong, GuildUser>(u.Id, u)));

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache CachePresences(ulong guildId, IReadOnlyList<Presence> presences)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.Presences = Cast(guild.Presences).SetItems(presences.Select(p => new KeyValuePair<ulong, Presence>(p.User.Id, p)));

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache CacheRole(Role role)
    {
        var guildId = role.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.Roles = Cast(guild.Roles).SetItem(role.Id, role);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache CacheGuildScheduledEvent(GuildScheduledEvent scheduledEvent)
    {
        var guildId = scheduledEvent.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.ScheduledEvents = Cast(guild.ScheduledEvents).SetItem(scheduledEvent.Id, scheduledEvent);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache CacheGuildThread(GuildThread thread)
    {
        var guildId = thread.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.ActiveThreads = Cast(guild.ActiveThreads).SetItem(thread.Id, thread);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache CacheGuildChannel(IGuildChannel channel)
    {
        var guildId = channel.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.Channels = Cast(guild.Channels).SetItem(channel.Id, channel);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache CacheStageInstance(StageInstance stageInstance)
    {
        var guildId = stageInstance.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.StageInstances = Cast(guild.StageInstances).SetItem(stageInstance.Id, stageInstance);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache CacheCurrentUser(CurrentUser user)
    {
        return Create(user,
                      _guilds);
    }

    public IGatewayClientCache CacheVoiceState(VoiceState voiceState)
    {
        var guildId = voiceState.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.VoiceStates = Cast(guild.VoiceStates).SetItem(voiceState.UserId, voiceState);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache CachePresence(Presence presence)
    {
        var guildId = presence.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.Presences = Cast(guild.Presences).SetItem(presence.User.Id, presence);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache SyncGuildEmojis(ulong guildId, IReadOnlyDictionary<ulong, GuildEmoji> emojis)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.Emojis = emojis;

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache SyncGuildStickers(ulong guildId, IReadOnlyDictionary<ulong, GuildSticker> stickers)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.Stickers = stickers;

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache SyncGuildActiveThreads(ulong guildId, IReadOnlyDictionary<ulong, GuildThread> threads)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.ActiveThreads = threads;

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache SyncGuilds(IReadOnlyList<ulong> guildIds)
    {
        var guilds = _guilds;
        return Create(_user,
                      guilds.RemoveRange(guilds.Keys.Except(guildIds)));
    }

    public IGatewayClientCache RemoveGuild(ulong guildId)
    {
        return Create(_user,
                      _guilds.Remove(guildId));
    }

    public IGatewayClientCache RemoveGuildUser(ulong guildId, ulong userId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.Users = Cast(guild.Users).Remove(userId);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache RemoveRole(ulong guildId, ulong roleId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.Roles = Cast(guild.Roles).Remove(roleId);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache RemoveGuildScheduledEvent(ulong guildId, ulong scheduledEventId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.ScheduledEvents = Cast(guild.ScheduledEvents).Remove(scheduledEventId);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache RemoveGuildThread(ulong guildId, ulong threadId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.ActiveThreads = Cast(guild.ActiveThreads).Remove(threadId);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache RemoveGuildChannel(ulong guildId, ulong channelId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.Channels = Cast(guild.Channels).Remove(channelId);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache RemoveStageInstance(ulong guildId, ulong stageInstanceId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.StageInstances = Cast(guild.StageInstances).Remove(stageInstanceId);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IGatewayClientCache RemoveVoiceState(ulong guildId, ulong userId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            var newGuild = guild.Clone();
            newGuild.VoiceStates = Cast(guild.VoiceStates).Remove(userId);

            return Create(_user,
                          guilds.SetItem(guildId, newGuild));
        }

        return this;
    }

    public IReadOnlyDictionary<TKey, TValue> CreateDictionary<TSource, TKey, TValue>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector) where TKey : notnull where TValue : class
    {
        return source.ToImmutableDictionary(keySelector, elementSelector);
    }

    private static ImmutableDictionary<TKey, TValue> Cast<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> dictionary) where TKey : notnull where TValue : class
    {
        if (dictionary is ImmutableDictionary<TKey, TValue> immutableDictionary)
            return immutableDictionary;

        ThrowInvalidDictionary();
        return null;
    }

    [DoesNotReturn]
    private static void ThrowInvalidDictionary()
    {
        throw new InvalidOperationException("The dictionary must be a 'System.Collections.Immutable.ImmutableDictionary<TKey, TValue>'. It should be created using a 'CreateDictionary<TSource, TKey, TValue>' method.");
    }

    public void Dispose()
    {
    }
}
