using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace NetCord.Rest;

public class RestGuild : ClientEntity, IJsonModel<JsonModels.JsonGuild>
{
    JsonModels.JsonGuild IJsonModel<JsonModels.JsonGuild>.JsonModel => _jsonModel;
    internal readonly JsonModels.JsonGuild _jsonModel;

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
    public ReadOnlyCollection<string> Features { get; }
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
        Roles = jsonModel.Roles.ToImmutableDictionaryOrEmpty(r => new GuildRole(r, Id, client));
        // guild emojis always have Id
        Emojis = jsonModel.Emojis.ToImmutableDictionaryOrEmpty(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, Id, client));
        Stickers = jsonModel.Stickers.ToImmutableDictionaryOrEmpty(s => s.Id, s => new GuildSticker(s, client));
        Features = new(jsonModel.Features);
        if (jsonModel.WelcomeScreen != null)
            WelcomeScreen = new(jsonModel.WelcomeScreen);
    }

    #region Guild
    public Task<GuildPreview> GetPreviewAsync(RequestProperties? properties = null) => _client.GetGuildPreviewAsync(Id, properties);
    public Task<RestGuild> ModifyAsync(Action<GuildOptions> action, RequestProperties? properties = null) => _client.ModifyGuildAsync(Id, action, properties);
    public Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteGuildAsync(Id, properties);
    public IAsyncEnumerable<GuildBan> GetBansAsync(RequestProperties? properties = null) => _client.GetGuildBansAsync(Id, properties);
    public IAsyncEnumerable<GuildBan> GetBansBeforeAsync(Snowflake userId, RequestProperties? properties = null) => _client.GetGuildBansBeforeAsync(Id, userId, properties);
    public IAsyncEnumerable<GuildBan> GetBansAfterAsync(Snowflake userId, RequestProperties? properties = null) => _client.GetGuildBansAfterAsync(Id, userId, properties);
    public Task<GuildBan> GetBanAsync(Snowflake userId, RequestProperties? properties = null) => _client.GetGuildBanAsync(Id, userId, properties);
    public Task<IReadOnlyDictionary<Snowflake, GuildRole>> GetRolesAsync(RequestProperties? properties = null) => _client.GetGuildRolesAsync(Id, properties);
    public Task<GuildRole> CreateRoleAsync(GuildRoleProperties guildRoleProperties, RequestProperties? properties = null) => _client.CreateGuildRoleAsync(Id, guildRoleProperties, properties);
    public Task<IReadOnlyDictionary<Snowflake, GuildRole>> ModifyRolePositionsAsync(GuildRolePositionProperties[] positions, RequestProperties? properties = null) => _client.ModifyGuildRolePositionsAsync(Id, positions, properties);
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
    public Task<IReadOnlyDictionary<Snowflake, IGuildChannel>> GetChannelsAsync(RequestProperties? properties = null) => _client.GetGuildChannelsAsync(Id, properties);
    public Task<IGuildChannel> CreateChannelAsync(GuildChannelProperties channelBuilder, RequestProperties? properties = null) => _client.CreateGuildChannelAsync(Id, channelBuilder, properties);
    public Task ModifyChannelPositionsAsync(IEnumerable<ChannelPositionProperties> positions, RequestProperties? properties = null) => _client.ModifyGuildChannelPositionsAsync(Id, positions, properties);
    public Task<IReadOnlyDictionary<Snowflake, GuildThread>> GetActiveThreadsAsync(RequestProperties? properties = null) => _client.GetActiveGuildThreadsAsync(Id, properties);
    public Task<GuildUser> GetUserAsync(Snowflake userId, RequestProperties? properties = null) => _client.GetGuildUserAsync(Id, userId, properties);
    public IAsyncEnumerable<GuildUser> GetUsersAsync(RequestProperties? properties = null) => _client.GetGuildUsersAsync(Id, properties);
    public IAsyncEnumerable<GuildUser> GetUsersAfterAsync(Snowflake userId, RequestProperties? properties = null) => _client.GetGuildUsersAfterAsync(Id, userId, properties);
    public Task<IReadOnlyDictionary<Snowflake, GuildUser>> FindUserAsync(string name, int limit, RequestProperties? properties = null) => _client.FindGuildUserAsync(Id, name, limit, properties);
    public Task<GuildUser?> AddUserAsync(Snowflake userId, UserProperties userProperties, RequestProperties? properties = null) => _client.AddGuildUserAsync(Id, userId, userProperties, properties);
    public Task<GuildUser> ModifyUserAsync(Snowflake userId, Action<GuildUserOptions> action, RequestProperties? properties = null) => _client.ModifyGuildUserAsync(Id, userId, action, properties);
    public Task<GuildUser> ModifyCurrentUserAsync(Action<CurrentGuildUserOptions> action, RequestProperties? properties = null) => _client.ModifyCurrentGuildUserAsync(Id, action, properties);
    public Task AddUserRoleAsync(Snowflake userId, Snowflake roleId, RequestProperties? properties = null) => _client.AddGuildUserRoleAsync(Id, userId, roleId, properties);
    public Task RemoveUserRoleAsync(Snowflake userId, Snowflake roleId, RequestProperties? properties = null) => _client.RemoveGuildUserRoleAsync(Id, userId, roleId, properties);
    public Task KickUserAsync(Snowflake userId, RequestProperties? properties = null) => _client.KickGuildUserAsync(Id, userId, properties);
    public Task BanUserAsync(Snowflake userId, RequestProperties? properties = null) => _client.BanGuildUserAsync(Id, userId, properties);
    public Task BanUserAsync(Snowflake userId, int deleteMessageDays, RequestProperties? properties = null) => _client.BanGuildUserAsync(Id, userId, deleteMessageDays, properties);
    public Task UnbanUserAsync(Snowflake userId, RequestProperties? properties = null) => _client.UnbanGuildUserAsync(Id, userId, properties);
    public Task ModifyCurrentUserVoiceStateAsync(Snowflake channelId, Action<CurrentUserVoiceStateOptions> action, RequestProperties? properties = null) => _client.ModifyCurrentGuildUserVoiceStateAsync(Id, channelId, action, properties);
    public Task ModifyUserVoiceStateAsync(Snowflake channelId, Snowflake userId, Action<VoiceStateOptions> action, RequestProperties? properties = null) => _client.ModifyGuildUserVoiceStateAsync(Id, channelId, userId, action, properties);
    #endregion

    #region AuditLog
    public IAsyncEnumerable<AuditLogEntry> GetAuditLogAsync(Snowflake? userId = null, AuditLogEvent? actionType = null, RequestProperties? properties = null) => _client.GetGuildAuditLogAsync(Id, userId, actionType, properties);
    public IAsyncEnumerable<AuditLogEntry> GetGuildAuditLogBeforeAsync(Snowflake before, Snowflake? userId = null, AuditLogEvent? actionType = null, RequestProperties? properties = null) => _client.GetGuildAuditLogBeforeAsync(Id, before, userId, actionType, properties);
    #endregion

    #region AutoModeration
    public Task<IReadOnlyDictionary<Snowflake, AutoModerationRule>> GetAutoModerationRulesAsync(RequestProperties? properties = null) => _client.GetAutoModerationRulesAsync(Id, properties);
    public Task<AutoModerationRule> GetAutoModerationRuleAsync(Snowflake autoModerationRuleId, RequestProperties? properties = null) => _client.GetAutoModerationRuleAsync(Id, autoModerationRuleId, properties);
    public Task<AutoModerationRule> CreateAutoModerationRuleAsync(AutoModerationRuleProperties autoModerationRuleProperties, RequestProperties? properties = null) => _client.CreateAutoModerationRuleAsync(Id, autoModerationRuleProperties, properties);
    public Task<AutoModerationRule> ModifyAutoModerationRuleAsync(Snowflake autoModerationRuleId, Action<AutoModerationRuleOptions> action, RequestProperties? properties = null) => _client.ModifyAutoModerationRuleAsync(Id, autoModerationRuleId, action, properties);
    public Task DeleteAutoModerationRuleAsync(Snowflake autoModerationRuleId, RequestProperties? properties = null) => _client.DeleteAutoModerationRuleAsync(Id, autoModerationRuleId, properties);
    #endregion

    #region Emoji
    public Task<IReadOnlyDictionary<Snowflake, GuildEmoji>> GetEmojisAsync(RequestProperties? properties = null) => _client.GetGuildEmojisAsync(Id, properties);
    public Task<GuildEmoji> GetEmojiAsync(Snowflake emojiId, RequestProperties? properties = null) => _client.GetGuildEmojiAsync(Id, emojiId, properties);
    public Task<GuildEmoji> CreateEmojiAsync(GuildEmojiProperties guildEmojiProperties, RequestProperties? properties = null) => _client.CreateGuildEmojiAsync(Id, guildEmojiProperties, properties);
    public Task<GuildEmoji> ModifyEmojiAsync(Snowflake emojiId, Action<GuildEmojiOptions> action, RequestProperties? properties = null) => _client.ModifyGuildEmojiAsync(Id, emojiId, action, properties);
    public Task DeleteEmojiAsync(Snowflake emojiId, RequestProperties? properties = null) => _client.DeleteGuildEmojiAsync(Id, emojiId, properties);
    #endregion

    #region GuildScheduledEvent
    public Task<IReadOnlyDictionary<Snowflake, GuildScheduledEvent>> GetGuildScheduledEventsAsync(bool withUserCount = false, RequestProperties? properties = null) => _client.GetGuildScheduledEventsAsync(Id, withUserCount, properties);
    public Task<GuildScheduledEvent> CreateGuildScheduledEventAsync(GuildScheduledEventProperties guildScheduledEventProperties, RequestProperties? properties = null) => _client.CreateGuildScheduledEventAsync(Id, guildScheduledEventProperties, properties);
    public Task<GuildScheduledEvent> GetGuildScheduledEventAsync(Snowflake scheduledEventId, bool withUserCount = false, RequestProperties? properties = null) => _client.GetGuildScheduledEventAsync(Id, scheduledEventId, withUserCount, properties);
    public Task<GuildScheduledEvent> ModifyGuildScheduledEventAsync(Snowflake scheduledEventId, Action<GuildScheduledEventOptions> action, RequestProperties? properties = null) => _client.ModifyGuildScheduledEventAsync(Id, scheduledEventId, action, properties);
    public Task DeleteGuildScheduledEventAsync(Snowflake scheduledEventId, RequestProperties? properties = null) => _client.DeleteGuildScheduledEventAsync(Id, scheduledEventId, properties);
    public IAsyncEnumerable<GuildScheduledEventUser> GetGuildScheduledEventUsersAsync(Snowflake scheduledEventId, bool guildUsers = false, RequestProperties? properties = null) => _client.GetGuildScheduledEventUsersAsync(Id, scheduledEventId, guildUsers, properties);
    public IAsyncEnumerable<GuildScheduledEventUser> GetGuildScheduledEventUsersAfterAsync(Snowflake scheduledEventId, Snowflake userId, bool guildUsers = false, RequestProperties? properties = null) => _client.GetGuildScheduledEventUsersAfterAsync(Id, scheduledEventId, userId, guildUsers, properties);
    public IAsyncEnumerable<GuildScheduledEventUser> GetGuildScheduledEventUsersBeforeAsync(Snowflake scheduledEventId, Snowflake userId, bool guildUsers = false, RequestProperties? properties = null) => _client.GetGuildScheduledEventUsersAfterAsync(Id, scheduledEventId, userId, guildUsers, properties);
    #endregion

    #region GuildTemplate
    public Task<IEnumerable<GuildTemplate>> GetGuildTemplatesAsync(RequestProperties? properties = null) => _client.GetGuildTemplatesAsync(Id, properties);
    public Task<GuildTemplate> CreateGuildTemplateAsync(GuildTemplateProperties guildTemplateProperties, RequestProperties? properties = null) => _client.CreateGuildTemplateAsync(Id, guildTemplateProperties, properties);
    public Task<GuildTemplate> SyncGuildTemplateAsync(string templateCode, RequestProperties? properties = null) => _client.SyncGuildTemplateAsync(Id, templateCode, properties);
    public Task<GuildTemplate> ModifyGuildTemplateAsync(string templateCode, Action<GuildTemplateOptions> action, RequestProperties? properties = null) => _client.ModifyGuildTemplateAsync(Id, templateCode, action, properties);
    public Task<GuildTemplate> DeleteGuildTemplateAsync(string templateCode, RequestProperties? properties = null) => _client.DeleteGuildTemplateAsync(Id, templateCode, properties);
    #endregion

    #region Interactions.ApplicationCommands
    public Task<IReadOnlyDictionary<Snowflake, GuildApplicationCommand>> GetApplicationCommandsAsync(Snowflake applicationId, RequestProperties? properties = null) => _client.GetGuildApplicationCommandsAsync(applicationId, Id, properties);
    public Task<GuildApplicationCommand> CreateApplicationCommandAsync(Snowflake applicationId, ApplicationCommandProperties applicationCommandProperties, RequestProperties? properties = null) => _client.CreateGuildApplicationCommandAsync(applicationId, Id, applicationCommandProperties, properties);
    public Task<GuildApplicationCommand> GetApplicationCommandAsync(Snowflake applicationId, Snowflake commandId, RequestProperties? properties = null) => _client.GetGuildApplicationCommandAsync(applicationId, Id, commandId, properties);
    public Task<GuildApplicationCommand> ModifyApplicationCommandAsync(Snowflake applicationId, Snowflake commandId, Action<ApplicationCommandOptions> action, RequestProperties? properties = null) => _client.ModifyGuildApplicationCommandAsync(applicationId, Id, commandId, action, properties);
    public Task DeleteApplicationCommandAsync(Snowflake applicationId, Snowflake commandId, RequestProperties? properties = null) => _client.DeleteGuildApplicationCommandAsync(applicationId, Id, commandId, properties);
    public Task<IReadOnlyDictionary<Snowflake, GuildApplicationCommand>> BulkOverwriteApplicationCommandsAsync(Snowflake applicationId, IEnumerable<ApplicationCommandProperties> commands, RequestProperties? properties = null) => _client.BulkOverwriteGuildApplicationCommandsAsync(applicationId, Id, commands, properties);
    public Task<IReadOnlyDictionary<Snowflake, ApplicationCommandGuildPermissions>> GetApplicationCommandsPermissionsAsync(Snowflake applicationId, RequestProperties? properties = null) => _client.GetGuildApplicationCommandsPermissionsAsync(applicationId, Id, properties);
    public Task<ApplicationCommandGuildPermissions> GetApplicationCommandPermissionsAsync(Snowflake applicationId, Snowflake commandId, RequestProperties? properties = null) => _client.GetApplicationCommandGuildPermissionsAsync(applicationId, Id, commandId, properties);
    public Task<ApplicationCommandGuildPermissions> OverwriteApplicationCommandPermissionsAsync(Snowflake applicationId, Snowflake commandId, IEnumerable<ApplicationCommandGuildPermissionProperties> newPermissions, RequestProperties? properties = null) => _client.OverwriteApplicationCommandGuildPermissionsAsync(applicationId, Id, commandId, newPermissions, properties);
    #endregion

    #region Sticker
    public Task<IReadOnlyDictionary<Snowflake, GuildSticker>> GetStickersAsync(RequestProperties? properties = null) => _client.GetGuildStickersAsync(Id, properties);
    public Task<GuildSticker> GetStickerAsync(Snowflake stickerId, RequestProperties? properties = null) => _client.GetGuildStickerAsync(Id, stickerId, properties);
    public Task<GuildSticker> CreateStickerAsync(GuildStickerProperties sticker, RequestProperties? properties = null) => _client.CreateGuildStickerAsync(Id, sticker, properties);
    public Task<GuildSticker> ModifyStickerAsync(Snowflake stickerId, Action<GuildStickerOptions> action, RequestProperties? properties = null) => _client.ModifyGuildStickerAsync(Id, stickerId, action, properties);
    public Task DeleteStickerAsync(Snowflake stickerId, RequestProperties? properties = null) => _client.DeleteGuildStickerAsync(Id, stickerId, properties);
    #endregion

    #region User
    public Task LeaveAsync(RequestProperties? properties = null) => _client.LeaveGuildAsync(Id, properties);
    #endregion

    #region Webhook
    public Task<IReadOnlyDictionary<Snowflake, Webhook>> GetWebhooksAsync(RequestProperties? properties = null) => _client.GetGuildWebhooksAsync(Id, properties);
    #endregion
}