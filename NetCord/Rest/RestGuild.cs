using System.Collections.Immutable;

using NetCord.Gateway;

namespace NetCord.Rest;

/// <summary>
/// Represents an isolated collection of users and channels, often referred to as a server in the UI.
/// </summary>
public partial class RestGuild : ClientEntity, IJsonModel<NetCord.JsonModels.JsonGuild>, IComparer<PartialGuildUser>
{
    NetCord.JsonModels.JsonGuild IJsonModel<NetCord.JsonModels.JsonGuild>.JsonModel => _jsonModel;
    internal readonly NetCord.JsonModels.JsonGuild _jsonModel;

    public RestGuild(NetCord.JsonModels.JsonGuild jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
        Roles = jsonModel.Roles.ToImmutableDictionaryOrEmpty(r => new Role(r, Id, client));
        // guild emojis always have Id
        Emojis = jsonModel.Emojis.ToImmutableDictionaryOrEmpty(e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, Id, client));
        Stickers = jsonModel.Stickers.ToImmutableDictionaryOrEmpty(s => s.Id, s => new GuildSticker(s, client));

        var welcomeScreen = jsonModel.WelcomeScreen;
        if (welcomeScreen is not null)
            WelcomeScreen = new(welcomeScreen);
    }

    public int Compare(PartialGuildUser? x, PartialGuildUser? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (x is null)
            return -1;

        if (y is null)
            return 1;

        var (xId, yId) = (x.Id, y.Id);

        if (xId == yId)
            return 0;

        var ownerId = OwnerId;
        if (xId == ownerId)
            return 1;

        if (yId == ownerId)
            return -1;

        return GetHighestRolePosition(x).CompareTo(GetHighestRolePosition(y));

        int GetHighestRolePosition(PartialGuildUser user)
        {
            int highestPosition = 0;
            foreach (var role in user.GetRoles(this))
            {
                var position = role.Position;
                if (position > highestPosition)
                    highestPosition = position;
            }

            return highestPosition;
        }
    }

    /// <summary>
    /// The <see cref="RestGuild"/>'s ID.
    /// </summary>
    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// The name of the <see cref="RestGuild"/>. Must be between 2 and 100 characters. Leading and trailing whitespace are trimmed.
    /// </summary>
    public string Name => _jsonModel.Name;

    /// <summary>
    /// Whether the <see cref="RestGuild"/> has an icon set.
    /// </summary>
    public bool HasIcon => IconHash is not null;

    /// <summary>
    /// The <see cref="RestGuild"/>'s icon hash.
    /// </summary>
    public string? IconHash => _jsonModel.IconHash;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the <see cref="RestGuild"/>'s icon.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated icons).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild's icon. If the guild does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetIconUrl(ImageFormat? format = null) => IconHash is string hash ? ImageUrl.GuildIcon(Id, hash, format) : null;

    /// <summary>
    /// Whether the <see cref="RestGuild"/> has a set splash.
    /// </summary>
    public bool HasSplash => SplashHash is not null;

    /// <summary>
    /// The <see cref="RestGuild"/>'s splash hash.
    /// </summary>
    public string? SplashHash => _jsonModel.SplashHash;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the <see cref="RestGuild"/>'s splash.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild's splash. If the guild does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetSplashUrl(ImageFormat format = ImageFormat.Png) => SplashHash is string hash ? ImageUrl.GuildSplash(Id, hash, format) : null;

    /// <summary>
    /// Whether the <see cref="RestGuild"/> has a set discovery splash.
    /// </summary>
    public bool HasDiscoverySplash => DiscoverySplashHash is not null;

    /// <summary>
    /// The <see cref="RestGuild"/>'s discovery splash hash.
    /// </summary>
    public string? DiscoverySplashHash => _jsonModel.DiscoverySplashHash;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the <see cref="RestGuild"/>'s discovery splash.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild's discovery splash. If the guild does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetDiscoverySplashUrl(ImageFormat format = ImageFormat.Png) => DiscoverySplashHash is string hash ? ImageUrl.GuildDiscoverySplash(Id, hash, format) : null;

    /// <summary>
    /// <see langword="true"/> if the user is the owner of the <see cref="RestGuild"/>.
    /// </summary>
    public virtual bool IsOwner => _jsonModel.IsOwner;

    /// <summary>
    /// The ID of the <see cref="RestGuild"/>'s owner.
    /// </summary>
    public ulong OwnerId => _jsonModel.OwnerId;

    /// <summary>
    /// Total permissions for the user in the <see cref="RestGuild"/> (excludes overwrites and implicit permissions).
    /// </summary>
    /// <remarks>
    /// Only available in objects returned from <see cref="RestClient.GetCurrentUserGuildsAsync(GuildsPaginationProperties?, RestRequestProperties?)"/>.
    /// </remarks>
    public Permissions? Permissions => _jsonModel.Permissions;

    /// <summary>
    /// ID of the <see cref="RestGuild"/>'s AFK channel.
    /// </summary>
    public ulong? AfkChannelId => _jsonModel.AfkChannelId;

    /// <summary>
    /// How long in seconds to wait before moving users to the AFK channel.
    /// </summary>
    public int AfkTimeout => _jsonModel.AfkTimeout;

    /// <summary>
    /// <see langword="true"/> if the <see cref="GuildWidget"/> is enabled.
    /// </summary>
    public bool? WidgetEnabled => _jsonModel.WidgetEnabled;

    /// <summary>
    /// The ID of the channel that the <see cref="GuildWidget"/> will generate an invite to, or <see langword="null"/> if set to no invite.
    /// </summary>
    public ulong? WidgetChannelId => _jsonModel.WidgetChannelId;

    /// <summary>
    /// The <see cref="NetCord.VerificationLevel"/> required for the <see cref="RestGuild"/>.
    /// </summary>
    public VerificationLevel VerificationLevel => _jsonModel.VerificationLevel;

    /// <summary>
    /// The <see cref="RestGuild"/>'s <see cref="NetCord.DefaultMessageNotificationLevel"/>.
    /// </summary>
    public DefaultMessageNotificationLevel DefaultMessageNotificationLevel => _jsonModel.DefaultMessageNotificationLevel;

    /// <summary>
    /// The <see cref="RestGuild"/>'s <see cref="NetCord.ContentFilter"/>.
    /// </summary>
    public ContentFilter ContentFilter => _jsonModel.ContentFilter;

    /// <summary>
    /// A dictionary of <see cref="Role"/> objects indexed by their IDs, representing the <see cref="RestGuild"/>'s roles.
    /// </summary>
    public ImmutableDictionary<ulong, Role> Roles { get; set; }

    /// <summary>
    /// A dictionary of <see cref="GuildEmoji"/> objects, indexed by their IDs, representing the <see cref="RestGuild"/>'s custom emojis.
    /// </summary>
    public ImmutableDictionary<ulong, GuildEmoji> Emojis { get; set; }

    /// <summary>
    /// A list of <see cref="RestGuild"/> feature strings, representing what features are currently enabled.
    /// </summary>
    public IReadOnlyList<string> Features => _jsonModel.Features;

    /// <summary>
    /// The <see cref="RestGuild"/>'s required Multi-Factor Authentication level.
    /// </summary>
    public MfaLevel MfaLevel => _jsonModel.MfaLevel;

    /// <summary>
    /// The <see cref="RestGuild"/> creator's application ID, if it was created by a bot.
    /// </summary>
    public ulong? ApplicationId => _jsonModel.ApplicationId;

    /// <summary>
    /// The ID of the channel where <see cref="RestGuild"/> notices such as welcome messages and boost events are posted.
    /// </summary>
    public ulong? SystemChannelId => _jsonModel.SystemChannelId;

    /// <summary>
    /// Represents the <see cref="RestGuild"/>'s current system channels settings.
    /// </summary>
    public SystemChannelFlags SystemChannelFlags => _jsonModel.SystemChannelFlags;

    /// <summary>
    /// The ID of the channel where community guilds can display their rules and/or guidelines.
    /// </summary>
    public ulong? RulesChannelId => _jsonModel.RulesChannelId;

    /// <summary>
    /// The maximum number of <see cref="Presence"/>s for the <see cref="RestGuild"/>. Always <see langword="null"/> with the exception of the largest guilds.
    /// </summary>
    public int? MaxPresences => _jsonModel.MaxPresences;

    /// <summary>
    /// The maximum number of <see cref="GuildUser"/>s for the <see cref="RestGuild"/>.
    /// </summary>
    public int? MaxUsers => _jsonModel.MaxUsers;

    /// <summary>
    /// The <see cref="RestGuild"/>'s vanity invite URL code.
    /// </summary>
    public string? VanityUrlCode => _jsonModel.VanityUrlCode;

    /// <summary>
    /// The <see cref="RestGuild"/>'s description, shown in the 'Discovery' tab.
    /// </summary>
    public string? Description => _jsonModel.Description;

    /// <summary>
    /// Whether the <see cref="RestGuild"/> has a set banner.
    /// </summary>
    public bool HasBanner => BannerHash is not null;

    /// <summary>
    /// The <see cref="RestGuild"/>'s banner hash.
    /// </summary>
    public string? BannerHash => _jsonModel.BannerHash;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the <see cref="RestGuild"/>'s banner.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated icons).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild's banner. If the guild does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetBannerUrl(ImageFormat? format = null) => BannerHash is string hash ? ImageUrl.GuildBanner(Id, hash, format) : null;

    /// <summary>
    /// The <see cref="RestGuild"/>'s current server boost level.
    /// </summary>
    public int PremiumTier => _jsonModel.PremiumTier;

    /// <summary>
    /// The number of boosts the <see cref="RestGuild"/> currently has.
    /// </summary>
    public int? PremiumSubscriptionCount => _jsonModel.PremiumSubscriptionCount;

    /// <summary>
    /// The preferred locale of a community <see cref="RestGuild"/>, used for the 'Discovery' tab and in notices from Discord, also sent in interactions. Defaults to <c>en-US</c>.
    /// </summary>
    public string PreferredLocale => _jsonModel.PreferredLocale;

    /// <summary>
    /// The ID of the channel where admins and moderators of community guilds receive notices from Discord.
    /// </summary>
    public ulong? PublicUpdatesChannelId => _jsonModel.PublicUpdatesChannelId;

    /// <summary>
    /// The maximum amount of users in a video channel.
    /// </summary>
    public int? MaxVideoChannelUsers => _jsonModel.MaxVideoChannelUsers;

    /// <summary>
    /// The maximum amount of users in a stage video channel.
    /// </summary>
    public int? MaxStageVideoChannelUsers => _jsonModel.MaxStageVideoChannelUsers;

    /// <summary>
    /// The approximate number of <see cref="GuildUser"/>s in the <see cref="RestGuild"/>.
    /// </summary>
    /// <remarks>
    /// Only available in objects returned from <see cref="RestClient.GetGuildAsync(ulong, bool, RestRequestProperties?)"/> and <see cref="RestClient.GetCurrentUserGuildsAsync(GuildsPaginationProperties?, RestRequestProperties?)"/>, where <c>withCounts</c> is true.
    /// </remarks>
    public int? ApproximateUserCount => _jsonModel.ApproximateUserCount;

    /// <summary>
    /// Approximate number of non-offline <see cref="GuildUser"/>s in the <see cref="RestGuild"/>.
    /// </summary>
    /// <remarks>
    /// Only available in objects returned from <see cref="RestClient.GetGuildAsync(ulong, bool, RestRequestProperties?)"/> and <see cref="RestClient.GetCurrentUserGuildsAsync(GuildsPaginationProperties?, RestRequestProperties?)"/>, where <c>withCounts</c> is true.
    /// </remarks>
    public int? ApproximatePresenceCount => _jsonModel.ApproximatePresenceCount;

    /// <summary>
    /// The welcome screen shown to new members, returned in an invite's <see cref="RestGuild"/> object.
    /// </summary>
    public GuildWelcomeScreen? WelcomeScreen { get; }

    /// <summary>
    /// The <see cref="RestGuild"/>'s set NSFW level.
    /// </summary>
    public NsfwLevel NsfwLevel => _jsonModel.NsfwLevel;

    /// <summary>
    /// A dictionary of <see cref="GuildSticker"/> objects indexed by their IDs, representing the <see cref="RestGuild"/>'s custom stickers.
    /// </summary>
    public ImmutableDictionary<ulong, GuildSticker> Stickers { get; set; }

    /// <summary>
    /// Whether the <see cref="RestGuild"/> has the boost progress bar enabled.
    /// </summary>
    public bool PremiumProgressBarEnabled => _jsonModel.PremiumProgressBarEnabled;

    /// <summary>
    /// The ID of the channel where admins and moderators of community guilds receive safety alerts from Discord.
    /// </summary>
    public ulong? SafetyAlertsChannelId => _jsonModel.SafetyAlertsChannelId;

    /// <summary>
    /// The guild's base role, applied to all users.
    /// </summary>
    public Role EveryoneRole => Roles[Id];
}
