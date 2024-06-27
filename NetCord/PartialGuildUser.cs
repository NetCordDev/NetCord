using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a <see cref="GuildUser"/> object that lacks a <see cref="GuildUser.GuildId"/> field, as well as methods relying on it.
/// </summary>
public class PartialGuildUser(JsonGuildUser jsonModel, RestClient client) : User(jsonModel.User, client), IJsonModel<JsonGuildUser>
{
    JsonGuildUser IJsonModel<JsonGuildUser>.JsonModel => _jsonModel;
    private protected new readonly JsonGuildUser _jsonModel = jsonModel;

    /// <summary>
    /// The user's guild nickname.
    /// </summary>
    public string? Nickname => _jsonModel.Nickname;

    /// <summary>
    /// The user's guild avatar hash.
    /// </summary>
    public string? GuildAvatarHash => _jsonModel.GuildAvatarHash;

    /// <summary>
    /// A list of <see cref="ulong"/> IDs representing the user's current roles.
    /// </summary>
    public IReadOnlyList<ulong> RoleIds => _jsonModel.RoleIds;

    /// <summary>
    /// The ID of the user's hoisted role, used to categorize the user in the member list.
    /// </summary>
    public ulong? HoistedRoleId => _jsonModel.HoistedRoleId;

    /// <summary>
    /// When the user joined the guild.
    /// </summary>
    public DateTimeOffset JoinedAt => _jsonModel.JoinedAt;

    /// <summary>
    /// When the user started boosting the guild. <see langword="null"/> if the user has never boosted.
    /// </summary>
    public DateTimeOffset? GuildBoostStart => _jsonModel.GuildBoostStart;

    /// <summary>
    /// Whether the user is deafened in voice channels.
    /// </summary>
    public bool Deafened => _jsonModel.Deafened;

    /// <summary>
    /// Whether the user is muted in voice channels.
    /// </summary>
    public bool Muted => _jsonModel.Muted;

    /// <summary>
    /// The user's current <see cref="GuildUserFlags"/>.
    /// </summary>
    public GuildUserFlags GuildFlags => _jsonModel.GuildFlags;

    /// <summary>
    /// Whether the user has passed the guild's Membership Screening requirements.
    /// </summary>
    public bool? IsPending => _jsonModel.IsPending;

    /// <summary>
    /// When the user's current timeout will expire, allowing them to communicate in the guild again. <see langword="null"/> or a time in the past if the user is not currently timed out.
    /// </summary>
    public DateTimeOffset? TimeOutUntil => _jsonModel.TimeOutUntil;

    /// <summary>
    /// Whether the user has a guild avatar set.
    /// </summary>
    public bool HasGuildAvatar => GuildAvatarHash is not null;
}
