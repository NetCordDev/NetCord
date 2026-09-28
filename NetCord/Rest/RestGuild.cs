using System.Collections.Immutable;

using NetCord.Gateway;
using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

/// <summary>
/// Represents an isolated collection of users and channels, often referred to as a server in the UI.
/// </summary>
public partial class RestGuild(JsonRestGuild jsonModel, RestClient client, IDictionaryProvider dictionaryProvider) : PartialGuild(jsonModel, client), IComparer<PartialGuildUser>
{
    public RestGuild(JsonRestGuild jsonModel, RestClient client) : this(jsonModel, client, IDictionaryProvider.OfDictionary)
    {
    }

    public int Compare(PartialGuildUser? x, PartialGuildUser? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        if (x is null)
            return -1;

        if (y is null)
            return 1;

        var ownerId = OwnerId;

        if (x.Id == ownerId)
            return y.Id == ownerId ? 0 : 1;

        if (y.Id == ownerId)
            return -1;

        using var xRoleEnumerator = x.GetRoles(this).GetEnumerator();

        if (!xRoleEnumerator.MoveNext())
        {
            using var yRoleEnumerator = y.GetRoles(this).GetEnumerator();

            return yRoleEnumerator.MoveNext() ? -1 : 0;
        }

        var xPosition = xRoleEnumerator.Current.Position;

        while (xRoleEnumerator.MoveNext())
        {
            var currentPosition = xRoleEnumerator.Current.Position;

            if (currentPosition > xPosition)
                xPosition = currentPosition;
        }

        int result = 1;

        foreach (var role in y.GetRoles(this))
        {
            int comparisonResult = xPosition.CompareTo(role.Position);

            if (comparisonResult < 0)
                return -1;

            result = Math.Min(result, comparisonResult);
        }

        return result;
    }
    public override ulong Id { get; } = jsonModel.Id;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the <see cref="RestGuild"/>'s icon.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated icons).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild's icon. If the guild does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetIconUrl(ImageFormat? format = null) => IconHash is string hash ? ImageUrl.GuildIcon(Id, hash, format) : null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the <see cref="RestGuild"/>'s splash.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild's splash. If the guild does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetSplashUrl(ImageFormat format) => SplashHash is string hash ? ImageUrl.GuildSplash(Id, hash, format) : null;

    /// <summary>
    /// Whether the <see cref="RestGuild"/> has a set discovery splash.
    /// </summary>
    public bool HasDiscoverySplash => DiscoverySplashHash is not null;

    /// <summary>
    /// The <see cref="RestGuild"/>'s discovery splash hash.
    /// </summary>
    public string? DiscoverySplashHash { get; } = jsonModel.DiscoverySplashHash;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the <see cref="RestGuild"/>'s discovery splash.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild's discovery splash. If the guild does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetDiscoverySplashUrl(ImageFormat format) => DiscoverySplashHash is string hash ? ImageUrl.GuildDiscoverySplash(Id, hash, format) : null;

    /// <summary>
    /// <see langword="true"/> if the user is the owner of the <see cref="RestGuild"/>.
    /// </summary>
    public virtual bool IsOwner { get; } = jsonModel.IsOwner;

    /// <summary>
    /// The ID of the <see cref="RestGuild"/>'s owner.
    /// </summary>
    public ulong OwnerId { get; } = jsonModel.OwnerId;

    /// <summary>
    /// Total permissions for the user in the <see cref="RestGuild"/> (excludes overwrites and implicit permissions).
    /// </summary>
    /// <remarks>
    /// Only available in objects returned from <see cref="RestClient.GetCurrentUserGuildsAsync(GuildsPaginationProperties?, RestRequestProperties?)"/>.
    /// </remarks>
    public Permissions? Permissions { get; } = jsonModel.Permissions;

    /// <summary>
    /// ID of the <see cref="RestGuild"/>'s AFK channel.
    /// </summary>
    public ulong? AfkChannelId { get; } = jsonModel.AfkChannelId;

    /// <summary>
    /// How long in seconds to wait before moving users to the AFK channel.
    /// </summary>
    public int AfkTimeout { get; } = jsonModel.AfkTimeout;

