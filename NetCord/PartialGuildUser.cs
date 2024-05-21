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
    /// This user's guild nickname.
    /// </summary>
    public string? Nickname => _jsonModel.Nickname;

    /// <summary>
    /// The user's guild avatar hash.
    /// </summary>
    public string? GuildAvatarHash => _jsonModel.GuildAvatarHash;

    /// <summary>
    /// Array of role object ids.
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
    /// When the user started boosting the guild.
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
    /// Guild member flags represented as a bit set, defaults to 0.
    /// </summary>
    public GuildUserFlags GuildFlags => _jsonModel.GuildFlags;

    /// <summary>
    /// Whether the user has not yet passed the guild's Membership Screening requirements.
    /// </summary>
    public bool? IsPending => _jsonModel.IsPending;

    /// <summary>
    /// When the user's timeout will expire and the user will be able to communicate in the guild again, null or a time in the past if the user is not timed out.
    /// </summary>
    public DateTimeOffset? TimeOutUntil => _jsonModel.TimeOutUntil;

    /// <summary>
    /// Whether the user has a set guild avatar.
    /// </summary>
    public bool HasGuildAvatar => GuildAvatarHash is not null;
}
