using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a user of any interactable resource on Discord.
/// </summary>
/// <remarks>
/// Users in Discord are generally considered the base entity and can be members of guilds, participate in text and voice chat, and much more. Users are separated by a distinction of 'bot' vs 'normal'. Bot users are automated users that are 'owned' by another user.
/// </remarks>
public partial class User : ClientEntity, IJsonModel<JsonModels.JsonUser>
{
    JsonModels.JsonUser IJsonModel<JsonModels.JsonUser>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonUser _jsonModel;

    public User(JsonModels.JsonUser jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        var avatarDecorationData = jsonModel.AvatarDecorationData;
        if (avatarDecorationData is not null)
            AvatarDecorationData = new(avatarDecorationData);
    }

    /// <summary>
    /// The user's ID.
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// The user's username, not unique across the platform. Restrictions:
    /// </summary>
    /// <remarks>
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
    /// </remarks>
    public string Username => _jsonModel.Username;

    /// <summary>
    /// The user's Discord-tag.
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public ushort Discriminator => _jsonModel.Discriminator;

    /// <summary>
    /// The user's display name, if it is set. For bots, this is the application name. Restrictions:
    /// </summary>
    /// <remarks>
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
    /// </remarks>
    public string? GlobalName => _jsonModel.GlobalName;

    /// <summary>
    /// The user's avatar hash.
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public string? AvatarHash => _jsonModel.AvatarHash;

    /// <summary>
    /// Whether the user belongs to an application.
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public bool IsBot => _jsonModel.IsBot;

    /// <summary>
    /// Whether the user is an Official Discord System user (part of the urgent message system).
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public bool? IsSystemUser => _jsonModel.IsSystemUser;

    /// <summary>
    /// Whether the user has two factor enabled on their account.
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public bool? MfaEnabled => _jsonModel.MfaEnabled;

    /// <summary>
    /// The user's banner hash.
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public string? BannerHash => _jsonModel.BannerHash;

    /// <summary>
    /// The user's banner color.
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public Color? AccentColor => _jsonModel.AccentColor;

    /// <summary>
    /// The user's chosen language option.
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public string? Locale => _jsonModel.Locale;

    /// <summary>
    /// Whether the email on this account has been verified.
    /// </summary>
    /// <remarks>
    /// Requires the <c>email</c> OAuth2 scope.
    /// </remarks>
    public bool? Verified => _jsonModel.Verified;

    /// <summary>
    /// The user's email.
    /// </summary>
    /// <remarks>
    /// Requires the <c>email</c> OAuth2 scope.
    /// </remarks>
    public string? Email => _jsonModel.Email;

    /// <summary>
    /// The flags on a user's account.
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public UserFlags? Flags => _jsonModel.Flags;

    /// <summary>
    /// The type of Nitro subscription on a user's account.
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public PremiumType? PremiumType => _jsonModel.PremiumType;

    /// <summary>
    /// The public flags on a user's account.
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public UserFlags? PublicFlags => _jsonModel.PublicFlags;

    /// <summary>
    /// Data for the user's avatar decoration.
    /// </summary>
    /// <remarks>
    /// Requires the <c>identify</c> OAuth2 scope.
    /// </remarks>
    public AvatarDecorationData? AvatarDecorationData { get; }

    /// <summary>
    /// Whether the user has a set custom avatar.
    /// </summary>
    public bool HasAvatar => AvatarHash is not null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the user's avatar.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated avatars).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the user's avatar. If the user does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetAvatarUrl(ImageFormat? format = null) => AvatarHash is string hash ? ImageUrl.UserAvatar(Id, hash, format) : null;

    /// <summary>
    /// Whether the user has a set custom banner image.
    /// </summary>
    public bool HasBanner => BannerHash is not null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the user's banner.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.Png"/> (or <see cref="ImageFormat.Gif"/> for animated banners).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the user's banner. If the user does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetBannerUrl(ImageFormat? format = null) => BannerHash is string hash ? ImageUrl.UserBanner(Id, hash, format) : null;

    /// <summary>
    /// Whether the user has a set avatar decoration.
    /// </summary>
    public bool HasAvatarDecoration => AvatarDecorationData is not null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the user's avatar decoration.
    /// </summary>
    /// <returns>An <see cref="ImageUrl"/> pointing to the user's avatar decoration. If the user does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetAvatarDecorationUrl() => AvatarDecorationData is { Hash: var hash } ? ImageUrl.AvatarDecoration(hash) : null;

    /// <summary>
    /// Returns an <see cref="ImageUrl"/> object representing the user's default avatar.
    /// </summary>
    public ImageUrl DefaultAvatarUrl => Discriminator is 0 ? ImageUrl.DefaultUserAvatar(Id) : ImageUrl.DefaultUserAvatar(Discriminator);

    /// <summary>
    /// Converts the ID of this user into its string representation, using Discord's mention syntax (<c>&lt;@803169206115237908&gt;</c>).
    /// </summary>
    public override string ToString() => $"<@{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatUser(destination, out charsWritten, Id);
}
