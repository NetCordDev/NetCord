using System.Collections.Immutable;

namespace NetCord;

public class RestGuild : ClientEntity
{
    private readonly JsonModels.JsonGuild _jsonEntity;

    public ImmutableDictionary<DiscordId, GuildRole> Roles => _roles;
    public ImmutableDictionary<DiscordId, Emoji> Emojis => _emojis;
    public ImmutableDictionary<DiscordId, GuildSticker> Stickers => _stickers;

    internal ImmutableDictionary<DiscordId, GuildRole> _roles;
    internal ImmutableDictionary<DiscordId, Emoji> _emojis;
    internal ImmutableDictionary<DiscordId, GuildSticker> _stickers;

    public override DiscordId Id => _jsonEntity.Id;
    public string Name => _jsonEntity.Name;
    public string? Icon => _jsonEntity.Icon;
    public string? Splash => _jsonEntity.SplashHash;
    public string? DiscoverySplashHash => _jsonEntity.DiscoverySplashHash;
    public DiscordId OwnerId => _jsonEntity.OwnerId;
    public DiscordId? AfkChannelId => _jsonEntity.AfkChannelId;
    public int AfkTimeout => _jsonEntity.AfkTimeout;
    public bool? WidgetEnabled => _jsonEntity.WidgetEnabled;
    public DiscordId? WidgetChannelId => _jsonEntity.WidgetChannelId;
    public VerificationLevel VerificationLevel => _jsonEntity.VerificationLevel;
    public DefaultMessageNotificationLevel DefaultMessageNotificationLevel => _jsonEntity.DefaultMessageNotificationLevel;
    public ContentFilter ContentFilter => _jsonEntity.ContentFilter;
    public GuildRole EveryoneRole => Roles[Id];
    public GuildFeatures Features { get; }
    public MFALevel MFALevel => _jsonEntity.MFALevel;
    public DiscordId? ApplicationId => _jsonEntity.ApplicationId;
    public DiscordId? SystemChannelId => _jsonEntity.SystemChannelId;
    public SystemChannelFlags SystemChannelFlags => _jsonEntity.SystemChannelFlags;
    public DiscordId? RulesChannelId => _jsonEntity.RulesChannelId;
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
    public int? ApproximateMemberCount => _jsonEntity.ApproximateMemberCount;
    public int? ApproximatePresenceCount => _jsonEntity.ApproximatePresenceCount;
    public GuildWelcomeScreen? WelcomeScreen { get; }
    public NSFWLevel NSFWLevel => _jsonEntity.NSFWLevel;
    public bool PremiumProgressBarEnabled => _jsonEntity.PremiumPropressBarEnabled;

    internal RestGuild(JsonModels.JsonGuild jsonEntity, RestClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
        _roles = jsonEntity.Roles.ToImmutableDictionaryOrEmpty(r => r.Id, r => new GuildRole(r, client));
        // guild emojis always have Id
        _emojis = jsonEntity.Emojis.ToImmutableDictionaryOrEmpty(e => e.Id.GetValueOrDefault(), e => new Emoji(e, client));
        _stickers = jsonEntity.Stickers.ToImmutableDictionaryOrEmpty(s => s.Id, s => new GuildSticker(s, client));
        Features = new(jsonEntity.Features);
        if (jsonEntity.WelcomeScreen != null)
            WelcomeScreen = new(jsonEntity.WelcomeScreen);
    }

