namespace NetCord;
public class User : ClientEntity
{
    private protected readonly JsonModels.JsonUser _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;
    public virtual string Username => _jsonEntity.Username;
    public virtual ushort Discriminator => _jsonEntity.Discriminator;
    public virtual string? AvatarHash => _jsonEntity.AvatarHash;
    public virtual bool IsBot => _jsonEntity.IsBot;
    public virtual bool? IsSystemUser => _jsonEntity.IsSystemUser;
    public virtual bool? MFAEnabled => _jsonEntity.MFAEnabled;
    public virtual string? BannerHash => _jsonEntity.BannerHash;
    public virtual Color? AccentColor => _jsonEntity.AccentColor;
    public virtual string? Locale => _jsonEntity.Locale;
    public virtual bool? Verified => _jsonEntity.Verified;
    public virtual string? Email => _jsonEntity.Email;
    public virtual UserFlags? Flags => _jsonEntity.Flags;
    public virtual PremiumType? PremiumType => _jsonEntity.PremiumType;
    public virtual UserFlags? PublicFlags => _jsonEntity.PublicFlags;

    internal User(JsonModels.JsonUser jsonEntity, RestClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
    }

    public bool HasAvatar => AvatarHash != null;

    /// <param name="format"></param>
    /// <param name="size">Any power of 2 between 16 and 4096.</param>
    public string GetAvatarUrl(ImageFormat? format = null, int? size = null) => CDN.GetAvatarUrl(Id, AvatarHash!, format, size);

    public string DefaultAvatarUrl => CDN.GetDefaultAvatarUrl(Discriminator);

    public Task<DMChannel> GetDMChannelAsync() => _client.GetDMChannelByUserIdAsync(Id);

    public override string ToString() => $"<@{Id}>";
}