using System.Collections.Immutable;

using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

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
    public int UserCount => _jsonModel.UserCount;

    public Guild(JsonGuild jsonModel, RestClient client) : base(jsonModel, client)
    {
        VoiceStates = _jsonModel.VoiceStates.ToImmutableDictionary(s => new VoiceState(s));
        Users = _jsonModel.Users.ToImmutableDictionary(u => new GuildUser(u, Id, client));
        Channels = _jsonModel.Channels.ToImmutableDictionary(c => (IGuildChannel)Channel.CreateFromJson(c, client));
        ActiveThreads = _jsonModel.ActiveThreads.ToImmutableDictionary(t => (GuildThread)Channel.CreateFromJson(t, client));
        StageInstances = _jsonModel.StageInstances.ToImmutableDictionary(i => new StageInstance(i, client));
        Presences = _jsonModel.Presences.ToImmutableDictionary(p => new Presence(p, Id, client));
        ScheduledEvents = _jsonModel.ScheduledEvents.ToImmutableDictionary(e => new GuildScheduledEvent(e, client));
    }

    public Guild(JsonGuild jsonModel, Guild oldGuild) : base(jsonModel with
    {
        CreatedAt = oldGuild._jsonModel.CreatedAt,
        IsLarge = oldGuild._jsonModel.IsLarge,
        UserCount = oldGuild._jsonModel.UserCount,
        VoiceStates = oldGuild._jsonModel.VoiceStates,
        Users = oldGuild._jsonModel.Users,
        Channels = oldGuild._jsonModel.Channels,
        ActiveThreads = oldGuild._jsonModel.ActiveThreads,
        StageInstances = oldGuild._jsonModel.StageInstances,
        Presences = oldGuild._jsonModel.Presences,
        ScheduledEvents = oldGuild._jsonModel.ScheduledEvents
    }, oldGuild._client)
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
