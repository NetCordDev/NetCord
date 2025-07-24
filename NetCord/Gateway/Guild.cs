using System.Runtime.CompilerServices;

using NetCord.Gateway.JsonModels;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// <inheritdoc/> Contains additional information about the guild's current state.
/// </summary>
public class Guild : RestGuild, ICloneable
{
    public Guild(JsonGuild jsonModel, ulong clientId, RestClient client, IDictionaryProvider dictionaryProvider) : base(jsonModel, client, dictionaryProvider)
    {
        var guildId = jsonModel.Id;

        VoiceStates = dictionaryProvider.CreateDictionary(jsonModel.VoiceStates ?? [], s => s.UserId, s => new VoiceState(s, guildId, client));
        Users = dictionaryProvider.CreateDictionary(jsonModel.Users ?? [], u => u.User.Id, u => new GuildUser(u, guildId, client));
        Channels = dictionaryProvider.CreateDictionary(jsonModel.Channels ?? [], c => c.Id, c => IGuildChannel.CreateFromJson(c, guildId, client));
        ActiveThreads = dictionaryProvider.CreateDictionary(jsonModel.ActiveThreads ?? [], t => t.Id, t => GuildThread.CreateFromJson(t, client));
        StageInstances = dictionaryProvider.CreateDictionary(jsonModel.StageInstances ?? [], i => i.Id, i => new StageInstance(i, client));
        Presences = dictionaryProvider.CreateDictionary(jsonModel.Presences ?? [], p => p.User.Id, p => new Presence(p, guildId, client));
        ScheduledEvents = dictionaryProvider.CreateDictionary(jsonModel.ScheduledEvents ?? [], e => e.Id, e => new GuildScheduledEvent(e, client));

        IsOwner = jsonModel.OwnerId == clientId;
    }

    public Guild(JsonGuild jsonModel, ulong clientId, Guild oldGuild, IDictionaryProvider dictionaryProvider) : base(Copy(jsonModel, oldGuild), oldGuild._client, dictionaryProvider)
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

    private protected override void UpdateJsonModel(JsonGuild jsonModel)
    {
        // From RestGuild
        jsonModel.Roles = [.. Roles.Values.Select(r => ((IJsonModel<JsonRole>)r).JsonModel)];
        jsonModel.Emojis = [.. Emojis.Values.Select(e => ((IJsonModel<JsonEmoji>)e).JsonModel)];
        jsonModel.Stickers = [.. Stickers.Values.Select(s => ((IJsonModel<JsonSticker>)s).JsonModel)];

        // From Guild
        jsonModel.VoiceStates = [.. VoiceStates.Values.Select(s => ((IJsonModel<JsonVoiceState>)s).JsonModel)];
        jsonModel.Users = [.. Users.Values.Select(u => ((IJsonModel<JsonGuildUser>)u).JsonModel)];
        jsonModel.Channels = [.. Channels.Values.Select(c => ((IJsonModel<JsonChannel>)c).JsonModel)];
        jsonModel.ActiveThreads = [.. ActiveThreads.Values.Select(t => ((IJsonModel<JsonChannel>)t).JsonModel)];
        jsonModel.StageInstances = [.. StageInstances.Values.Select(i => ((IJsonModel<JsonStageInstance>)i).JsonModel)];
        jsonModel.Presences = [.. Presences.Values.Select(p => ((IJsonModel<JsonPresence>)p).JsonModel)];
        jsonModel.ScheduledEvents = [.. ScheduledEvents.Values.Select(e => ((IJsonModel<JsonGuildScheduledEvent>)e).JsonModel)];
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

    object ICloneable.Clone() => MemberwiseClone();

    internal Guild Clone() => Unsafe.As<Guild>(MemberwiseClone());

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
    public IReadOnlyDictionary<ulong, VoiceState> VoiceStates { get; set; }

    /// <summary>
    /// A dictionary of <see cref="GuildUser"/> objects, representing users in the <see cref="Guild"/>.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildUser> Users { get; set; }

    /// <summary>
    /// A dictionary of <see cref="IGuildChannel"/> objects, representing channels present in the <see cref="Guild"/>.
    /// </summary>
    public IReadOnlyDictionary<ulong, IGuildChannel> Channels { get; set; }

    /// <summary>
    /// An array of <see cref="GuildThread"/> objects, representing all active threads in the <see cref="Guild"/> that current user has permission to view.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildThread> ActiveThreads { get; set; }

    /// <summary>
    /// A dictionary of <see cref="Presence"/> objects, will only include offline users if <see cref="IsLarge"/> is <see langword="true"/>.
    /// </summary>
    public IReadOnlyDictionary<ulong, Presence> Presences { get; set; }

    /// <summary>
    /// A dictionary of <see cref="StageInstance"/> objects, representing active stage instances in the <see cref="Guild"/>.
    /// </summary>
    public IReadOnlyDictionary<ulong, StageInstance> StageInstances { get; set; }

    /// <summary>
    /// A dictionary of <see cref="GuildScheduledEvent"/> objects, representing currently scheduled events in the <see cref="Guild"/>.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildScheduledEvent> ScheduledEvents { get; set; }

    /// <inheritdoc/>
    public override bool IsOwner { get; }
}
