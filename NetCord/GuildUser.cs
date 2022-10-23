using System.Globalization;

using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class GuildUser : User, IJsonModel<JsonGuildUser>
{
    JsonGuildUser IJsonModel<JsonGuildUser>.JsonModel => _jsonModel;
    private protected new readonly JsonGuildUser _jsonModel;

    public override ulong Id => _jsonModel.User.Id;
    public override string Username => _jsonModel.User.Username;
    public override ushort Discriminator => _jsonModel.User.Discriminator;
    public override string? AvatarHash => _jsonModel.User.AvatarHash;
    public override bool IsBot => _jsonModel.User.IsBot;
    public override bool? IsSystemUser => _jsonModel.User.IsSystemUser;
    public override bool? MfaEnabled => _jsonModel.User.MfaEnabled;
    public override CultureInfo? Locale => _jsonModel.User.Locale;
    public override bool? Verified => _jsonModel.User.Verified;
    public override string? Email => _jsonModel.User.Email;
    public override UserFlags? Flags => _jsonModel.User.Flags;
    public override PremiumType? PremiumType => _jsonModel.User.PremiumType;
    public override UserFlags? PublicFlags => _jsonModel.User.PublicFlags;
    public string? Nickname => _jsonModel.Nickname;
    public string? GuildAvatarHash => _jsonModel.GuildAvatarHash;
    public IReadOnlyList<ulong> RoleIds => _jsonModel.RoleIds;
    public ulong? HoistedRoleId => _jsonModel.HoistedRoleId;
    public DateTimeOffset JoinedAt => _jsonModel.JoinedAt;
    public DateTimeOffset? GuildBoostStart => _jsonModel.GuildBoostStart;
    public bool Deafened => _jsonModel.Deafened;
    public bool Muted => _jsonModel.Muted;
    public bool? IsPending => _jsonModel.IsPending;
    public DateTimeOffset? TimeOutUntil => _jsonModel.TimeOutUntil;
    public ulong GuildId { get; }

    public GuildUser(JsonGuildUser jsonModel, ulong guildId, RestClient client) : base(jsonModel.User, client)
    {
        _jsonModel = jsonModel;
        GuildId = guildId;
    }

    public IEnumerable<GuildRole> GetRoles(RestGuild guild)
    {
        var roles = guild.Roles;
        return RoleIds.Select(r => roles[r]);
    }

    public bool HasGuildAvatar => GuildAvatarHash != null;

    public ImageUrl GetGuildAvatarUrl(ImageFormat? format = null) => ImageUrl.GuildUserAvatar(GuildId, Id, GuildAvatarHash!, format);

    public Task<GuildUser> TimeOutAsync(DateTimeOffset until, RequestProperties? properties = null) => ModifyAsync(u => u.TimeOutUntil = until, properties);

    #region Guild
    public Task<GuildUser> ModifyAsync(Action<GuildUserOptions> action, RequestProperties? properties = null) => _client.ModifyGuildUserAsync(GuildId, Id, action, properties);
    public Task AddRoleAsync(ulong roleId, RequestProperties? properties = null) => _client.AddGuildUserRoleAsync(GuildId, Id, roleId, properties);
    public Task RemoveRoleAsync(ulong roleId, RequestProperties? properties = null) => _client.RemoveGuildUserRoleAsync(GuildId, Id, roleId, properties);
    public Task KickAsync(RequestProperties? properties = null) => _client.KickGuildUserAsync(GuildId, Id, properties);
    public Task BanAsync(int deleteMessageSeconds = 0, RequestProperties? properties = null) => _client.BanGuildUserAsync(GuildId, Id, deleteMessageSeconds, properties);
    public Task UnbanAsync(RequestProperties? properties = null) => _client.UnbanGuildUserAsync(GuildId, Id, properties);
    public Task ModifyVoiceStateAsync(ulong channelId, Action<VoiceStateOptions> action, RequestProperties? properties = null) => _client.ModifyGuildUserVoiceStateAsync(GuildId, channelId, Id, action, properties);
    #endregion
}
