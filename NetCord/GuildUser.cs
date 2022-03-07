namespace NetCord;

public class GuildUser : User
{
    private readonly JsonModels.JsonGuildUser _jsonGuildEntity;

    public override DiscordId Id => _jsonGuildEntity.User.Id;
    public override string Username => _jsonGuildEntity.User.Username;
    public override ushort Discriminator => _jsonGuildEntity.User.Discriminator;
    public override string? AvatarHash => _jsonGuildEntity.User.AvatarHash;
    public override bool IsBot => _jsonGuildEntity.User.IsBot;
    public override bool? IsSystemUser => _jsonGuildEntity.User.IsSystemUser;
    public override bool? MFAEnabled => _jsonGuildEntity.User.MFAEnabled;
    public override string? Locale => _jsonGuildEntity.User.Locale;
    public override bool? Verified => _jsonGuildEntity.User.Verified;
    public override string? Email => _jsonGuildEntity.User.Email;
    public override UserFlags? Flags => _jsonGuildEntity.User.Flags;
    public override PremiumType? PremiumType => _jsonGuildEntity.User.PremiumType;
    public override UserFlags? PublicFlags => _jsonGuildEntity.User.PublicFlags;
    public string? Nickname => _jsonGuildEntity.Nickname;
    public string? GuildAvatarHash => _jsonGuildEntity.GuildAvatarHash;
    public IEnumerable<DiscordId> RolesIds => _jsonGuildEntity.RolesIds;
    public DiscordId? HoistedRoleId => _jsonGuildEntity.HoistedRoleId;
    public DateTimeOffset JoinedAt => _jsonGuildEntity.JoinedAt;
    public DateTimeOffset? GuildBoostStart => _jsonGuildEntity.GuildBoostStart;
    public bool Deafened => _jsonGuildEntity.Deafened;
    public bool Muted => _jsonGuildEntity.Muted;
    public bool? IsPending => _jsonGuildEntity.IsPending;
    public Permission? Permissions { get; }
    public DateTimeOffset? TimeOutUntil => _jsonGuildEntity.TimeOutUntil;
    public DiscordId GuildId { get; }

    internal GuildUser(JsonModels.JsonGuildUser jsonEntity, DiscordId guildId, RestClient client) : base(jsonEntity.User, client)
    {
        _jsonGuildEntity = jsonEntity;
        GuildId = guildId;
        if (jsonEntity.Permissions != null)
            Permissions = (Permission)ulong.Parse(jsonEntity.Permissions);
    }

    public IEnumerable<GuildRole> GetRoles(Guild guild)
    {
        var roles = guild.Roles;
        return RolesIds.Select(r => roles[r]);
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