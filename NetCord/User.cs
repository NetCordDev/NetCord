using System.Globalization;

using NetCord.Rest;

namespace NetCord;

public class User : ClientEntity, IJsonModel<JsonModels.JsonUser>
{
    JsonModels.JsonUser IJsonModel<JsonModels.JsonUser>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonUser _jsonModel;

    public override Snowflake Id => _jsonModel.Id;
    public virtual string Username => _jsonModel.Username;
    public virtual ushort Discriminator => _jsonModel.Discriminator;
    public virtual string? AvatarHash => _jsonModel.AvatarHash;
    public virtual bool IsBot => _jsonModel.IsBot;
    public virtual bool? IsSystemUser => _jsonModel.IsSystemUser;
    public virtual bool? MFAEnabled => _jsonModel.MFAEnabled;
    public virtual string? BannerHash => _jsonModel.BannerHash;
    public virtual Color? AccentColor => _jsonModel.AccentColor;
    public virtual CultureInfo? Locale => _jsonModel.Locale;
    public virtual bool? Verified => _jsonModel.Verified;
    public virtual string? Email => _jsonModel.Email;
    public virtual UserFlags? Flags => _jsonModel.Flags;
    public virtual PremiumType? PremiumType => _jsonModel.PremiumType;
    public virtual UserFlags? PublicFlags => _jsonModel.PublicFlags;

    public User(JsonModels.JsonUser jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }

    public bool HasAvatar => AvatarHash != null;

    public ImageUrl GetAvatarUrl(ImageFormat? format = null) => ImageUrl.UserAvatar(Id, AvatarHash!, format);

    public bool HasBanner => BannerHash != null;

    public ImageUrl GetBannerUrl(ImageFormat? format = null) => ImageUrl.UserBanner(Id, BannerHash!, format);

    public ImageUrl DefaultAvatarUrl => ImageUrl.DefaultUserAvatar(Discriminator);

    public Task<DMChannel> GetDMChannelAsync() => _client.GetDMChannelAsync(Id);

    public override string ToString() => $"<@{Id}>";
}