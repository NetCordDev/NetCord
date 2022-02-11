namespace NetCord;

public class Application : Entity
{
    private readonly JsonModels.JsonApplication _jsonEntity;

    public override DiscordId Id => _jsonEntity.Id;
    public string Name => _jsonEntity.Name;
    public string? IconHash => _jsonEntity.IconHash;
    public string Description => _jsonEntity.Description;
    public IEnumerable<string> RpcOrigins => _jsonEntity.RpcOrigins;
    public bool IsBotPublic => _jsonEntity.IsBotPublic;
    public bool BotRequireCodeGrant => _jsonEntity.BotRequireCodeGrant;
    public string? TermsOfServiceUrl => _jsonEntity.TermsOfServiceUrl;
    public string? PrivacyPolicyUrl => _jsonEntity.PrivacyPolicyUrl;
    public User Owner { get; }
    public string Summary => _jsonEntity.Summary;
    public string VerifyKey => _jsonEntity.VerifyKey;
    public Team? Team { get; }
    /// <summary>
    /// If this application is a game sold on Discord, this field will be the guild to which it has been linked
    /// </summary>
    public DiscordId? GuildId => _jsonEntity.GuildId;
    public DiscordId? PrimarySkuId => _jsonEntity.PrimarySkuId;
    public string? Slug => _jsonEntity.Slug;
    public string? CoverImageHash => _jsonEntity.CoverImageHash;
    public ApplicationFlags? Flags => _jsonEntity.Flags;

    internal Application(JsonModels.JsonApplication jsonEntity, RestClient client)
    {
        _jsonEntity = jsonEntity;
        Owner = new(jsonEntity.Owner, client);
        if (jsonEntity.Team != null)
            Team = new(jsonEntity.Team, client);
    }
}
