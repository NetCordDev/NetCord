using System.Collections.Immutable;

namespace NetCord;

public class RestGuild : ClientEntity, IJsonModel<JsonModels.JsonGuild>
{
    JsonModels.JsonGuild IJsonModel<JsonModels.JsonGuild>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonGuild _jsonModel;

    public ImmutableDictionary<Snowflake, GuildRole> Roles { get; internal set; }
    public ImmutableDictionary<Snowflake, GuildEmoji> Emojis { get; internal set; }
    public ImmutableDictionary<Snowflake, GuildSticker> Stickers { get; internal set; }

    public override Snowflake Id => _jsonModel.Id;
    public string Name => _jsonModel.Name;
    public string? Icon => _jsonModel.Icon;
    public string? Splash => _jsonModel.SplashHash;
    public string? DiscoverySplashHash => _jsonModel.DiscoverySplashHash;
    public Snowflake OwnerId => _jsonModel.OwnerId;
    public Snowflake? AfkChannelId => _jsonModel.AfkChannelId;
    public int AfkTimeout => _jsonModel.AfkTimeout;
    public bool? WidgetEnabled => _jsonModel.WidgetEnabled;
    public Snowflake? WidgetChannelId => _jsonModel.WidgetChannelId;
    public VerificationLevel VerificationLevel => _jsonModel.VerificationLevel;
    public DefaultMessageNotificationLevel DefaultMessageNotificationLevel => _jsonModel.DefaultMessageNotificationLevel;
    public ContentFilter ContentFilter => _jsonModel.ContentFilter;
    public GuildRole EveryoneRole => Roles[Id];
    public GuildFeatures Features { get; }
    public MFALevel MFALevel => _jsonModel.MFALevel;
    public Snowflake? ApplicationId => _jsonModel.ApplicationId;
    public Snowflake? SystemChannelId => _jsonModel.SystemChannelId;
    public SystemChannelFlags SystemChannelFlags => _jsonModel.SystemChannelFlags;
    public Snowflake? RulesChannelId => _jsonModel.RulesChannelId;
    public int? MaxPresences => _jsonModel.MaxPresences;
    public int? MaxMembers => _jsonModel.MaxMembers;
    public string? VanityUrlCode => _jsonModel.VanityUrlCode;
    public string? Description => _jsonModel.Description;
    public string? BannerHash => _jsonModel.BannerHash;
    public int PremiumTier => _jsonModel.PremiumTier;
    public int? PremiumSubscriptionCount => _jsonModel.PremiumSubscriptionCount;
    public System.Globalization.CultureInfo PreferredLocale => _jsonModel.PreferredLocale;
    public Snowflake? PublicUpdatesChannelId => _jsonModel.PublicUpdatesChannelId;
    public int? MaxVideoChannelUsers => _jsonModel.MaxVideoChannelUsers;
    public int? ApproximateMemberCount => _jsonModel.ApproximateMemberCount;
    public int? ApproximatePresenceCount => _jsonModel.ApproximatePresenceCount;
    public GuildWelcomeScreen? WelcomeScreen { get; }
    public NSFWLevel NSFWLevel => _jsonModel.NSFWLevel;
    public bool PremiumProgressBarEnabled => _jsonModel.PremiumPropressBarEnabled;

