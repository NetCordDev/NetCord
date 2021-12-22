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
    public string? Permissions => _jsonGuildEntity.Permissions;
    public DateTimeOffset? TimeOutUntil => _jsonGuildEntity.TimeOutUntil;

    public Guild Guild { get; }

    internal GuildUser(JsonModels.JsonGuildUser jsonEntity, Guild guild, BotClient client) : base(jsonEntity.User, client)
    {
        _jsonGuildEntity = jsonEntity;
        Guild = guild;
    }

    public Task<GuildUser> ModifyAsync(Action<GuildUserProperties> func, RequestOptions? options = null)
        => _client.Rest.Guild.User.ModifyAsync(Guild, Id, func, options);

    public Task KickAsync(RequestOptions? options = null) => Guild.KickUserAsync(Id, options);

    public Task BanAsync(RequestOptions? options = null) => Guild.BanUserAsync(Id, options);
    public Task BanAsync(int deleteMessageDays, RequestOptions? options = null) => Guild.BanUserAsync(Id, deleteMessageDays, options);

    public Task UnbanAsync(RequestOptions? options = null) => Guild.UnbanUserAsync(Id, options);

    public bool HasGuildAvatar => GuildAvatarHash != null;

    public string GetGuildAvatarUrl(ImageFormat? format = null) => _client.Rest.Guild.User.GetGuildAvatarUrl(Guild.Id, Id, GuildAvatarHash!, format);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    /// <param name="size">any power of two between 16 and 4096</param>
    /// <returns></returns>
    public string GetGuildAvatarUrl(int size, ImageFormat? format = null) => _client.Rest.Guild.User.GetGuildAvatarUrl(Guild.Id, Id, GuildAvatarHash!, format, size);

    public Task AddRoleAsync(DiscordId roleId, RequestOptions? options = null) => _client.Rest.Guild.User.AddRoleAsync(Guild, Id, roleId, options);

    public Task RemoveRoleAsync(DiscordId roleId, RequestOptions? options = null) => _client.Rest.Guild.User.AddRoleAsync(Guild, Id, roleId, options);
}