    public Task<GuildPreview> GetPreviewAsync(RequestOptions? options = null) => _client.Guild.GetPreviewAsync(Id, options);
    public Task<RestGuild> ModifyAsync(Action<GuildOptions> action, RequestOptions? options = null) => _client.Guild.ModifyAsync(Id, action, options);
    public Task DeleteAsync(RequestOptions? options = null) => _client.Guild.DeleteAsync(Id, options);
    public Task<IReadOnlyDictionary<DiscordId, IGuildChannel>> GetChannelsAsync(RequestOptions? options = null) => _client.Guild.Channel.GetAsync(Id, options);
    public Task<IGuildChannel> CreateChannelAsync(GuildChannelProperties channelBuilder, RequestOptions? options = null) => _client.Guild.Channel.CreateAsync(Id, channelBuilder, options);
    public Task ModifyPositionsAsync(ChannelPosition[] positions, RequestOptions? options = null) => _client.Guild.Channel.ModifyPositionsAsync(Id, positions, options);
    public Task<(IReadOnlyDictionary<DiscordId, Thread> Threads, IReadOnlyDictionary<DiscordId, ThreadUser> CurrentUsers)> GetActiveThreadsAsync(RequestOptions? options = null) => _client.Guild.Channel.GetActiveThreadsAsync(Id, options);
    public Task<IReadOnlyDictionary<DiscordId, GuildBan>> GetBansAsync(RequestOptions? options = null) => _client.Guild.GetBansAsync(Id, options);
    public Task<GuildBan> GetBanAsync(DiscordId userId, RequestOptions? options = null) => _client.Guild.GetBanAsync(Id, userId, options);
    public Task<IReadOnlyDictionary<DiscordId, GuildRole>> GetRolesAsync(RequestOptions? options = null) => _client.Guild.GetRolesAsync(Id, options);
    public Task<GuildRole> CreateRoleAsync(GuildRoleProperties guildRoleProperties, RequestOptions? options = null) => _client.Guild.CreateRoleAsync(Id, guildRoleProperties, options);
    public Task<IReadOnlyDictionary<DiscordId, GuildRole>> ModifyRolePositionsAsync(GuildRolePosition[] positions, RequestOptions? options = null) => _client.Guild.ModifyRolePositionsAsync(Id, positions, options);
    public Task<GuildRole> ModifyRoleAsync(DiscordId roleId, Action<GuildRoleOptions> action, RequestOptions? options = null) => _client.Guild.ModifyRoleAsync(Id, roleId, action, options);
    public Task DeleteRoleAsync(DiscordId roleId, RequestOptions? options = null) => _client.Guild.DeleteRoleAsync(Id, roleId, options);
    public Task<int> GetPruneCountAsync(int days, DiscordId[]? roles = null, RequestOptions? options = null) => _client.Guild.GetPruneCountAsync(Id, days, roles, options);
    public Task<int?> PruneAsync(GuildPruneProperties pruneProperties, RequestOptions? options = null) => _client.Guild.PruneAsync(Id, pruneProperties, options);
    public Task<IEnumerable<VoiceRegion>> GetVoiceRegionsAsync(RequestOptions? options = null) => _client.Guild.GetVoiceRegionsAsync(Id, options);
    public Task<IEnumerable<GuildInvite>> GetInvitesAsync(RequestOptions? options = null) => _client.Guild.GetInvitesAsync(Id, options);
    public Task<IReadOnlyDictionary<DiscordId, Integration>> GetIntegrationsAsync(RequestOptions? options = null) => _client.Guild.GetIntegrationsAsync(Id, options);
    public Task DeleteIntegrationAsync(DiscordId integrationId, RequestOptions? options = null) => _client.Guild.DeleteIntegrationAsync(Id, integrationId, options);
    public Task<GuildWidgetSettings> GetWidgetSettingsAsync(RequestOptions? options = null) => _client.Guild.GetWidgetSettingsAsync(Id, options);
    public Task<GuildWidgetSettings> ModifyWidgetSettingsAsync(Action<GuildWidgetSettingsOptions> action, RequestOptions? options = null) => _client.Guild.ModifyWidgetSettingsAsync(Id, action, options);
    public Task<GuildWidget> GetWidgetAsync(RequestOptions? options = null) => _client.Guild.GetWidgetAsync(Id, options);
    public Task<GuildVanityInvite> GetVanityInviteAsync(RequestOptions? options = null) => _client.Guild.GetVanityInviteAsync(Id, options);
    public Task<GuildWelcomeScreen> GetWelcomeScreenAsync(RequestOptions? options = null) => _client.Guild.GetWelcomeScreenAsync(Id, options);
    public Task<GuildWelcomeScreen> ModifyWelcomeScreenAsync(Action<GuildWelcomeScreenOptions> action, RequestOptions? options = null) => _client.Guild.ModifyWelcomeScreenAsync(Id, action, options);
    public Task ModifyCurrentUserVoiceStateAsync(DiscordId channelId, Action<CurrentUserVoiceStateOptions> action, RequestOptions? options = null) => _client.Guild.ModifyCurrentUserVoiceStateAsync(Id, channelId, action, options);
    public Task ModifyUserVoiceStateAsync(DiscordId channelId, Action<VoiceStateOptions> action, RequestOptions? options = null) => _client.Guild.ModifyUserVoiceStateAsync(Id, channelId, action, options);

    public Task KickUserAsync(DiscordId userId, RequestOptions? options = null) => _client.Guild.User.KickAsync(Id, userId, options);

    public Task BanUserAsync(DiscordId userId, RequestOptions? options = null) => _client.Guild.User.BanAsync(Id, userId, options);
    public Task BanUserAsync(DiscordId userId, int deleteMessageDays, RequestOptions? options = null) => _client.Guild.User.BanAsync(Id, userId, deleteMessageDays, options);

    public Task UnbanUserAsync(DiscordId userId, RequestOptions? options = null) => _client.Guild.User.UnbanAsync(Id, userId, options);


}