using System.Globalization;

namespace NetCord;
public class User : ClientEntity
{
    private protected readonly JsonModels.JsonUser _jsonEntity;

    public override Snowflake Id => _jsonEntity.Id;
    public virtual string Username => _jsonEntity.Username;
    public virtual ushort Discriminator => _jsonEntity.Discriminator;
    public virtual string? AvatarHash => _jsonEntity.AvatarHash;
    public virtual bool IsBot => _jsonEntity.IsBot;
    public virtual bool? IsSystemUser => _jsonEntity.IsSystemUser;
    public virtual bool? MFAEnabled => _jsonEntity.MFAEnabled;
    public virtual string? BannerHash => _jsonEntity.BannerHash;
    public virtual Color? AccentColor => _jsonEntity.AccentColor;
    public virtual CultureInfo? Locale => _jsonEntity.Locale;
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

    public ImageUrl GetAvatarUrl(ImageFormat? format = null) => ImageUrl.UserAvatar(Id, AvatarHash!, format);

    public bool HasBanner => BannerHash != null;

    public ImageUrl GetBannerUrl(ImageFormat? format = null) => ImageUrl.UserBanner(Id, BannerHash!, format);

    public ImageUrl DefaultAvatarUrl => ImageUrl.DefaultUserAvatar(Discriminator);

    public Task<DMChannel> GetDMChannelAsync() => _client.GetDMChannelAsync(Id);

    public override string ToString() => $"<@{Id}>";
}