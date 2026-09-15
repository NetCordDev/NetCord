using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a <see cref="GuildUser"/> object that lacks a <see cref="GuildUser.GuildId"/> field, as well as methods relying on it.
/// </summary>
public class PartialGuildUser(JsonGuildUser jsonModel, RestClient client) : User(jsonModel.User, client), IJsonModel<JsonGuildUser>
{
    JsonGuildUser IJsonModel<JsonGuildUser>.JsonModel => jsonModel;

    /// <summary>
    /// The user's guild nickname.
    /// </summary>
    public string? Nickname => jsonModel.Nickname;

    /// <summary>
    /// The user's guild avatar hash.
    /// </summary>
    public string? GuildAvatarHash => jsonModel.GuildAvatarHash;

    /// <summary>
    /// The user's guild banner hash.
    /// </summary>
    public string? GuildBannerHash => jsonModel.GuildBannerHash;

    /// <summary>
    /// A list of <see cref="ulong"/> IDs representing the user's current roles.
    /// </summary>
    public IReadOnlyList<ulong> RoleIds => jsonModel.RoleIds;

    /// <summary>
    /// When the user joined the guild.
    /// </summary>
    public DateTimeOffset? JoinedAt => jsonModel.JoinedAt;

    /// <summary>
    /// When the user started boosting the guild. <see langword="null"/> if the user has never boosted.
    /// </summary>
    public DateTimeOffset? GuildBoostStart => jsonModel.GuildBoostStart;

    /// <summary>
    /// Whether the user is deafened in voice channels.
    /// </summary>
    public bool Deafened => jsonModel.Deafened;

    /// <summary>
    /// Whether the user is muted in voice channels.
    /// </summary>
    public bool Muted => jsonModel.Muted;

    /// <summary>
    /// The user's current <see cref="GuildUserFlags"/>.
    /// </summary>
    public GuildUserFlags GuildFlags => jsonModel.GuildFlags;

    /// <summary>
    /// Whether the user has passed the guild's Membership Screening requirements.
    /// </summary>
    public bool? IsPending => jsonModel.IsPending;

    /// <summary>
    /// When the user's current timeout will expire, allowing them to communicate in the guild again. <see langword="null"/> or a time in the past if the user is not currently timed out.
    /// </summary>
    public DateTimeOffset? TimeOutUntil => jsonModel.TimeOutUntil;

    /// <summary>
    /// Data for the guild user's avatar decoration.
    /// </summary>
    public AvatarDecorationData? GuildAvatarDecorationData { get; } = jsonModel.GuildAvatarDecorationData is { } guildAvatarDecorationData ? new(guildAvatarDecorationData) : null;

    /// <summary>
    /// Whether the user has a guild avatar set.
    /// </summary>
    public bool HasGuildAvatar => GuildAvatarHash is not null;

    /// <summary>
    /// Whether the user has a guild banner set.
    /// </summary>
    public bool HasGuildBanner => GuildBannerHash is not null;

    /// <summary>
    /// Whether the user has a set avatar decoration.
    /// </summary>
    public bool HasGuildAvatarDecoration => GuildAvatarDecorationData is not null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the user's guild avatar decoration.
    /// </summary>
    /// <returns>An <see cref="ImageUrl"/> pointing to the user's guild avatar decoration. If the user does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetGuildAvatarDecorationUrl() => GuildAvatarDecorationData is { Hash: var hash } ? ImageUrl.AvatarDecoration(hash) : null;
}
