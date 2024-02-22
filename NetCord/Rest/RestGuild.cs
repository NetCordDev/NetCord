using System.Collections.Immutable;

namespace NetCord.Rest;

public partial class RestGuild : ClientEntity, IJsonModel<NetCord.JsonModels.JsonGuild>, IComparer<PartialGuildUser>
{
    NetCord.JsonModels.JsonGuild IJsonModel<NetCord.JsonModels.JsonGuild>.JsonModel => _jsonModel;
    internal readonly NetCord.JsonModels.JsonGuild _jsonModel;

    public ImmutableDictionary<ulong, Role> Roles { get; set; }
    public ImmutableDictionary<ulong, GuildEmoji> Emojis { get; set; }
    public ImmutableDictionary<ulong, GuildSticker> Stickers { get; set; }

    public override ulong Id => _jsonModel.Id;
    public string Name => _jsonModel.Name;
    public string? IconHash => _jsonModel.IconHash;
    public string? SplashHash => _jsonModel.SplashHash;
    public string? DiscoverySplashHash => _jsonModel.DiscoverySplashHash;
    public virtual bool IsOwner => _jsonModel.IsOwner;
    public ulong OwnerId => _jsonModel.OwnerId;
    public Permissions? Permissions => _jsonModel.Permissions;
    public ulong? AfkChannelId => _jsonModel.AfkChannelId;
    public int AfkTimeout => _jsonModel.AfkTimeout;
    public bool? WidgetEnabled => _jsonModel.WidgetEnabled;
    public ulong? WidgetChannelId => _jsonModel.WidgetChannelId;
    public VerificationLevel VerificationLevel => _jsonModel.VerificationLevel;
    public DefaultMessageNotificationLevel DefaultMessageNotificationLevel => _jsonModel.DefaultMessageNotificationLevel;
    public ContentFilter ContentFilter => _jsonModel.ContentFilter;
    public Role EveryoneRole => Roles[Id];
    public IReadOnlyList<string> Features => _jsonModel.Features;
    public MfaLevel MfaLevel => _jsonModel.MfaLevel;
    public ulong? ApplicationId => _jsonModel.ApplicationId;
    public ulong? SystemChannelId => _jsonModel.SystemChannelId;
    public SystemChannelFlags SystemChannelFlags => _jsonModel.SystemChannelFlags;
    public ulong? RulesChannelId => _jsonModel.RulesChannelId;
    public int? MaxPresences => _jsonModel.MaxPresences;
    public int? MaxUsers => _jsonModel.MaxUsers;
    public string? VanityUrlCode => _jsonModel.VanityUrlCode;
    public string? Description => _jsonModel.Description;
    public string? BannerHash => _jsonModel.BannerHash;
    public int PremiumTier => _jsonModel.PremiumTier;
    public int? PremiumSubscriptionCount => _jsonModel.PremiumSubscriptionCount;
    public string PreferredLocale => _jsonModel.PreferredLocale;
    public ulong? PublicUpdatesChannelId => _jsonModel.PublicUpdatesChannelId;
    public int? MaxVideoChannelUsers => _jsonModel.MaxVideoChannelUsers;
    public int? MaxStageVideoChannelUsers => _jsonModel.MaxStageVideoChannelUsers;
    public int? ApproximateUserCount => _jsonModel.ApproximateUserCount;
    public int? ApproximatePresenceCount => _jsonModel.ApproximatePresenceCount;
    public GuildWelcomeScreen? WelcomeScreen { get; }
    public NsfwLevel NsfwLevel => _jsonModel.NsfwLevel;
    public bool PremiumProgressBarEnabled => _jsonModel.PremiumProgressBarEnabled;
    public ulong? SafetyAlertsChannelId => _jsonModel.SafetyAlertsChannelId;

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
}
