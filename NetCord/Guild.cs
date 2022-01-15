using System.Collections.Immutable;

using NetCord.JsonModels;

namespace NetCord;

public class Guild : ClientEntity
{
    internal readonly JsonGuild _jsonEntity;

    public ImmutableDictionary<DiscordId, VoiceState> VoiceStates => _voiceStates;
    public ImmutableDictionary<DiscordId, GuildUser> Users => _users;
    public ImmutableDictionary<DiscordId, IGuildChannel> Channels => _channels;
    public ImmutableDictionary<DiscordId, Thread> ActiveThreads => _activeThreads;
    public ImmutableDictionary<DiscordId, GuildRole> Roles => _roles;
    public ImmutableDictionary<DiscordId, Emoji> Emojis => _emojis;
    public ImmutableDictionary<DiscordId, StageInstance> StageInstances => _stageInstances;
    public ImmutableDictionary<DiscordId, Presence> Presences => _presences;
    public ImmutableDictionary<DiscordId, GuildSticker> Stickers => _stickers;

    internal ImmutableDictionary<DiscordId, VoiceState> _voiceStates;
    internal ImmutableDictionary<DiscordId, GuildUser> _users;
    internal ImmutableDictionary<DiscordId, IGuildChannel> _channels;
    internal ImmutableDictionary<DiscordId, Thread> _activeThreads;
    internal ImmutableDictionary<DiscordId, GuildRole> _roles;
    internal ImmutableDictionary<DiscordId, Emoji> _emojis;
    internal ImmutableDictionary<DiscordId, StageInstance> _stageInstances;
    internal ImmutableDictionary<DiscordId, Presence> _presences;
    internal ImmutableDictionary<DiscordId, GuildSticker> _stickers;

    public override DiscordId Id => _jsonEntity.Id;
    public string Name => _jsonEntity.Name;
    public string? Icon => _jsonEntity.Icon;
    public string? IconHash => _jsonEntity.IconHash;
    public string? Splash => _jsonEntity.SplashHash;
    public string? DiscoverySplashHash => _jsonEntity.DiscoverySplashHash;
    //public bool? IsOwner => _jsonEntity.IsOwner;
    public DiscordId OwnerId => _jsonEntity.OwnerId;
    public GuildUser Owner => _users[OwnerId];
    //public string? Permissions => _jsonEntity.Permissions;
    public DiscordId? AfkChannelId => _jsonEntity.AfkChannelId;
    public int AfkTimeout => _jsonEntity.AfkTimeout;
    public bool? WidgetEnabled => _jsonEntity.WidgetEnabled;
    public DiscordId? WidgetChannelId => _jsonEntity.WidgetChannelId;
    public VerificationLevel VerificationLevel => _jsonEntity.VerificationLevel;
    public DefaultMessageNotificationLevel DefaultMessageNotificationLevel => _jsonEntity.DefaultMessageNotificationLevel;
    public ContentFilter ContentFilter => _jsonEntity.ContentFilter;
    //public IReadOnlyDictionary<DiscordId, Role> Roles
    //{
    //    get
    //    {
    //        lock (Roles)
    //            return new Dictionary<DiscordId, Role>(Roles);
    //    }
    //}
    public GuildRole EveryoneRole
    {
        get => Roles[Id];
    }
    //public IReadOnlyDictionary<DiscordId, Emoji> Emojis
    //{
    //    get
    //    {
    //        lock (Emojis)
    //            return new Dictionary<DiscordId, Emoji>(Emojis);
    //    }
    //}
    public GuildFeatures Features { get; }
    public MFALevel MFALevel => _jsonEntity.MFALevel;
    public DiscordId? ApplicationId => _jsonEntity.ApplicationId;
    public DiscordId? SystemChannelId => _jsonEntity.SystemChannelId;
    public SystemChannelFlags SystemChannelFlags => _jsonEntity.SystemChannelFlags;
    public DiscordId? RulesChannelId => _jsonEntity.RulesChannelId;
    public DateTimeOffset? CreatedAt => _jsonEntity.CreatedAt;
    public bool? IsLarge => _jsonEntity.IsLarge;
    public bool? IsUnavaible => _jsonEntity.IsUnavaible;
    public int? MemberCount { get; internal set; }
    //public IReadOnlyDictionary<DiscordId, GuildUser> Users
    //{
    //    get
    //    {
    //        lock (Users)
    //            return new Dictionary<DiscordId, GuildUser>(Users);
    //    }
    //}
    //public IReadOnlyDictionary<DiscordId, IGuildChannel> Channels
    //{
    //    get
    //    {
    //        lock (Channels)
    //            return new Dictionary<DiscordId, IGuildChannel>(Channels);
    //    }
    //}
    //public IReadOnlyDictionary<DiscordId, Thread> ActiveThreads
    //{
    //    get
    //    {
    //        lock (ActiveThreads)
    //            return new Dictionary<DiscordId, Thread>(ActiveThreads);
    //    }
    //}
    //public IReadOnlyDictionary<DiscordId, Presence> Presences
    //{
    //    get
    //    {
    //        lock (Presences)
    //            return new Dictionary<DiscordId, Presence>(Presences);
    //    }
    //}
    public int? MaxPresences => _jsonEntity.MaxPresences;
    public int? MaxMembers => _jsonEntity.MaxMembers;
    public string? VanityUrlCode => _jsonEntity.VanityUrlCode;
    public string? Description => _jsonEntity.Description;
    public string? BannerHash => _jsonEntity.BannerHash;
    public int PremiumTier => _jsonEntity.PremiumTier;
    public int? PremiumSubscriptionCount => _jsonEntity.PremiumSubscriptionCount;
    public System.Globalization.CultureInfo PreferredLocale => _jsonEntity.PreferredLocale;
    public DiscordId? PublicUpdatesChannelId => _jsonEntity.PublicUpdatesChannelId;
    public int? MaxVideoChannelUsers => _jsonEntity.MaxVideoChannelUsers;
    public int? ApproximateMemberCount { get; internal set; }
    public int? ApproximatePresenceCount => _jsonEntity.ApproximatePresenceCount;
    public GuildWelcomeScreen? WelcomeScreen { get; }
    public NSFWLevel NSFWLevel => _jsonEntity.NSFWLevel;
    //public IReadOnlyDictionary<DiscordId, StageInstance> StageInstances
    //{
    //    get
    //    {
    //        lock (StageInstances)
    //            return new Dictionary<DiscordId, StageInstance>(StageInstances);
    //    }
    //}

