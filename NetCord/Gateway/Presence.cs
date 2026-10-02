using NetCord.Gateway.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class Presence(JsonPresence jsonModel, ulong guildId, RestClient client)
{
    public PresenceUser User { get; } = new(jsonModel.User, client);

    public ulong GuildId { get; } = guildId;

    public UserStatusType? Status { get; } = jsonModel.Status;

    public IReadOnlyList<UserActivity>? Activities { get; } = jsonModel.Activities?.Select(a => new UserActivity(a, guildId, client)).ToArray();

    public IReadOnlyDictionary<Platform, UserStatusType>? Platform { get; } = jsonModel.ClientStatus;
}

/// <summary>
/// Represents a user of any interactable resource on Discord.
/// </summary>
/// <remarks>
/// Users in Discord are generally considered the base entity and can be members of guilds, participate in text and voice chat, and much more. Users are separated by a distinction of 'bot' vs 'normal'. Bot users are automated users that are 'owned' by another user.
/// </remarks>
public partial class PresenceUser(JsonPresenceUser jsonModel, RestClient client) : ClientEntity(client)
{
    /// <inheritdoc cref="User.Id" />
    public override ulong Id { get; } = jsonModel.Id;

    /// <inheritdoc cref="User.Username" />
    public string? Username { get; } = jsonModel.Username;

    /// <inheritdoc cref="User.Discriminator" />
    public ushort? Discriminator { get; } = jsonModel.Discriminator;

    /// <inheritdoc cref="User.GlobalName" />
    public string? GlobalName { get; } = jsonModel.GlobalName;

    /// <inheritdoc cref="User.AvatarHash" />
    public string? AvatarHash { get; } = jsonModel.AvatarHash;

    /// <inheritdoc cref="User.IsBot" />
    public bool? IsBot { get; } = jsonModel.IsBot;

    /// <inheritdoc cref="User.IsSystemUser" />
    public bool? IsSystemUser { get; } = jsonModel.IsSystemUser;

    /// <inheritdoc cref="User.MfaEnabled" />
    public bool? MfaEnabled { get; } = jsonModel.MfaEnabled;

    /// <inheritdoc cref="User.BannerHash" />
    public string? BannerHash { get; } = jsonModel.BannerHash;

    /// <inheritdoc cref="User.AccentColor" />
    public Color? AccentColor { get; } = jsonModel.AccentColor;

    /// <inheritdoc cref="User.Locale" />
    public string? Locale { get; } = jsonModel.Locale;

    /// <inheritdoc cref="User.Verified" />
    public bool? Verified { get; } = jsonModel.Verified;

    /// <inheritdoc cref="User.Email" />
    public string? Email { get; } = jsonModel.Email;

    /// <inheritdoc cref="User.Flags" />
    public UserFlags? Flags { get; } = jsonModel.Flags;

    /// <inheritdoc cref="User.PremiumType" />
    public PremiumType? PremiumType { get; } = jsonModel.PremiumType;

    /// <inheritdoc cref="User.PublicFlags" />
    public UserFlags? PublicFlags { get; } = jsonModel.PublicFlags;

    /// <inheritdoc cref="User.AvatarDecorationData" />
    public AvatarDecorationData? AvatarDecorationData { get; } = jsonModel.AvatarDecorationData is { } avatarDecorationData ? new(avatarDecorationData) : null;

    /// <inheritdoc cref="User.Collectibles" />
    public Collectibles? Collectibles { get; } = jsonModel.Collectibles is { } collectibles ? new(collectibles) : null;

    /// <inheritdoc cref="User.PrimaryGuild" />
    public UserPrimaryGuild? PrimaryGuild { get; } = jsonModel.PrimaryGuild is { } primaryGuild ? new(primaryGuild) : null;

    /// <inheritdoc cref="User.GetAvatarUrl(ImageFormat?)" />
    public ImageUrl? GetAvatarUrl(ImageFormat? format = null) => AvatarHash is string hash ? ImageUrl.UserAvatar(Id, hash, format) : null;

    /// <inheritdoc cref="User.GetBannerUrl(ImageFormat?)" />
    public ImageUrl? GetBannerUrl(ImageFormat? format = null) => BannerHash is string hash ? ImageUrl.UserBanner(Id, hash, format) : null;

    /// <inheritdoc cref="User.GetAvatarDecorationUrl()" />
    public ImageUrl? GetAvatarDecorationUrl() => AvatarDecorationData is { Hash: var hash } ? ImageUrl.AvatarDecoration(hash) : null;

    // No DefaultAvatarUrl because Discriminator may be null

    /// <inheritdoc cref="User.ToString()" />
    public override string ToString() => $"<@{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatUser(destination, out charsWritten, Id);
}
