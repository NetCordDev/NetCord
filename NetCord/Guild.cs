using System.Collections.Immutable;

using NetCord.JsonModels;

namespace NetCord;

public class Guild : RestGuild
{
    public ImmutableDictionary<DiscordId, VoiceState> VoiceStates => _voiceStates;
    public ImmutableDictionary<DiscordId, GuildUser> Users => _users;
    public ImmutableDictionary<DiscordId, IGuildChannel> Channels => _channels;
    public ImmutableDictionary<DiscordId, GuildThread> ActiveThreads => _activeThreads;
    public ImmutableDictionary<DiscordId, StageInstance> StageInstances => _stageInstances;
    public ImmutableDictionary<DiscordId, Presence> Presences => _presences;
    public ImmutableDictionary<DiscordId, GuildScheduledEvent> ScheduledEvents => _scheduledEvents;

    internal ImmutableDictionary<DiscordId, VoiceState> _voiceStates;
    internal ImmutableDictionary<DiscordId, GuildUser> _users;
    internal ImmutableDictionary<DiscordId, IGuildChannel> _channels;
    internal ImmutableDictionary<DiscordId, GuildThread> _activeThreads;
    internal ImmutableDictionary<DiscordId, StageInstance> _stageInstances;
    internal ImmutableDictionary<DiscordId, Presence> _presences;
    internal ImmutableDictionary<DiscordId, GuildScheduledEvent> _scheduledEvents;

    public string? IconHash => _jsonEntity.IconHash;
    public GuildUser Owner => _users[OwnerId];
    public DateTimeOffset CreatedAt => _jsonEntity.CreatedAt;
    public bool IsLarge => _jsonEntity.IsLarge;
    public bool IsUnavaible => _jsonEntity.IsUnavaible;
    public int MemberCount => _jsonEntity.MemberCount;

    internal Guild(JsonGuild jsonEntity, RestClient client) : base(jsonEntity, client)
    {
        _voiceStates = _jsonEntity.VoiceStates.ToImmutableDictionary(s => s.UserId, s => new VoiceState(s));
        _users = _jsonEntity.Users.DistinctBy(u => u.User.Id).ToImmutableDictionary(u => u.User.Id, u => new GuildUser(u, Id, client));
        _channels = _jsonEntity.Channels.ToImmutableDictionary(c => c.Id, c => (IGuildChannel)Channel.CreateFromJson(c, client));
        _activeThreads = _jsonEntity.ActiveThreads.ToImmutableDictionary(t => t.Id, t => (GuildThread)Channel.CreateFromJson(t, client));
        _stageInstances = _jsonEntity.StageInstances.ToImmutableDictionary(i => i.Id, i => new StageInstance(i, client));
        _presences = _jsonEntity.Presences.ToImmutableDictionary(p => p.User.Id, p => new Presence(p, client));
        _scheduledEvents = _jsonEntity.ScheduledEvents.ToImmutableDictionary(e => e.Id, e => new GuildScheduledEvent(e, client));
    }

    internal Guild(JsonGuild jsonEntity, Guild oldGuild) : base(jsonEntity with { CreatedAt = oldGuild.CreatedAt, IsLarge = oldGuild.IsLarge, MemberCount = oldGuild.MemberCount }, oldGuild._client)
    {
        _voiceStates = oldGuild.VoiceStates;
        _users = oldGuild._users;
        _channels = oldGuild._channels;
        _activeThreads = oldGuild._activeThreads;
        _stageInstances = oldGuild._stageInstances;
        _presences = oldGuild._presences;
        _scheduledEvents = oldGuild._scheduledEvents;
    }
}