    public RestGuild(JsonModels.JsonGuild jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        Roles = jsonModel.Roles.ToImmutableDictionaryOrEmpty(r => r.Id, r => new GuildRole(r, client));
        // guild emojis always have Id
        Emojis = jsonModel.Emojis.ToImmutableDictionaryOrEmpty(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, Id, client));
        Stickers = jsonModel.Stickers.ToImmutableDictionaryOrEmpty(s => s.Id, s => new GuildSticker(s, client));
        Features = new(jsonModel.Features);
        if (jsonModel.WelcomeScreen != null)
            WelcomeScreen = new(jsonModel.WelcomeScreen);
    }

    public Task<GuildPreview> GetPreviewAsync(RequestProperties? options = null) => _client.GetGuildPreviewAsync(Id, options);
    public Task<RestGuild> ModifyAsync(Action<GuildOptions> action, RequestProperties? options = null) => _client.ModifyGuildAsync(Id, action, options);
    public Task DeleteAsync(RequestProperties? options = null) => _client.DeleteGuildAsync(Id, options);
    public Task<IReadOnlyDictionary<Snowflake, IGuildChannel>> GetChannelsAsync(RequestProperties? options = null) => _client.GetGuildChannelsAsync(Id, options);
    public Task<IGuildChannel> CreateChannelAsync(GuildChannelProperties channelBuilder, RequestProperties? options = null) => _client.CreateGuildChannelAsync(Id, channelBuilder, options);
    public Task ModifyPositionsAsync(ChannelPosition[] positions, RequestProperties? options = null) => _client.ModifyGuildChannelPositionsAsync(Id, positions, options);
    public Task<IReadOnlyDictionary<Snowflake, GuildThread>> GetActiveThreadsAsync(RequestProperties? options = null) => _client.GetActiveGuildThreadsAsync(Id, options);
    public IAsyncEnumerable<GuildBan> GetBansAsync(RequestProperties? options = null) => _client.GetGuildBansAsync(Id, options);
    public IAsyncEnumerable<GuildBan> GetBansBeforeAsync(Snowflake userId, RequestProperties? options = null) => _client.GetGuildBansBeforeAsync(Id, userId, options);
    public IAsyncEnumerable<GuildBan> GetBansAfterAsync(Snowflake userId, RequestProperties? options = null) => _client.GetGuildBansAfterAsync(Id, userId, options);
    public Task<GuildBan> GetBanAsync(Snowflake userId, RequestProperties? options = null) => _client.GetGuildBanAsync(Id, userId, options);
    public Task<IReadOnlyDictionary<Snowflake, GuildRole>> GetRolesAsync(RequestProperties? options = null) => _client.GetGuildRolesAsync(Id, options);
    public Task<GuildRole> CreateRoleAsync(GuildRoleProperties guildRoleProperties, RequestProperties? options = null) => _client.CreateGuildRoleAsync(Id, guildRoleProperties, options);
    public Task<IReadOnlyDictionary<Snowflake, GuildRole>> ModifyRolePositionsAsync(GuildRolePosition[] positions, RequestProperties? options = null) => _client.ModifyGuildRolePositionsAsync(Id, positions, options);
    public Task<GuildRole> ModifyRoleAsync(Snowflake roleId, Action<GuildRoleOptions> action, RequestProperties? options = null) => _client.ModifyGuildRoleAsync(Id, roleId, action, options);
    public Task DeleteRoleAsync(Snowflake roleId, RequestProperties? options = null) => _client.DeleteGuildRoleAsync(Id, roleId, options);
    public Task<int> GetPruneCountAsync(int days, Snowflake[]? roles = null, RequestProperties? options = null) => _client.GetGuildPruneCountAsync(Id, days, roles, options);
    public Task<int?> PruneAsync(GuildPruneProperties pruneProperties, RequestProperties? options = null) => _client.GuildPruneAsync(Id, pruneProperties, options);
    public Task<IEnumerable<VoiceRegion>> GetVoiceRegionsAsync(RequestProperties? options = null) => _client.GetGuildVoiceRegionsAsync(Id, options);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? options = null) => _client.GetGuildInvitesAsync(Id, options);
    public Task<IReadOnlyDictionary<Snowflake, Integration>> GetIntegrationsAsync(RequestProperties? options = null) => _client.GetGuildIntegrationsAsync(Id, options);
    public Task DeleteIntegrationAsync(Snowflake integrationId, RequestProperties? options = null) => _client.DeleteGuildIntegrationAsync(Id, integrationId, options);
    public Task<GuildWidgetSettings> GetWidgetSettingsAsync(RequestProperties? options = null) => _client.GetGuildWidgetSettingsAsync(Id, options);
    public Task<GuildWidgetSettings> ModifyWidgetSettingsAsync(Action<GuildWidgetSettingsOptions> action, RequestProperties? options = null) => _client.ModifyGuildWidgetSettingsAsync(Id, action, options);
    public Task<GuildWidget> GetWidgetAsync(RequestProperties? options = null) => _client.GetGuildWidgetAsync(Id, options);
    public Task<GuildVanityInvite> GetVanityInviteAsync(RequestProperties? options = null) => _client.GetGuildVanityInviteAsync(Id, options);
    public Task<GuildWelcomeScreen> GetWelcomeScreenAsync(RequestProperties? options = null) => _client.GetGuildWelcomeScreenAsync(Id, options);
    public Task<GuildWelcomeScreen> ModifyWelcomeScreenAsync(Action<GuildWelcomeScreenOptions> action, RequestProperties? options = null) => _client.ModifyGuildWelcomeScreenAsync(Id, action, options);
    public Task ModifyCurrentUserVoiceStateAsync(Snowflake channelId, Action<CurrentUserVoiceStateOptions> action, RequestProperties? options = null) => _client.ModifyCurrentGuildUserVoiceStateAsync(Id, channelId, action, options);
    public Task ModifyUserVoiceStateAsync(Snowflake channelId, Snowflake userId, Action<VoiceStateOptions> action, RequestProperties? options = null) => _client.ModifyGuildUserVoiceStateAsync(Id, channelId, userId, action, options);

    public Task KickUserAsync(Snowflake userId, RequestProperties? options = null) => _client.KickGuildUserAsync(Id, userId, options);

    public Task BanUserAsync(Snowflake userId, RequestProperties? options = null) => _client.BanGuildUserAsync(Id, userId, options);
    public Task BanUserAsync(Snowflake userId, int deleteMessageDays, RequestProperties? options = null) => _client.BanGuildUserAsync(Id, userId, deleteMessageDays, options);

    public Task UnbanUserAsync(Snowflake userId, RequestProperties? options = null) => _client.UnbanGuildUserAsync(Id, userId, options);
}