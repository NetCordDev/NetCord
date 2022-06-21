using System.Globalization;

using NetCord.Rest;

namespace NetCord;

public class GuildUser : User
{
    private protected new readonly JsonModels.JsonGuildUser _jsonModel;

    public override Snowflake Id => _jsonModel.User.Id;
    public override string Username => _jsonModel.User.Username;
    public override ushort Discriminator => _jsonModel.User.Discriminator;
    public override string? AvatarHash => _jsonModel.User.AvatarHash;
    public override bool IsBot => _jsonModel.User.IsBot;
    public override bool? IsSystemUser => _jsonModel.User.IsSystemUser;
    public override bool? MFAEnabled => _jsonModel.User.MFAEnabled;
    public override CultureInfo? Locale => _jsonModel.User.Locale;
    public override bool? Verified => _jsonModel.User.Verified;
    public override string? Email => _jsonModel.User.Email;
    public override UserFlags? Flags => _jsonModel.User.Flags;
    public override PremiumType? PremiumType => _jsonModel.User.PremiumType;
    public override UserFlags? PublicFlags => _jsonModel.User.PublicFlags;
    public string? Nickname => _jsonModel.Nickname;
    public string? GuildAvatarHash => _jsonModel.GuildAvatarHash;
    public IEnumerable<Snowflake> RoleIds => _jsonModel.RoleIds;
    public Snowflake? HoistedRoleId => _jsonModel.HoistedRoleId;
    public DateTimeOffset JoinedAt => _jsonModel.JoinedAt;
    public DateTimeOffset? GuildBoostStart => _jsonModel.GuildBoostStart;
    public bool Deafened => _jsonModel.Deafened;
    public bool Muted => _jsonModel.Muted;
    public bool? IsPending => _jsonModel.IsPending;
    public DateTimeOffset? TimeOutUntil => _jsonModel.TimeOutUntil;
    public Snowflake GuildId { get; }

    public GuildUser(JsonModels.JsonGuildUser jsonModel, Snowflake guildId, RestClient client) : base(jsonModel.User, client)
    {
        _jsonModel = jsonModel;
        GuildId = guildId;
    }

    public IEnumerable<GuildRole> GetRoles(RestGuild guild)
    {
        var roles = guild.Roles;
        return RoleIds.Select(r => roles[r]);
    }

    public Task<GuildUser> ModifyAsync(Action<GuildUserProperties> func, RequestProperties? options = null)
        => _client.ModifyGuildUserAsync(GuildId, Id, func, options);

    public Task KickAsync(RequestProperties? options = null) => _client.KickGuildUserAsync(GuildId, Id, options);

    public Task BanAsync(RequestProperties? options = null) => _client.BanGuildUserAsync(GuildId, Id, options);
    public Task BanAsync(int deleteMessageDays, RequestProperties? options = null) => _client.BanGuildUserAsync(GuildId, Id, deleteMessageDays, options);

    public Task UnbanAsync(RequestProperties? options = null) => _client.UnbanGuildUserAsync(GuildId, Id, options);

    public bool HasGuildAvatar => GuildAvatarHash != null;

    public ImageUrl GetGuildAvatarUrl(ImageFormat? format = null) => ImageUrl.GuildUserAvatar(GuildId, Id, GuildAvatarHash!, format);

    public Task AddRoleAsync(Snowflake roleId, RequestProperties? options = null) => _client.AddGuildUserRoleAsync(GuildId, Id, roleId, options);

    public Task RemoveRoleAsync(Snowflake roleId, RequestProperties? options = null) => _client.RemoveGuildUserRoleAsync(GuildId, Id, roleId, options);
}