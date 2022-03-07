using System.Collections.Immutable;

namespace NetCord;

public class RestGuild : ClientEntity
{
    private protected readonly JsonModels.JsonGuild _jsonEntity;

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

    public Task<GuildPreview> GetPreviewAsync(RequestProperties? options = null) => _client.GetGuildPreviewAsync(Id, options);
    public Task<RestGuild> ModifyAsync(Action<GuildOptions> action, RequestProperties? options = null) => _client.ModifyGuildAsync(Id, action, options);
    public Task DeleteAsync(RequestProperties? options = null) => _client.DeleteGuildAsync(Id, options);
    public Task<IReadOnlyDictionary<DiscordId, IGuildChannel>> GetChannelsAsync(RequestProperties? options = null) => _client.GetGuildChannelsAsync(Id, options);
    public Task<IGuildChannel> CreateChannelAsync(GuildChannelProperties channelBuilder, RequestProperties? options = null) => _client.CreateGuildChannelAsync(Id, channelBuilder, options);
    public Task ModifyPositionsAsync(ChannelPosition[] positions, RequestProperties? options = null) => _client.ModifyGuildChannelPositionsAsync(Id, positions, options);
    public Task<(IReadOnlyDictionary<DiscordId, GuildThread> Threads, IReadOnlyDictionary<DiscordId, ThreadUser> CurrentUsers)> GetActiveThreadsAsync(RequestProperties? options = null) => _client.GetActiveGuildThreadsAsync(Id, options);
    public Task<IReadOnlyDictionary<DiscordId, GuildBan>> GetBansAsync(RequestProperties? options = null) => _client.GetGuildBansAsync(Id, options);
    public Task<GuildBan> GetBanAsync(DiscordId userId, RequestProperties? options = null) => _client.GetGuildBanAsync(Id, userId, options);
    public Task<IReadOnlyDictionary<DiscordId, GuildRole>> GetRolesAsync(RequestProperties? options = null) => _client.GetGuildRolesAsync(Id, options);
    public Task<GuildRole> CreateRoleAsync(GuildRoleProperties guildRoleProperties, RequestProperties? options = null) => _client.CreateGuildRoleAsync(Id, guildRoleProperties, options);
    public Task<IReadOnlyDictionary<DiscordId, GuildRole>> ModifyRolePositionsAsync(GuildRolePosition[] positions, RequestProperties? options = null) => _client.ModifyGuildRolePositionsAsync(Id, positions, options);
    public Task<GuildRole> ModifyRoleAsync(DiscordId roleId, Action<GuildRoleOptions> action, RequestProperties? options = null) => _client.ModifyGuildRoleAsync(Id, roleId, action, options);
    public Task DeleteRoleAsync(DiscordId roleId, RequestProperties? options = null) => _client.DeleteGuildRoleAsync(Id, roleId, options);
    public Task<int> GetPruneCountAsync(int days, DiscordId[]? roles = null, RequestProperties? options = null) => _client.GetGuildPruneCountAsync(Id, days, roles, options);
    public Task<int?> PruneAsync(GuildPruneProperties pruneProperties, RequestProperties? options = null) => _client.GuildPruneAsync(Id, pruneProperties, options);
    public Task<IEnumerable<VoiceRegion>> GetVoiceRegionsAsync(RequestProperties? options = null) => _client.GetGuildVoiceRegionsAsync(Id, options);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? options = null) => _client.GetGuildInvitesAsync(Id, options);
    public Task<IReadOnlyDictionary<DiscordId, Integration>> GetIntegrationsAsync(RequestProperties? options = null) => _client.GetGuildIntegrationsAsync(Id, options);
    public Task DeleteIntegrationAsync(DiscordId integrationId, RequestProperties? options = null) => _client.DeleteGuildIntegrationAsync(Id, integrationId, options);
    public Task<GuildWidgetSettings> GetWidgetSettingsAsync(RequestProperties? options = null) => _client.GetGuildWidgetSettingsAsync(Id, options);
    public Task<GuildWidgetSettings> ModifyWidgetSettingsAsync(Action<GuildWidgetSettingsOptions> action, RequestProperties? options = null) => _client.ModifyGuildWidgetSettingsAsync(Id, action, options);
    public Task<GuildWidget> GetWidgetAsync(RequestProperties? options = null) => _client.GetGuildWidgetAsync(Id, options);
    public Task<GuildVanityInvite> GetVanityInviteAsync(RequestProperties? options = null) => _client.GetGuildVanityInviteAsync(Id, options);
    public Task<GuildWelcomeScreen> GetWelcomeScreenAsync(RequestProperties? options = null) => _client.GetGuildWelcomeScreenAsync(Id, options);
    public Task<GuildWelcomeScreen> ModifyWelcomeScreenAsync(Action<GuildWelcomeScreenOptions> action, RequestProperties? options = null) => _client.ModifyGuildWelcomeScreenAsync(Id, action, options);
    public Task ModifyCurrentUserVoiceStateAsync(DiscordId channelId, Action<CurrentUserVoiceStateOptions> action, RequestProperties? options = null) => _client.ModifyCurrentGuildUserVoiceStateAsync(Id, channelId, action, options);
    public Task ModifyUserVoiceStateAsync(DiscordId channelId, DiscordId userId, Action<VoiceStateOptions> action, RequestProperties? options = null) => _client.ModifyGuildUserVoiceStateAsync(Id, channelId, userId, action, options);

    public Task KickUserAsync(DiscordId userId, RequestProperties? options = null) => _client.KickGuildUserAsync(Id, userId, options);

    public Task BanUserAsync(DiscordId userId, RequestProperties? options = null) => _client.BanGuildUserAsync(Id, userId, options);
    public Task BanUserAsync(DiscordId userId, int deleteMessageDays, RequestProperties? options = null) => _client.BanGuildUserAsync(Id, userId, deleteMessageDays, options);

    public Task UnbanUserAsync(DiscordId userId, RequestProperties? options = null) => _client.UnbanGuildUserAsync(Id, userId, options);
}