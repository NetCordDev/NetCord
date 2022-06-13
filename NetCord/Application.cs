namespace NetCord;

public class Application : Entity, IJsonModel<JsonModels.JsonApplication>
{
    JsonModels.JsonApplication IJsonModel<JsonModels.JsonApplication>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplication _jsonModel;

    public override Snowflake Id => _jsonModel.Id;
    public string Name => _jsonModel.Name;
    public string? IconHash => _jsonModel.IconHash;
    public string Description => _jsonModel.Description;
    public IEnumerable<string> RpcOrigins => _jsonModel.RpcOrigins;
    public bool IsBotPublic => _jsonModel.IsBotPublic;
    public bool BotRequireCodeGrant => _jsonModel.BotRequireCodeGrant;
    public string? TermsOfServiceUrl => _jsonModel.TermsOfServiceUrl;
    public string? PrivacyPolicyUrl => _jsonModel.PrivacyPolicyUrl;
    public User Owner { get; }
    public string Summary => _jsonModel.Summary;
    public string VerifyKey => _jsonModel.VerifyKey;
    public Team? Team { get; }
    /// <summary>
    /// If this application is a game sold on Discord, this field will be the guild to which it has been linked
    /// </summary>
    public Snowflake? GuildId => _jsonModel.GuildId;
    public Snowflake? PrimarySkuId => _jsonModel.PrimarySkuId;
    public string? Slug => _jsonModel.Slug;
    public string? CoverImageHash => _jsonModel.CoverImageHash;
    public ApplicationFlags? Flags => _jsonModel.Flags;

    public Application(JsonModels.JsonApplication jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Owner = new(jsonModel.Owner, client);
        if (jsonModel.Team != null)
            Team = new(jsonModel.Team, client);
    }
}
