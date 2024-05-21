using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a user of any interactable resource on Discord.
/// </summary>
/// <remarks>
/// Users in Discord are generally considered the base entity and can be members of guilds, participate in text and voice chat, and much more. Users are separated by a distinction of 'bot' vs 'normal'. Bot users are automated users that are 'owned' by another user.
/// </remarks>
public partial class User(JsonModels.JsonUser jsonModel, RestClient client) : ClientEntity(client), IJsonModel<JsonModels.JsonUser>
{
    JsonModels.JsonUser IJsonModel<JsonModels.JsonUser>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonUser _jsonModel = jsonModel;

    /// <summary>
    /// The user's ID.<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// The user's username, not unique across the platform. Restrictions:
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///         Must be between 2 and 32 characters long.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///         Usernames cannot contain the following substrings: <c>@</c>, <c>#</c>, <c>:</c>, <c>```</c>, <c>discord</c>.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///         Usernames cannot be: <c>everyone</c> or <c>here</c>.
    ///         </description>
    ///     </item>
    /// </list>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public string Username => _jsonModel.Username;

    /// <summary>
    /// The user's Discord-tag.<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public ushort Discriminator => _jsonModel.Discriminator;

    /// <summary>
    /// The user's display name, if it is set. For bots, this is the application name. Restrictions:
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///         Must be between 1 and 32 characters long.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///         Some zero-width and non-rendering characters are limited.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///         Names are sanitized and trimmed of leading, trailing, and excessive internal whitespace.
    ///         </description>
    ///     </item>
    /// </list>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public string? GlobalName => _jsonModel.GlobalName;

    /// <summary>
    /// The user's avatar hash.<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public string? AvatarHash => _jsonModel.AvatarHash;

    /// <summary>
    /// Whether the user belongs to an OAuth2 application.<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public bool IsBot => _jsonModel.IsBot;

    /// <summary>
    /// Whether the user is an Official Discord System user (part of the urgent message system).<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public bool? IsSystemUser => _jsonModel.IsSystemUser;

    /// <summary>
    /// Whether the user has two factor enabled on their account.<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public bool? MfaEnabled => _jsonModel.MfaEnabled;

    /// <summary>
    /// The user's banner hash.<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public string? BannerHash => _jsonModel.BannerHash;

    /// <summary>
    /// The user's banner color encoded as an integer representation of hexadecimal color code.<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public Color? AccentColor => _jsonModel.AccentColor;

    /// <summary>
    /// The user's chosen language option.<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public string? Locale => _jsonModel.Locale;

    /// <summary>
    /// Whether the email on this account has been verified.<br/>
    /// Requires the <c>email</c> OAuth2 scope.
    /// </summary>
    public bool? Verified => _jsonModel.Verified;

    /// <summary>
    /// The user's email.<br/>
    /// Requires the <c>email</c> OAuth2 scope.
    /// </summary>
    public string? Email => _jsonModel.Email;

    /// <summary>
    /// The flags on a user's account.<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public UserFlags? Flags => _jsonModel.Flags;

    /// <summary>
    /// The type of Nitro subscription on a user's account.<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public PremiumType? PremiumType => _jsonModel.PremiumType;

    /// <summary>
    /// The public flags on a user's account.<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public UserFlags? PublicFlags => _jsonModel.PublicFlags;

    /// <summary>
    /// The user's avatar decoration hash.<br/>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </summary>
    public string? AvatarDecorationHash => _jsonModel.AvatarDecorationHash;

    /// <summary>
    /// Whether the user has a set custom avatar.
    /// </summary>
    public bool HasAvatar => AvatarHash is not null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the user's avatar.
    /// </summary>
    /// <param name="format"> The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated avatars) if <see langword="null"/>. </param>
    /// <returns> An <see cref="ImageUrl"/> pointing to the user's avatar. If the user does not have one set, the <see cref="ImageUrl"/> will be invalid. </returns>
    public ImageUrl GetAvatarUrl(ImageFormat? format = null) => ImageUrl.UserAvatar(Id, AvatarHash!, format);

    /// <summary>
    /// Whether the user has a set custom banner image.
    /// </summary>
    public bool HasBanner => BannerHash is not null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the user's banner.
    /// </summary>
    /// <param name="format"> The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated banners) if <see langword="null"/>. </param>
    /// <returns> An <see cref="ImageUrl"/> pointing to the user's banner. If the user does not have one set, the <see cref="ImageUrl"/> will be invalid. </returns>
    public ImageUrl GetBannerUrl(ImageFormat? format = null) => ImageUrl.UserBanner(Id, BannerHash!, format);

    /// <summary>
    /// Whether the user has a set avatar decoration.
    /// </summary>
    public bool HasAvatarDecoration => AvatarDecorationHash is not null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the user's avatar decoration URL.
    /// </summary>
    /// <returns> An <see cref="ImageUrl"/> pointing to the user's avatar decoration. If the user does not have one set, the <see cref="ImageUrl"/> will be invalid. </returns>
    public ImageUrl GetAvatarDecorationUrl() => ImageUrl.UserAvatarDecoration(Id, AvatarDecorationHash!);

    /// <summary>
    /// Returns an <see cref="ImageUrl"/> object representing the user's default avatar.
    /// </summary>
    public ImageUrl DefaultAvatarUrl => Discriminator is 0 ? ImageUrl.DefaultUserAvatar(Id) : ImageUrl.DefaultUserAvatar(Discriminator);

    /// <summary>
    /// Converts the ID of this user into its string representation, using Discord's mention syntax.
    /// </summary>
    public override string ToString() => $"<@{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatUser(destination, out charsWritten, Id);
}
