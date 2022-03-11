namespace NetCord;

public class GuildUser : User
{
    private protected readonly new JsonModels.JsonGuildUser _jsonEntity;

    public override DiscordId Id => _jsonEntity.User.Id;
    public override string Username => _jsonEntity.User.Username;
    public override ushort Discriminator => _jsonEntity.User.Discriminator;
    public override string? AvatarHash => _jsonEntity.User.AvatarHash;
    public override bool IsBot => _jsonEntity.User.IsBot;
    public override bool? IsSystemUser => _jsonEntity.User.IsSystemUser;
    public override bool? MFAEnabled => _jsonEntity.User.MFAEnabled;
    public override string? Locale => _jsonEntity.User.Locale;
    public override bool? Verified => _jsonEntity.User.Verified;
    public override string? Email => _jsonEntity.User.Email;
    public override UserFlags? Flags => _jsonEntity.User.Flags;
    public override PremiumType? PremiumType => _jsonEntity.User.PremiumType;
    public override UserFlags? PublicFlags => _jsonEntity.User.PublicFlags;
    public string? Nickname => _jsonEntity.Nickname;
    public string? GuildAvatarHash => _jsonEntity.GuildAvatarHash;
    public IEnumerable<DiscordId> RoleIds => _jsonEntity.RoleIds;
    public DiscordId? HoistedRoleId => _jsonEntity.HoistedRoleId;
    public DateTimeOffset JoinedAt => _jsonEntity.JoinedAt;
    public DateTimeOffset? GuildBoostStart => _jsonEntity.GuildBoostStart;
    public bool Deafened => _jsonEntity.Deafened;
    public bool Muted => _jsonEntity.Muted;
    public bool? IsPending => _jsonEntity.IsPending;
    public DateTimeOffset? TimeOutUntil => _jsonEntity.TimeOutUntil;
    public DiscordId GuildId { get; }

    internal GuildUser(JsonModels.JsonGuildUser jsonEntity, DiscordId guildId, RestClient client) : base(jsonEntity.User, client)
    {
        _jsonEntity = jsonEntity;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    /// <param name="size">Any power of 2 between 16 and 4096.</param>
    /// <returns></returns>
    public string GetGuildAvatarUrl(ImageFormat? format = null, int? size = null) => CDN.GetGuildAvatarUrl(GuildId, Id, GuildAvatarHash!, format, size);

    public Task AddRoleAsync(DiscordId roleId, RequestProperties? options = null) => _client.AddGuildUserRoleAsync(GuildId, Id, roleId, options);

    public Task RemoveRoleAsync(DiscordId roleId, RequestProperties? options = null) => _client.RemoveGuildUserRoleAsync(GuildId, Id, roleId, options);
}