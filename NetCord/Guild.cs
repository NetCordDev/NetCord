using System.Collections.Immutable;

using NetCord.JsonModels;
using NetCord.Rest;

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

    public string? IconHash => _jsonModel.IconHash;
    public GuildUser Owner => Users[OwnerId];
    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;
    public bool IsLarge => _jsonModel.IsLarge;
    public bool IsUnavaible => _jsonModel.IsUnavaible;
    public int MemberCount => _jsonModel.MemberCount;

    public Guild(JsonGuild jsonModel, RestClient client) : base(jsonModel, client)
    {
        VoiceStates = _jsonModel.VoiceStates.ToImmutableDictionary(s => s.UserId, s => new VoiceState(s));
        Users = _jsonModel.Users.DistinctBy(u => u.User.Id).ToImmutableDictionary(u => u.User.Id, u => new GuildUser(u, Id, client));
        Channels = _jsonModel.Channels.ToImmutableDictionary(c => c.Id, c => (IGuildChannel)Channel.CreateFromJson(c, client));
        ActiveThreads = _jsonModel.ActiveThreads.ToImmutableDictionary(t => t.Id, t => (GuildThread)Channel.CreateFromJson(t, client));
        StageInstances = _jsonModel.StageInstances.ToImmutableDictionary(i => i.Id, i => new StageInstance(i, client));
        Presences = _jsonModel.Presences.ToImmutableDictionary(p => p.User.Id, p => new Presence(p, client));
        ScheduledEvents = _jsonModel.ScheduledEvents.ToImmutableDictionary(e => e.Id, e => new GuildScheduledEvent(e, client));
    }

    public Guild(JsonGuild jsonModel, Guild oldGuild) : base(jsonModel with { CreatedAt = oldGuild.CreatedAt, IsLarge = oldGuild.IsLarge, MemberCount = oldGuild.MemberCount }, oldGuild._client)
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