    /// <summary>
    /// <see langword="true"/> if the <see cref="GuildWidget"/> is enabled.
    /// </summary>
    public bool? WidgetEnabled { get; } = jsonModel.WidgetEnabled;

    /// <summary>
    /// The ID of the channel that the <see cref="GuildWidget"/> will generate an invite to, or <see langword="null"/> if set to no invite.
    /// </summary>
    public ulong? WidgetChannelId { get; } = jsonModel.WidgetChannelId;

    public new VerificationLevel VerificationLevel => base.VerificationLevel.GetValueOrDefault();

    /// <summary>
    /// The <see cref="RestGuild"/>'s <see cref="NetCord.DefaultMessageNotificationLevel"/>.
    /// </summary>
    public DefaultMessageNotificationLevel DefaultMessageNotificationLevel { get; } = jsonModel.DefaultMessageNotificationLevel;

    /// <summary>
    /// The <see cref="RestGuild"/>'s <see cref="NetCord.ContentFilter"/>.
    /// </summary>
    public ContentFilter ContentFilter { get; } = jsonModel.ContentFilter;

    /// <summary>
    /// A dictionary of <see cref="Role"/> objects indexed by their IDs, representing the <see cref="RestGuild"/>'s roles.
    /// </summary>
    public IReadOnlyDictionary<ulong, Role> Roles { get; set; } = CreateRoles(jsonModel, client, dictionaryProvider);

    private static IReadOnlyDictionary<ulong, Role> CreateRoles(JsonRestGuild jsonModel, RestClient client, IDictionaryProvider dictionaryProvider)
    {
        var guildId = jsonModel.Id;
        return dictionaryProvider.CreateDictionary(jsonModel.Roles ?? [], r => r.Id, r => new Role(r, guildId, client));
    }

    /// <summary>
    /// A dictionary of <see cref="GuildEmoji"/> objects, indexed by their IDs, representing the <see cref="RestGuild"/>'s custom emojis.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildEmoji> Emojis { get; set; } = CreateEmojis(jsonModel, client, dictionaryProvider);

    private static IReadOnlyDictionary<ulong, GuildEmoji> CreateEmojis(JsonRestGuild jsonModel, RestClient client, IDictionaryProvider dictionaryProvider)
    {
        var guildId = jsonModel.Id;
        return dictionaryProvider.CreateDictionary(jsonModel.Emojis ?? [], e => e.Id.GetValueOrDefault(), e => new GuildEmoji(e, guildId, client));
    }

    /// <inheritdoc cref="PartialGuild.Features"/>
    public new IReadOnlyList<string> Features => base.Features!;

    /// <summary>
    /// The <see cref="RestGuild"/>'s required Multi-Factor Authentication level.
    /// </summary>
    public MfaLevel MfaLevel { get; } = jsonModel.MfaLevel;

    /// <summary>
    /// The <see cref="RestGuild"/> creator's application ID, if it was created by a bot.
    /// </summary>
    public ulong? ApplicationId { get; } = jsonModel.ApplicationId;

    /// <summary>
    /// The ID of the channel where <see cref="RestGuild"/> notices such as welcome messages and boost events are posted.
    /// </summary>
    public ulong? SystemChannelId { get; } = jsonModel.SystemChannelId;

    /// <summary>
    /// Represents the <see cref="RestGuild"/>'s current system channels settings.
    /// </summary>
    public SystemChannelFlags SystemChannelFlags { get; } = jsonModel.SystemChannelFlags;

    /// <summary>
    /// The ID of the channel where community guilds can display their rules and/or guidelines.
    /// </summary>
    public ulong? RulesChannelId { get; } = jsonModel.RulesChannelId;

    /// <summary>
    /// The maximum number of <see cref="Presence"/>s for the <see cref="RestGuild"/>. Always <see langword="null"/> with the exception of the largest guilds.
    /// </summary>
    public int? MaxPresences { get; } = jsonModel.MaxPresences;

    /// <summary>
    /// The maximum number of <see cref="GuildUser"/>s for the <see cref="RestGuild"/>.
    /// </summary>
    public int? MaxUsers { get; } = jsonModel.MaxUsers;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the <see cref="RestGuild"/>'s banner.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated icons).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the guild's banner. If the guild does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetBannerUrl(ImageFormat? format = null) => BannerHash is string hash ? ImageUrl.GuildBanner(Id, hash, format) : null;