    //public IReadOnlyDictionary<DiscordId, GuildSticker> Stickers
    //{
    //    get
    //    {
    //        lock (Stickers)
    //            return new Dictionary<DiscordId, GuildSticker>(Stickers);
    //    }
    //}

    //public IReadOnlyDictionary<DiscordId, VoiceState> VoiceStates
    //{
    //    get
    //    {
    //        lock (VoiceStates)
    //            return new Dictionary<DiscordId, VoiceState>(VoiceStates);
    //    }
    //}

    internal Guild(JsonGuild jsonEntity, RestClient client) : base(client)
    {
        _jsonEntity = jsonEntity;

        _voiceStates = _jsonEntity.VoiceStates.ToImmutableDictionaryOrEmpty(s => s.UserId, s => new VoiceState(s));
        _users = _jsonEntity.Users.ToImmutableDictionaryOrEmpty(u => u.User.Id,
            u => new GuildUser(u, this, client));

        _channels = _jsonEntity.Channels.ToImmutableDictionaryOrEmpty(c => c.Id, c => (IGuildChannel)Channel.CreateFromJson(c, client));
        _activeThreads = _jsonEntity.ActiveThreads.ToImmutableDictionaryOrEmpty(t => t.Id, t => (Thread)Channel.CreateFromJson(t, client));
        _roles = _jsonEntity.Roles.ToImmutableDictionaryOrEmpty(r => r.Id, r => new GuildRole(r, client));
        // guild emojis always have Id
        _emojis = _jsonEntity.Emojis.ToImmutableDictionaryOrEmpty(e => e.Id.GetValueOrDefault(), e => new Emoji(e, client));
        _stageInstances = _jsonEntity.StageInstances.ToImmutableDictionaryOrEmpty(i => i.Id, i => new StageInstance(i, client));
        _stickers = _jsonEntity.Stickers.ToImmutableDictionaryOrEmpty(s => s.Id, s => new GuildSticker(s, client));
        MemberCount = _jsonEntity.MemberCount;
        ApproximateMemberCount = _jsonEntity.ApproximateMemberCount;
        _presences = _jsonEntity.Presences.ToImmutableDictionaryOrEmpty(p => p.User.Id, p => new Presence(p, client));
        Features = new(_jsonEntity.Features);
    }

    public Task KickUserAsync(DiscordId userId, RequestOptions? options = null) => _client.Guild.User.KickAsync(Id, userId, options);

    public Task BanUserAsync(DiscordId userId, RequestOptions? options = null) => _client.Guild.User.BanAsync(Id, userId, options);
    public Task BanUserAsync(DiscordId userId, int deleteMessageDays, RequestOptions? options = null) => _client.Guild.User.BanAsync(Id, userId, deleteMessageDays, options);

    public Task UnbanUserAsync(DiscordId userId, RequestOptions? options = null) => _client.Guild.User.UnbanAsync(Id, userId, options);

    public Task<Guild> ModifyAsync(Action<GuildOptions> action, RequestOptions? options = null) => _client.Guild.ModifyAsync(Id, action, options);
}