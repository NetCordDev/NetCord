using NetCord.Rest;

namespace NetCord;

/// <summary>
/// 
/// </summary>
public partial class User(JsonModels.JsonUser jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonModels.JsonUser>
{
    JsonModels.JsonUser IJsonModel<JsonModels.JsonUser>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonUser _jsonModel = jsonModel;

    /// <summary>
    /// The user's ID. Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// The user's username, not unique across the platform.
    /// </summary>
    public string Username => _jsonModel.Username;

    /// <summary>
    /// The user's Discord-tag.
    /// </summary>
    public ushort Discriminator => _jsonModel.Discriminator;

    /// <summary>
    /// The user's display name, if it is set. For bots, this is the application name.
    /// </summary>
    public string? GlobalName => _jsonModel.GlobalName;

    /// <summary>
    /// The user's avatar hash.
    /// </summary>
    public string? AvatarHash => _jsonModel.AvatarHash;

    /// <summary>
    /// Whether the user belongs to an OAuth2 application.
    /// </summary>
    public bool IsBot => _jsonModel.IsBot;

    /// <summary>
    /// Whether the user is an Official Discord System user (part of the urgent message system).
    /// </summary>
    public bool? IsSystemUser => _jsonModel.IsSystemUser;

    /// <summary>
    /// Whether the user has two factor enabled on their account.
    /// </summary>
    public bool? MfaEnabled => _jsonModel.MfaEnabled;

    /// <summary>
    /// The user's banner hash.
    /// </summary>
    public string? BannerHash => _jsonModel.BannerHash;

    /// <summary>
    /// The user's banner color encoded as an integer representation of hexadecimal color code.
    /// </summary>
    public Color? AccentColor => _jsonModel.AccentColor;

    /// <summary>
    /// The user's chosen language option.
    /// </summary>
    public string? Locale => _jsonModel.Locale;

    /// <summary>
    /// Whether the email on this account has been verified.
    /// </summary>
    public bool? Verified => _jsonModel.Verified;

    /// <summary>
    /// The user's email.
    /// </summary>
    public string? Email => _jsonModel.Email;

    /// <summary>
    /// The flags on a user's account.
    /// </summary>
    public UserFlags? Flags => _jsonModel.Flags;

    /// <summary>
    /// The type of Nitro subscription on a user's account.
    /// </summary>
    public PremiumType? PremiumType => _jsonModel.PremiumType;

    /// <summary>
    /// The public flags on a user's account.
    /// </summary>
    public UserFlags? PublicFlags => _jsonModel.PublicFlags;

    /// <summary>
    /// The user's avatar decoration hash.
    /// </summary>
    public string? AvatarDecorationHash => _jsonModel.AvatarDecorationHash;

    /// <summary>
    /// Whether the user has set a custom avatar.
    /// </summary>
    public bool HasAvatar => AvatarHash is not null;

    /// <summary>
    /// 
    /// </summary>
    public ImageUrl GetAvatarUrl(ImageFormat? format = null) => ImageUrl.UserAvatar(Id, AvatarHash!, format);

    /// <summary>
    /// 
    /// </summary>
    public bool HasBanner => BannerHash is not null;

    /// <summary>
    /// 
    /// </summary>
    public ImageUrl GetBannerUrl(ImageFormat? format = null) => ImageUrl.UserBanner(Id, BannerHash!, format);

    /// <summary>
    /// 
    /// </summary>
    public bool HasAvatarDecoration => AvatarDecorationHash is not null;

    /// <summary>
    /// 
    /// </summary>
    public ImageUrl GetAvatarDecorationUrl() => ImageUrl.UserAvatarDecoration(Id, AvatarDecorationHash!);

    /// <summary>
    /// 
    /// </summary>
    public ImageUrl DefaultAvatarUrl => Discriminator is 0 ? ImageUrl.DefaultUserAvatar(Id) : ImageUrl.DefaultUserAvatar(Discriminator);

    /// <summary>
    /// 
    /// </summary>
    public override string ToString() => $"<@{Id}>";

    /// <summary>
    /// 
    /// </summary>
    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatUser(destination, out charsWritten, Id);
}
