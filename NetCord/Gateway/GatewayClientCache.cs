using System.Collections.Immutable;

using NetCord.Gateway.JsonModels;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public sealed record GatewayClientCache : IGatewayClientCache
{
    public GatewayClientCache()
    {
        _guilds = CollectionsUtils.CreateImmutableDictionary<ulong, Guild>();
    }

    public GatewayClientCache(JsonGatewayClientCache jsonModel, ulong clientId, RestClient client)
    {
        var userModel = jsonModel.User;
        if (userModel is not null)
            _user = new(userModel, client);
        _guilds = jsonModel.Guilds.ToImmutableDictionary(g => g.Id, g => new Guild(g, clientId, client));
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
            Guilds = _guilds.Select(p => ((IJsonModel<JsonGuild>)p.Value).JsonModel).ToArray(),
        };
    }

    public IGatewayClientCache CacheGuild(Guild guild)
    {
        return this with
        {
            _guilds = _guilds.SetItem(guild.Id, guild),
        };
    }

    public IGatewayClientCache CacheGuildUser(GuildUser user)
    {
        var guildId = user.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.Users = g.Users.SetItem(user.Id, user))),
            };
        }

        return this;
    }

    public IGatewayClientCache CacheGuildUsers(ulong guildId, IEnumerable<GuildUser> users)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.Users = g.Users.SetItems(users.Select(u => new KeyValuePair<ulong, GuildUser>(u.Id, u))))),
            };
        }

        return this;
    }

    public IGatewayClientCache CachePresences(ulong guildId, IEnumerable<Presence> presences)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.Presences = g.Presences.SetItems(presences.Select(p => new KeyValuePair<ulong, Presence>(p.User.Id, p))))),
            };
        }

        return this;
    }

    public IGatewayClientCache CacheRole(Role role)
    {
        var guildId = role.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.Roles = g.Roles.SetItem(role.Id, role))),
            };
        }

        return this;
    }

    public IGatewayClientCache CacheGuildScheduledEvent(GuildScheduledEvent scheduledEvent)
    {
        var guildId = scheduledEvent.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.ScheduledEvents = g.ScheduledEvents.SetItem(scheduledEvent.Id, scheduledEvent))),
            };
        }

        return this;
    }

    public IGatewayClientCache CacheGuildEmojis(ulong guildId, ImmutableDictionary<ulong, GuildEmoji> emojis)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.Emojis = emojis)),
            };
        }

        return this;
    }

    public IGatewayClientCache CacheGuildStickers(ulong guildId, ImmutableDictionary<ulong, GuildSticker> stickers)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.Stickers = stickers)),
            };
        }

        return this;
    }

    public IGatewayClientCache CacheGuildThread(GuildThread thread)
    {
        var guildId = thread.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.ActiveThreads = g.ActiveThreads.SetItem(thread.Id, thread))),
            };
        }

        return this;
    }

    public IGatewayClientCache CacheGuildChannel(IGuildChannel channel)
    {
        var guildId = channel.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.Channels = g.Channels.SetItem(channel.Id, channel))),
            };
        }

        return this;
    }

    public IGatewayClientCache CacheStageInstance(StageInstance stageInstance)
    {
        var guildId = stageInstance.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.StageInstances = g.StageInstances.SetItem(stageInstance.Id, stageInstance))),
            };
        }

        return this;
    }

    public IGatewayClientCache CacheCurrentUser(CurrentUser user)
    {
        return this with
        {
            _user = user,
        };
    }

    public IGatewayClientCache CacheVoiceState(VoiceState voiceState)
    {
        var guildId = voiceState.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.VoiceStates = g.VoiceStates.SetItem(voiceState.UserId, voiceState))),
            };
        }

        return this;
    }

    public IGatewayClientCache CachePresence(Presence presence)
    {
        var guildId = presence.GuildId;
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.Presences = g.Presences.SetItem(presence.User.Id, presence))),
            };
        }

        return this;
    }

    public IGatewayClientCache SyncGuildActiveThreads(ulong guildId, ImmutableDictionary<ulong, GuildThread> threads)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.ActiveThreads = threads)),
            };
        }

        return this;
    }

    public IGatewayClientCache SyncGuilds(IReadOnlyList<ulong> guildIds)
    {
        var guilds = _guilds;
        return this with
        {
            _guilds = guilds.RemoveRange(guilds.Keys.Except(guildIds)),
        };
    }

    public IGatewayClientCache RemoveGuild(ulong guildId)
    {
        return this with
        {
            _guilds = _guilds.Remove(guildId),
        };
    }

    public IGatewayClientCache RemoveGuildUser(ulong guildId, ulong userId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.Users = g.Users.Remove(userId))),
            };
        }

        return this;
    }

    public IGatewayClientCache RemoveRole(ulong guildId, ulong roleId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.Roles = g.Roles.Remove(roleId))),
            };
        }

        return this;
    }

    public IGatewayClientCache RemoveGuildScheduledEvent(ulong guildId, ulong scheduledEventId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.ScheduledEvents = g.ScheduledEvents.Remove(scheduledEventId))),
            };
        }

        return this;
    }

    public IGatewayClientCache RemoveGuildThread(ulong guildId, ulong threadId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.ActiveThreads = g.ActiveThreads.Remove(threadId))),
            };
        }

        return this;
    }

    public IGatewayClientCache RemoveGuildChannel(ulong guildId, ulong channelId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.Channels = g.Channels.Remove(channelId))),
            };
        }

        return this;
    }

    public IGatewayClientCache RemoveStageInstance(ulong guildId, ulong stageInstanceId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.StageInstances = g.StageInstances.Remove(stageInstanceId))),
            };
        }

        return this;
    }

    public IGatewayClientCache RemoveVoiceState(ulong guildId, ulong userId)
    {
        var guilds = _guilds;
        if (guilds.TryGetValue(guildId, out var guild))
        {
            return this with
            {
                _guilds = guilds.SetItem(guildId, guild.With(g => g.VoiceStates = g.VoiceStates.Remove(userId))),
            };
        }

        return this;
    }

    public void Dispose()
    {
    }
}
