namespace NetCord;
public class User : ClientEntity
{
    private protected readonly JsonModels.JsonUser _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;
    public virtual string Username => _jsonEntity.Username;
    public virtual ushort Discriminator => _jsonEntity.Discriminator;
    public virtual string? AvatarHash => _jsonEntity.AvatarHash;
    public virtual bool IsBot => _jsonEntity.IsBot;
    public virtual bool? IsOfficialDiscordUser => _jsonEntity.IsOfficialDiscordUser;
    public virtual bool? MFAEnabled => _jsonEntity.MFAEnabled;
    public virtual string? Locale => _jsonEntity.Locale;
    public virtual bool? Verified => _jsonEntity.Verified;
    public virtual string? Email => _jsonEntity.Email;
    public virtual UserFlags? Flags => _jsonEntity.Flags;
    public virtual PremiumType? PremiumType => _jsonEntity.PremiumType;
    public virtual UserFlags? PublicFlags => _jsonEntity.PublicFlags;

    internal User(JsonModels.JsonUser jsonEntity, BotClient client) : base(client)
    {
        _jsonEntity = jsonEntity;
    }

    public bool HasAvatar => AvatarHash != null;

    public string GetAvatarUrl(ImageFormat? format = null) => UserHelper.GetAvatarUrl(Id, AvatarHash, format);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="format"></param>
    /// <param name="size">any power of two between 16 and 4096</param>
    /// <returns></returns>
    public string GetAvatarUrl(int size, ImageFormat? format = null) => UserHelper.GetAvatarUrl(Id, AvatarHash, size, format);

    public string DefaultAvatarUrl => UserHelper.GetDefaultAvatarUrl(Discriminator);

    public Task<DMChannel> GetDMChannelAsync() => UserHelper.GetDMChannelAsync(_client, Id);


    public override string ToString() => $"<@{Id}>";
}