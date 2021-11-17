namespace NetCord;

public class GuildUser : User
{
    private readonly JsonModels.JsonGuildUser _jsonGuildEntity;

    public override DiscordId Id => _jsonGuildEntity.User.Id;
    public override string Username => _jsonGuildEntity.User.Username;
    public override ushort Discriminator => _jsonGuildEntity.User.Discriminator;
    public override string? AvatarHash => _jsonGuildEntity.User.AvatarHash;
    public override bool IsBot => _jsonGuildEntity.User.IsBot;
    public override bool? IsOfficialDiscordUser => _jsonGuildEntity.User.IsOfficialDiscordUser;
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

    public Guild Guild { get; }

    internal GuildUser(JsonModels.JsonGuildUser jsonEntity, Guild guild, BotClient client) : base(jsonEntity.User, client)
    {
        _jsonGuildEntity = jsonEntity;
        Guild = guild;
    }

    public Task<GuildUser> ModifyAsync(Action<GuildUserProperties> func)
        => GuildUserHelper.ModifyUserAsync(_client, Guild, Id, func);

    public Task KickAsync() => Guild.KickUserAsync(Id);
    public Task KickAsync(string reason) => Guild.KickUserAsync(Id, reason);

    public Task BanAsync() => Guild.AddBanAsync(Id);
    public Task BanAsync(string reason) => Guild.AddBanAsync(Id, reason);
    public Task BanAsync(int deleteMessageDays, string reason) => Guild.AddBanAsync(Id, deleteMessageDays, reason);

    public Task UnBanAsync() => Guild.RemoveBanAsync(Id);
    public Task UnBanAsync(string reason) => Guild.RemoveBanAsync(Id, reason);

    public bool HasGuildAvatar => GuildAvatarHash != null;

    public string GetGuildAvatarUrl(ImageFormat? format = null) => GuildUserHelper.GetGuildAvatarUrl(Guild.Id, Id, GuildAvatarHash, format);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    /// <param name="size">any power of two between 16 and 4096</param>
    /// <returns></returns>
    public string GetGuildAvatarUrl(int size, ImageFormat? format = null) => GuildUserHelper.GetGuildAvatarUrl(Guild.Id, Id, GuildAvatarHash, format, size);

    public Task AddRoleAsync(DiscordId roleId) => GuildUserHelper.AddUserRoleAsync(_client, Guild, Id, roleId);

    public Task AddRoleAsync(DiscordId roleId, string reason) => GuildUserHelper.AddUserRoleAsync(_client, Guild, Id, roleId, reason);

    public Task RemoveRoleAsync(DiscordId roleId) => GuildUserHelper.RemoveUserRoleAsync(_client, Guild, Id, roleId);

    public Task RemoveRoleAsync(DiscordId roleId, string reason) => GuildUserHelper.RemoveUserRoleAsync(_client, Guild, Id, roleId, reason);
}