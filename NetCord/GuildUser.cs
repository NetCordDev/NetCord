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

    public Task<GuildUser> ModifyAsync(Action<GuildUserProperties> func, RequestOptions? options = null)
        => _client.Guild.User.ModifyAsync(GuildId, Id, func, options);

    public Task KickAsync(RequestOptions? options = null) => _client.Guild.User.KickAsync(GuildId, Id, options);

    public Task BanAsync(RequestOptions? options = null) => _client.Guild.User.BanAsync(GuildId, Id, options);
    public Task BanAsync(int deleteMessageDays, RequestOptions? options = null) => _client.Guild.User.BanAsync(GuildId, Id, deleteMessageDays, options);

    public Task UnbanAsync(RequestOptions? options = null) => _client.Guild.User.UnbanAsync(GuildId, Id, options);

    public bool HasGuildAvatar => GuildAvatarHash != null;

    public string GetGuildAvatarUrl(ImageFormat? format = null) => CDN.GetGuildAvatarUrl(GuildId, Id, GuildAvatarHash!, format);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    /// <param name="size">any power of two between 16 and 4096</param>
    /// <returns></returns>
    public string GetGuildAvatarUrl(int size, ImageFormat? format = null) => CDN.GetGuildAvatarUrl(GuildId, Id, GuildAvatarHash!, format, size);

    public Task AddRoleAsync(DiscordId roleId, RequestOptions? options = null) => _client.Guild.User.AddRoleAsync(GuildId, Id, roleId, options);

    public Task RemoveRoleAsync(DiscordId roleId, RequestOptions? options = null) => _client.Guild.User.AddRoleAsync(GuildId, Id, roleId, options);
}