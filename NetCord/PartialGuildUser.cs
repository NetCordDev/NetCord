using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class PartialGuildUser : User, IJsonModel<JsonGuildUser>
{
    JsonGuildUser IJsonModel<JsonGuildUser>.JsonModel => _jsonModel;
    private protected new readonly JsonGuildUser _jsonModel;

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

    public PartialGuildUser(JsonGuildUser jsonModel, RestClient client) : base(jsonModel.User, client)
    {
        _jsonModel = jsonModel;
    }

    public IEnumerable<Role> GetRoles(RestGuild guild)
    {
        var roles = guild.Roles;
        return RoleIds.Select(r => roles[r]);
    }

    public bool HasGuildAvatar => GuildAvatarHash != null;
}
