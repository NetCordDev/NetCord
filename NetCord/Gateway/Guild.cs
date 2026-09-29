using System.Runtime.CompilerServices;

using NetCord.Gateway.JsonModels;
using NetCord.Rest;
using NetCord.Rest.JsonModels;

namespace NetCord.Gateway;

/// <summary>
/// <inheritdoc/> Contains additional information about the guild's current state.
/// </summary>
public class Guild : RestGuild, ICloneable
{
    public Guild(JsonGuild jsonModel, RestClient client, IDictionaryProvider dictionaryProvider) : base(jsonModel, client, dictionaryProvider)
    {
        JoinedAt = jsonModel.JoinedAt;
        IsLarge = jsonModel.IsLarge;
        IsUnavailable = jsonModel.IsUnavailable;
        UserCount = jsonModel.UserCount;

        VoiceStates = CreateVoiceStates(jsonModel, client, dictionaryProvider);
        Users = CreateUsers(jsonModel, client, dictionaryProvider);
        Channels = CreateChannels(jsonModel, client, dictionaryProvider);
        ActiveThreads = dictionaryProvider.CreateDictionary(jsonModel.ActiveThreads, t => t.Id, t => GuildThread.CreateFromJson(t, client));
        Presences = CreatePresences(jsonModel, client, dictionaryProvider);
        StageInstances = dictionaryProvider.CreateDictionary(jsonModel.StageInstances, s => s.Id, s => new StageInstance(s, client));
        ScheduledEvents = dictionaryProvider.CreateDictionary(jsonModel.ScheduledEvents, e => e.Id, e => new GuildScheduledEvent(e, client));
    }

    public Guild(JsonRestGuild jsonModel, Guild oldGuild, IDictionaryProvider dictionaryProvider) : base(jsonModel, oldGuild._client, dictionaryProvider)
    {
        JoinedAt = oldGuild.JoinedAt;
        IsLarge = oldGuild.IsLarge;
        IsUnavailable = oldGuild.IsUnavailable;
        UserCount = oldGuild.UserCount;

        VoiceStates = oldGuild.VoiceStates;
        Users = oldGuild.Users;
        Channels = oldGuild.Channels;
        ActiveThreads = oldGuild.ActiveThreads;
        Presences = oldGuild.Presences;
        StageInstances = oldGuild.StageInstances;
        ScheduledEvents = oldGuild.ScheduledEvents;
    }

    object ICloneable.Clone() => MemberwiseClone();

    internal Guild Clone() => Unsafe.As<Guild>(MemberwiseClone());

    /// <summary>
    /// When the current user joined the <see cref="Guild"/>.
    /// </summary>
    public DateTimeOffset JoinedAt { get; }

    /// <summary>
    /// Whether the <see cref="Guild"/>'s member count is over the <c>large</c> threshold.
    /// </summary>
    public bool IsLarge { get; }

    /// <summary>
    /// Whether the <see cref="Guild"/> is unavailable due to an outage.
    /// </summary>
    public bool? IsUnavailable { get; }

    /// <summary>
    /// The total number of <see cref="GuildUser"/>s in the <see cref="Guild"/>.
    /// </summary>
    public int UserCount { get; }

    /// <summary>
    /// A dictionary of <see cref="VoiceState"/> objects, representing the states of <see cref="GuildUser"/>s currently in voice channels.
    /// </summary>
    public IReadOnlyDictionary<ulong, VoiceState> VoiceStates { get; set; }

    private static IReadOnlyDictionary<ulong, VoiceState> CreateVoiceStates(JsonGuild jsonModel, RestClient client, IDictionaryProvider dictionaryProvider)
    {
        var guildId = jsonModel.Id;
        return dictionaryProvider.CreateDictionary(jsonModel.VoiceStates, s => s.UserId, s => new VoiceState(s, guildId, client));
    }

    /// <summary>
    /// A dictionary of <see cref="GuildUser"/> objects, representing users in the <see cref="Guild"/>.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildUser> Users { get; set; }

    private static IReadOnlyDictionary<ulong, GuildUser> CreateUsers(JsonGuild jsonModel, RestClient client, IDictionaryProvider dictionaryProvider)
    {
        var guildId = jsonModel.Id;
        return dictionaryProvider.CreateDictionary(jsonModel.Users.DistinctBy(u => u.User.Id),
                                                   u => u.User.Id,
                                                   u => new GuildUser(u, guildId, client));
    }

    /// <summary>
    /// A dictionary of <see cref="IGuildChannel"/> objects, representing channels present in the <see cref="Guild"/>.
    /// </summary>
    public IReadOnlyDictionary<ulong, IGuildChannel> Channels { get; set; }

    private static IReadOnlyDictionary<ulong, IGuildChannel> CreateChannels(JsonGuild jsonModel, RestClient client, IDictionaryProvider dictionaryProvider)
    {
        var guildId = jsonModel.Id;
        return dictionaryProvider.CreateDictionary(jsonModel.Channels, c => c.Id, c => IGuildChannel.CreateFromJson(c, guildId, client));
    }

    /// <summary>
    /// An array of <see cref="GuildThread"/> objects, representing all active threads in the <see cref="Guild"/> that current user has permission to view.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildThread> ActiveThreads { get; set; }

    /// <summary>
    /// A dictionary of <see cref="Presence"/> objects, will only include offline users if <see cref="IsLarge"/> is <see langword="true"/>.
    /// </summary>
    public IReadOnlyDictionary<ulong, Presence> Presences { get; set; }

    private static IReadOnlyDictionary<ulong, Presence> CreatePresences(JsonGuild jsonModel, RestClient client, IDictionaryProvider dictionaryProvider)
    {
        var guildId = jsonModel.Id;
        return dictionaryProvider.CreateDictionary(jsonModel.Presences, p => p.User.Id, p => new Presence(p, guildId, client));
    }

    /// <summary>
    /// A dictionary of <see cref="StageInstance"/> objects, representing active stage instances in the <see cref="Guild"/>.
    /// </summary>
    public IReadOnlyDictionary<ulong, StageInstance> StageInstances { get; set; }

    /// <summary>
    /// A dictionary of <see cref="GuildScheduledEvent"/> objects, representing currently scheduled events in the <see cref="Guild"/>.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildScheduledEvent> ScheduledEvents { get; set; }
}
