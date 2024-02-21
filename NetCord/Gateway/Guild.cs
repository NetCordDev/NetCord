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

    public override bool IsOwner { get; }
    public DateTimeOffset JoinedAt => _jsonModel.JoinedAt;
    public bool IsLarge => _jsonModel.IsLarge;
    public bool IsUnavailable => _jsonModel.IsUnavailable;
    public int UserCount => _jsonModel.UserCount;

    public Guild(JsonGuild jsonModel, ulong clientId, RestClient client) : base(jsonModel, client)
    {
        var guildId = jsonModel.Id;
        VoiceStates = jsonModel.VoiceStates.ToImmutableDictionaryOrEmpty(s => new VoiceState(s, guildId, client));
        Users = jsonModel.Users.ToImmutableDictionaryOrEmpty(u => new GuildUser(u, guildId, client));
        Channels = jsonModel.Channels.ToImmutableDictionaryOrEmpty(c => IGuildChannel.CreateFromJson(c, guildId, client));
        ActiveThreads = jsonModel.ActiveThreads.ToImmutableDictionaryOrEmpty(t => GuildThread.CreateFromJson(t, client));
        StageInstances = jsonModel.StageInstances.ToImmutableDictionaryOrEmpty(i => new StageInstance(i, client));
        Presences = jsonModel.Presences.ToImmutableDictionaryOrEmpty(p => new Presence(p, guildId, client));
        ScheduledEvents = jsonModel.ScheduledEvents.ToImmutableDictionaryOrEmpty(e => new GuildScheduledEvent(e, client));
        IsOwner = jsonModel.OwnerId == clientId;
    }

    public Guild(JsonGuild jsonModel, ulong clientId, Guild oldGuild) : base(Copy(jsonModel, oldGuild), oldGuild._client)
    {
        VoiceStates = oldGuild.VoiceStates;
        Users = oldGuild.Users;
        Channels = oldGuild.Channels;
        ActiveThreads = oldGuild.ActiveThreads;
        StageInstances = oldGuild.StageInstances;
        Presences = oldGuild.Presences;
        ScheduledEvents = oldGuild.ScheduledEvents;
        IsOwner = jsonModel.OwnerId == clientId;
    }

    private static JsonGuild Copy(JsonGuild jsonModel, Guild oldGuild)
    {
        var oldJsonModel = oldGuild._jsonModel;
        jsonModel.JoinedAt = oldJsonModel.JoinedAt;
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
