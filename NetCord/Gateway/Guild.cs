using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class Guild : RestGuild
{
    public ImmutableDictionary<ulong, VoiceState> VoiceStates { get; set; }
    public ImmutableDictionary<ulong, GuildUser> Users { get; set; }
    public ImmutableDictionary<ulong, IGuildChannel> Channels { get; set; }
    public ImmutableDictionary<ulong, GuildThread> ActiveThreads { get; set; }
    public ImmutableDictionary<ulong, StageInstance> StageInstances { get; set; }
    public ImmutableDictionary<ulong, Presence> Presences { get; set; }
    public ImmutableDictionary<ulong, GuildScheduledEvent> ScheduledEvents { get; set; }

    public DateTimeOffset CreatedAt => _jsonModel.CreatedAt;
    public bool IsLarge => _jsonModel.IsLarge;
    public bool IsUnavailable => _jsonModel.IsUnavailable;
    public int UserCount => _jsonModel.UserCount;

    public Guild(JsonGuild jsonModel, RestClient client) : base(jsonModel, client)
    {
        VoiceStates = _jsonModel.VoiceStates.ToImmutableDictionaryOrEmpty(s => new VoiceState(s, client));
        Users = _jsonModel.Users.ToImmutableDictionaryOrEmpty(u => new GuildUser(u, Id, client));
        Channels = _jsonModel.Channels.ToImmutableDictionaryOrEmpty(c => (IGuildChannel)Channel.CreateFromJson(c, client));
        ActiveThreads = _jsonModel.ActiveThreads.ToImmutableDictionaryOrEmpty(t => (GuildThread)Channel.CreateFromJson(t, client));
        StageInstances = _jsonModel.StageInstances.ToImmutableDictionaryOrEmpty(i => new StageInstance(i, client));
        Presences = _jsonModel.Presences.ToImmutableDictionaryOrEmpty(p => new Presence(p, Id, client));
        ScheduledEvents = _jsonModel.ScheduledEvents.ToImmutableDictionaryOrEmpty(e => new GuildScheduledEvent(e, client));
    }

    public Guild(JsonGuild jsonModel, Guild oldGuild) : base(Copy(jsonModel, oldGuild), oldGuild._client)
    {
        VoiceStates = oldGuild.VoiceStates;
        Users = oldGuild.Users;
        Channels = oldGuild.Channels;
        ActiveThreads = oldGuild.ActiveThreads;
        StageInstances = oldGuild.StageInstances;
        Presences = oldGuild.Presences;
        ScheduledEvents = oldGuild.ScheduledEvents;
    }

    private static JsonGuild Copy(JsonGuild jsonModel, Guild oldGuild)
    {
        var oldJsonModel = oldGuild._jsonModel;
        jsonModel.CreatedAt = oldJsonModel.CreatedAt;
        jsonModel.IsLarge = oldJsonModel.IsLarge;
        jsonModel.UserCount = oldJsonModel.UserCount;
        jsonModel.VoiceStates = oldJsonModel.VoiceStates;
        jsonModel.Users = oldJsonModel.Users;
        jsonModel.Channels = oldJsonModel.Channels;
        jsonModel.ActiveThreads = oldJsonModel.ActiveThreads;
        jsonModel.Presences = oldJsonModel.Presences;
        jsonModel.StageInstances = oldJsonModel.StageInstances;
        jsonModel.ScheduledEvents = oldJsonModel.ScheduledEvents;
        return jsonModel;
    }

    public Guild With(Action<Guild> action)
    {
        var cloned = Unsafe.As<Guild>(MemberwiseClone());
        action(cloned);
        return cloned;
    }
}