    /// <summary>
    /// The <see cref="RestGuild"/>'s current server boost level.
    /// </summary>
    public int PremiumTier { get; } = jsonModel.PremiumTier;

    /// <summary>
    /// The number of boosts the <see cref="RestGuild"/> currently has.
    /// </summary>
    public int? PremiumSubscriptionCount { get; } = jsonModel.PremiumSubscriptionCount;

    /// <summary>
    /// The preferred locale of a community <see cref="RestGuild"/>, used for the 'Discovery' tab and in notices from Discord, also sent in interactions. Defaults to <c>en-US</c>.
    /// </summary>
    public string PreferredLocale { get; } = jsonModel.PreferredLocale;

    /// <summary>
    /// The ID of the channel where admins and moderators of community guilds receive notices from Discord.
    /// </summary>
    public ulong? PublicUpdatesChannelId { get; } = jsonModel.PublicUpdatesChannelId;

    /// <summary>
    /// The maximum amount of users in a video channel.
    /// </summary>
    public int? MaxVideoChannelUsers { get; } = jsonModel.MaxVideoChannelUsers;

    /// <summary>
    /// The maximum amount of users in a stage video channel.
    /// </summary>
    public int? MaxStageVideoChannelUsers { get; } = jsonModel.MaxStageVideoChannelUsers;

    /// <summary>
    /// The approximate number of <see cref="GuildUser"/>s in the <see cref="RestGuild"/>.
    /// </summary>
    /// <remarks>
    /// Only available in objects returned from <see cref="RestClient.GetGuildAsync(ulong, bool, RestRequestProperties?, CancellationToken)"/> and <see cref="RestClient.GetCurrentUserGuildsAsync(GuildsPaginationProperties?, RestRequestProperties?)"/>, where <c>withCounts</c> is true.
    /// </remarks>
    public int? ApproximateUserCount { get; } = jsonModel.ApproximateUserCount;

    /// <summary>
    /// Approximate number of non-offline <see cref="GuildUser"/>s in the <see cref="RestGuild"/>.
    /// </summary>
    /// <remarks>
    /// Only available in objects returned from <see cref="RestClient.GetGuildAsync(ulong, bool, RestRequestProperties?, CancellationToken)"/> and <see cref="RestClient.GetCurrentUserGuildsAsync(GuildsPaginationProperties?, RestRequestProperties?)"/>, where <c>withCounts</c> is true.
    /// </remarks>
    public int? ApproximatePresenceCount { get; } = jsonModel.ApproximatePresenceCount;

    /// <summary>
    /// The <see cref="RestGuild"/>'s set NSFW level.
    /// </summary>
    public NsfwLevel NsfwLevel { get; } = jsonModel.NsfwLevel;

    /// <summary>
    /// A dictionary of <see cref="GuildSticker"/> objects indexed by their IDs, representing the <see cref="RestGuild"/>'s custom stickers.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildSticker> Stickers { get; set; } = dictionaryProvider.CreateDictionary(jsonModel.Stickers ?? [], s => s.Id, s => new GuildSticker(s, client));

    /// <summary>
    /// Whether the <see cref="RestGuild"/> has the boost progress bar enabled.
    /// </summary>
    public bool PremiumProgressBarEnabled { get; } = jsonModel.PremiumProgressBarEnabled;

    /// <summary>
    /// The ID of the channel where admins and moderators of community guilds receive safety alerts from Discord.
    /// </summary>
    public ulong? SafetyAlertsChannelId { get; } = jsonModel.SafetyAlertsChannelId;

    /// <summary>
    /// The guild's base role, applied to all users.
    /// </summary>
    /// <returns>The guild's base role, applied to all users or <see langword="null"/> if the guild is partial.</returns>
    public Role? EveryoneRole => Roles.GetValueOrDefault(Id);

    /// <inheritdoc cref="ImageUrl.GuildWidget" />
    public ImageUrl GetWidgetUrl(GuildWidgetStyle? style = null, string hostname = Discord.RestHostname, ApiVersion? version = null) => ImageUrl.GuildWidget(Id, style, hostname, version);
}
