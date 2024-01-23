using System.Globalization;

using NetCord.Rest;

namespace NetCord;

public partial class User : ClientEntity, IJsonModel<JsonModels.JsonUser>
{
    JsonModels.JsonUser IJsonModel<JsonModels.JsonUser>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonUser _jsonModel;

    public override ulong Id => _jsonModel.Id;
    public string Username => _jsonModel.Username;
    public ushort Discriminator => _jsonModel.Discriminator;
    public string? GlobalName => _jsonModel.GlobalName;
    public string? AvatarHash => _jsonModel.AvatarHash;
    public bool IsBot => _jsonModel.IsBot;
    public bool? IsSystemUser => _jsonModel.IsSystemUser;
    public bool? MfaEnabled => _jsonModel.MfaEnabled;
    public string? BannerHash => _jsonModel.BannerHash;
    public Color? AccentColor => _jsonModel.AccentColor;
    public CultureInfo? Locale => _jsonModel.Locale;
    public bool? Verified => _jsonModel.Verified;
    public string? Email => _jsonModel.Email;
    public UserFlags? Flags => _jsonModel.Flags;
    public PremiumType? PremiumType => _jsonModel.PremiumType;
    public UserFlags? PublicFlags => _jsonModel.PublicFlags;
    public string? AvatarDecorationHash => _jsonModel.AvatarDecorationHash;

    public User(JsonModels.JsonUser jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;
    }

    public bool HasAvatar => AvatarHash is not null;

    public ImageUrl GetAvatarUrl(ImageFormat? format = null) => ImageUrl.UserAvatar(Id, AvatarHash!, format);

    public bool HasBanner => BannerHash is not null;

    public ImageUrl GetBannerUrl(ImageFormat? format = null) => ImageUrl.UserBanner(Id, BannerHash!, format);

    public bool HasAvatarDecoration => AvatarDecorationHash is not null;

    public ImageUrl GetAvatarDecorationUrl() => ImageUrl.UserAvatarDecoration(Id, AvatarDecorationHash!);

    public ImageUrl DefaultAvatarUrl => Discriminator is 0 ? ImageUrl.DefaultUserAvatar(Id) : ImageUrl.DefaultUserAvatar(Discriminator);

    public override string ToString() => $"<@{Id}>";

    public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null) => Mention.TryFormatUser(destination, out charsWritten, Id);
}
