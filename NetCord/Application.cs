using NetCord.Rest;

namespace NetCord;

public class Application : Entity, IJsonModel<JsonModels.JsonApplication>
{
    JsonModels.JsonApplication IJsonModel<JsonModels.JsonApplication>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplication _jsonModel;

    public override ulong Id => _jsonModel.Id;
    public string Name => _jsonModel.Name;
    public string? IconHash => _jsonModel.IconHash;
    public string Description => _jsonModel.Description;
    public IReadOnlyList<string> RpcOrigins => _jsonModel.RpcOrigins;
    public bool? BotPublic => _jsonModel.BotPublic;
    public bool? BotRequireCodeGrant => _jsonModel.BotRequireCodeGrant;
    public string? TermsOfServiceUrl => _jsonModel.TermsOfServiceUrl;
    public string? PrivacyPolicyUrl => _jsonModel.PrivacyPolicyUrl;
    public User? Owner { get; }
    public string VerifyKey => _jsonModel.VerifyKey;
    public Team? Team { get; }
    public ulong? GuildId => _jsonModel.GuildId;
    public ulong? PrimarySkuId => _jsonModel.PrimarySkuId;
    public string? Slug => _jsonModel.Slug;
    public string? CoverImageHash => _jsonModel.CoverImageHash;
    public ApplicationFlags? Flags => _jsonModel.Flags;
    public IReadOnlyList<string>? Tags => _jsonModel.Tags;
    public ApplicationInstallParams? InstallParams { get; }
    public string? CustomInstallUrl => _jsonModel.CustomInstallUrl;
    public string? RoleConnectionsVerificationUrl => _jsonModel.RoleConnectionsVerificationUrl;

    public Application(JsonModels.JsonApplication jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.Owner != null)
            Owner = new(jsonModel.Owner, client);
        if (jsonModel.Team != null)
            Team = new(jsonModel.Team, client);
        if (jsonModel.InstallParams != null)
            InstallParams = new(jsonModel.InstallParams);
    }
}
