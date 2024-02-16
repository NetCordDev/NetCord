using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class PartialGuildUser(JsonGuildUser jsonModel, RestClient client) : User(jsonModel.User, client), IJsonModel<JsonGuildUser>
{
    JsonGuildUser IJsonModel<JsonGuildUser>.JsonModel => _jsonModel;
    private protected new readonly JsonGuildUser _jsonModel = jsonModel;

    public string? Nickname => _jsonModel.Nickname;
    public string? GuildAvatarHash => _jsonModel.GuildAvatarHash;
    public IReadOnlyList<ulong> RoleIds => _jsonModel.RoleIds;
    public ulong? HoistedRoleId => _jsonModel.HoistedRoleId;
    public DateTimeOffset JoinedAt => _jsonModel.JoinedAt;
    public DateTimeOffset? GuildBoostStart => _jsonModel.GuildBoostStart;
    public bool Deafened => _jsonModel.Deafened;
    public bool Muted => _jsonModel.Muted;
    public GuildUserFlags GuildFlags => _jsonModel.GuildFlags;
    public bool? IsPending => _jsonModel.IsPending;
    public DateTimeOffset? TimeOutUntil => _jsonModel.TimeOutUntil;

    public bool HasGuildAvatar => GuildAvatarHash is not null;
}
