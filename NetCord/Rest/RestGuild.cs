using System.Collections.Immutable;

namespace NetCord.Rest;

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

    public Task<GuildPreview> GetPreviewAsync(RequestProperties? properties = null) => _client.GetGuildPreviewAsync(Id, properties);
    public Task<RestGuild> ModifyAsync(Action<GuildOptions> action, RequestProperties? properties = null) => _client.ModifyGuildAsync(Id, action, properties);
    public Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteGuildAsync(Id, properties);
    public Task<IReadOnlyDictionary<Snowflake, IGuildChannel>> GetChannelsAsync(RequestProperties? properties = null) => _client.GetGuildChannelsAsync(Id, properties);
    public Task<IGuildChannel> CreateChannelAsync(GuildChannelProperties channelBuilder, RequestProperties? properties = null) => _client.CreateGuildChannelAsync(Id, channelBuilder, properties);
    public Task ModifyPositionsAsync(ChannelPosition[] positions, RequestProperties? properties = null) => _client.ModifyGuildChannelPositionsAsync(Id, positions, properties);
    public Task<IReadOnlyDictionary<Snowflake, GuildThread>> GetActiveThreadsAsync(RequestProperties? properties = null) => _client.GetActiveGuildThreadsAsync(Id, properties);
    public IAsyncEnumerable<GuildBan> GetBansAsync(RequestProperties? properties = null) => _client.GetGuildBansAsync(Id, properties);
    public IAsyncEnumerable<GuildBan> GetBansBeforeAsync(Snowflake userId, RequestProperties? properties = null) => _client.GetGuildBansBeforeAsync(Id, userId, properties);
    public IAsyncEnumerable<GuildBan> GetBansAfterAsync(Snowflake userId, RequestProperties? properties = null) => _client.GetGuildBansAfterAsync(Id, userId, properties);
    public Task<GuildBan> GetBanAsync(Snowflake userId, RequestProperties? properties = null) => _client.GetGuildBanAsync(Id, userId, properties);
    public Task<IReadOnlyDictionary<Snowflake, GuildRole>> GetRolesAsync(RequestProperties? properties = null) => _client.GetGuildRolesAsync(Id, properties);
    public Task<GuildRole> CreateRoleAsync(GuildRoleProperties guildRoleProperties, RequestProperties? properties = null) => _client.CreateGuildRoleAsync(Id, guildRoleProperties, properties);
    public Task<IReadOnlyDictionary<Snowflake, GuildRole>> ModifyRolePositionsAsync(GuildRolePosition[] positions, RequestProperties? properties = null) => _client.ModifyGuildRolePositionsAsync(Id, positions, properties);
    public Task<GuildRole> ModifyRoleAsync(Snowflake roleId, Action<GuildRoleOptions> action, RequestProperties? properties = null) => _client.ModifyGuildRoleAsync(Id, roleId, action, properties);
    public Task DeleteRoleAsync(Snowflake roleId, RequestProperties? properties = null) => _client.DeleteGuildRoleAsync(Id, roleId, properties);
    public Task<int> GetPruneCountAsync(int days, Snowflake[]? roles = null, RequestProperties? properties = null) => _client.GetGuildPruneCountAsync(Id, days, roles, properties);
    public Task<int?> PruneAsync(GuildPruneProperties pruneProperties, RequestProperties? properties = null) => _client.GuildPruneAsync(Id, pruneProperties, properties);
    public Task<IEnumerable<VoiceRegion>> GetVoiceRegionsAsync(RequestProperties? properties = null) => _client.GetGuildVoiceRegionsAsync(Id, properties);
    public Task<IEnumerable<RestGuildInvite>> GetInvitesAsync(RequestProperties? properties = null) => _client.GetGuildInvitesAsync(Id, properties);
    public Task<IReadOnlyDictionary<Snowflake, Integration>> GetIntegrationsAsync(RequestProperties? properties = null) => _client.GetGuildIntegrationsAsync(Id, properties);
    public Task DeleteIntegrationAsync(Snowflake integrationId, RequestProperties? properties = null) => _client.DeleteGuildIntegrationAsync(Id, integrationId, properties);
    public Task<GuildWidgetSettings> GetWidgetSettingsAsync(RequestProperties? properties = null) => _client.GetGuildWidgetSettingsAsync(Id, properties);
    public Task<GuildWidgetSettings> ModifyWidgetSettingsAsync(Action<GuildWidgetSettingsOptions> action, RequestProperties? properties = null) => _client.ModifyGuildWidgetSettingsAsync(Id, action, properties);
    public Task<GuildWidget> GetWidgetAsync(RequestProperties? properties = null) => _client.GetGuildWidgetAsync(Id, properties);
    public Task<GuildVanityInvite> GetVanityInviteAsync(RequestProperties? properties = null) => _client.GetGuildVanityInviteAsync(Id, properties);
    public Task<GuildWelcomeScreen> GetWelcomeScreenAsync(RequestProperties? properties = null) => _client.GetGuildWelcomeScreenAsync(Id, properties);
    public Task<GuildWelcomeScreen> ModifyWelcomeScreenAsync(Action<GuildWelcomeScreenOptions> action, RequestProperties? properties = null) => _client.ModifyGuildWelcomeScreenAsync(Id, action, properties);
    public Task ModifyCurrentUserVoiceStateAsync(Snowflake channelId, Action<CurrentUserVoiceStateOptions> action, RequestProperties? properties = null) => _client.ModifyCurrentGuildUserVoiceStateAsync(Id, channelId, action, properties);
    public Task ModifyUserVoiceStateAsync(Snowflake channelId, Snowflake userId, Action<VoiceStateOptions> action, RequestProperties? properties = null) => _client.ModifyGuildUserVoiceStateAsync(Id, channelId, userId, action, properties);

    public Task KickUserAsync(Snowflake userId, RequestProperties? properties = null) => _client.KickGuildUserAsync(Id, userId, properties);

    public Task BanUserAsync(Snowflake userId, RequestProperties? properties = null) => _client.BanGuildUserAsync(Id, userId, properties);
    public Task BanUserAsync(Snowflake userId, int deleteMessageDays, RequestProperties? properties = null) => _client.BanGuildUserAsync(Id, userId, deleteMessageDays, properties);

    public Task UnbanUserAsync(Snowflake userId, RequestProperties? properties = null) => _client.UnbanGuildUserAsync(Id, userId, properties);
}