using System.Collections.Immutable;

using NetCord.JsonModels;

namespace NetCord;

public class Guild : RestGuild
{
    public ImmutableDictionary<Snowflake, VoiceState> VoiceStates { get; internal set; }
    public ImmutableDictionary<Snowflake, GuildUser> Users { get; internal set; }
    public ImmutableDictionary<Snowflake, IGuildChannel> Channels { get; internal set; }
    public ImmutableDictionary<Snowflake, GuildThread> ActiveThreads { get; internal set; }
    public ImmutableDictionary<Snowflake, StageInstance> StageInstances { get; internal set; }
    public ImmutableDictionary<Snowflake, Presence> Presences { get; internal set; }
    public ImmutableDictionary<Snowflake, GuildScheduledEvent> ScheduledEvents { get; internal set; }

    public string? IconHash => _jsonEntity.IconHash;
    public GuildUser Owner => Users[OwnerId];
    public DateTimeOffset CreatedAt => _jsonEntity.CreatedAt;
    public bool IsLarge => _jsonEntity.IsLarge;
    public bool IsUnavaible => _jsonEntity.IsUnavaible;
    public int MemberCount => _jsonEntity.MemberCount;

    internal Guild(JsonGuild jsonEntity, RestClient client) : base(jsonEntity, client)
    {
        VoiceStates = _jsonEntity.VoiceStates.ToImmutableDictionary(s => s.UserId, s => new VoiceState(s));
        Users = _jsonEntity.Users.DistinctBy(u => u.User.Id).ToImmutableDictionary(u => u.User.Id, u => new GuildUser(u, Id, client));
        Channels = _jsonEntity.Channels.ToImmutableDictionary(c => c.Id, c => (IGuildChannel)Channel.CreateFromJson(c, client));
        ActiveThreads = _jsonEntity.ActiveThreads.ToImmutableDictionary(t => t.Id, t => (GuildThread)Channel.CreateFromJson(t, client));
        StageInstances = _jsonEntity.StageInstances.ToImmutableDictionary(i => i.Id, i => new StageInstance(i, client));
        Presences = _jsonEntity.Presences.ToImmutableDictionary(p => p.User.Id, p => new Presence(p, client));
        ScheduledEvents = _jsonEntity.ScheduledEvents.ToImmutableDictionary(e => e.Id, e => new GuildScheduledEvent(e, client));
    }

    internal Guild(JsonGuild jsonEntity, Guild oldGuild) : base(jsonEntity with { CreatedAt = oldGuild.CreatedAt, IsLarge = oldGuild.IsLarge, MemberCount = oldGuild.MemberCount }, oldGuild._client)
    {
        VoiceStates = oldGuild.VoiceStates;
        Users = oldGuild.Users;
        Channels = oldGuild.Channels;
        ActiveThreads = oldGuild.ActiveThreads;
        StageInstances = oldGuild.StageInstances;
        Presences = oldGuild.Presences;
        ScheduledEvents = oldGuild.ScheduledEvents;
    }
}