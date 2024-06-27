using System.Collections.Immutable;
using System.Runtime.CompilerServices;

using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// <inheritdoc/> Contains additional information about the guild's current state.
/// </summary>
public class Guild : RestGuild
{
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

    /// <summary>
    /// When the current user joined the <see cref="Guild"/>.
    /// </summary>
    public DateTimeOffset JoinedAt => _jsonModel.JoinedAt;

    /// <summary>
    /// Whether the <see cref="Guild"/>'s member count is over the <c>large</c> threshold.
    /// </summary>
    public bool IsLarge => _jsonModel.IsLarge;

    /// <summary>
    /// Whether the <see cref="Guild"/> is unavailable due to an outage.
    /// </summary>
    public bool IsUnavailable => _jsonModel.IsUnavailable;

    /// <summary>
    /// The total number of <see cref="GuildUser"/>s in the <see cref="Guild"/>.
    /// </summary>
    public int UserCount => _jsonModel.UserCount;

    /// <summary>
    /// A dictionary of <see cref="VoiceState"/> objects, representing the states of <see cref="GuildUser"/>s currently in voice channels.
    /// </summary>
    public ImmutableDictionary<ulong, VoiceState> VoiceStates { get; set; }

    /// <summary>
    /// A dictionary of <see cref="GuildUser"/> objects, representing users in the <see cref="Guild"/>.
    /// </summary>
    public ImmutableDictionary<ulong, GuildUser> Users { get; set; }

    /// <summary>
    /// A dictionary of <see cref="IGuildChannel"/> objects, representing channels present in the <see cref="Guild"/>.
    /// </summary>
    public ImmutableDictionary<ulong, IGuildChannel> Channels { get; set; }

    /// <summary>
    /// An array of <see cref="GuildThread"/> objects, representing all active threads in the <see cref="Guild"/> that current user has permission to view.
    /// </summary>
    public ImmutableDictionary<ulong, GuildThread> ActiveThreads { get; set; }

    /// <summary>
    /// A dictionary of <see cref="Presence"/> objects, will only include offline users if <see cref="IsLarge"/> is <see langword="true"/>.
    /// </summary>
    public ImmutableDictionary<ulong, Presence> Presences { get; set; }

    /// <summary>
    /// A dictionary of <see cref="StageInstance"/> objects, representing active stage instances in the <see cref="Guild"/>.
    /// </summary>
    public ImmutableDictionary<ulong, StageInstance> StageInstances { get; set; }

    /// <summary>
    /// A dictionary of <see cref="GuildScheduledEvent"/> objects, representing currently scheduled events in the <see cref="Guild"/>.
    /// </summary>
    public ImmutableDictionary<ulong, GuildScheduledEvent> ScheduledEvents { get; set; }

    /// <inheritdoc/>
    public override bool IsOwner { get; }
}
