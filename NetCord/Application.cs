using NetCord.Rest;

namespace NetCord;

public partial class Application : ClientEntity, IJsonModel<JsonModels.JsonApplication>
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
    public User? Bot { get; }
    public string? TermsOfServiceUrl => _jsonModel.TermsOfServiceUrl;
    public string? PrivacyPolicyUrl => _jsonModel.PrivacyPolicyUrl;
    public User? Owner { get; }
    public string VerifyKey => _jsonModel.VerifyKey;
    public Team? Team { get; }
    public ulong? GuildId => _jsonModel.GuildId;
    public RestGuild? Guild { get; }
    public ulong? PrimarySkuId => _jsonModel.PrimarySkuId;
    public string? Slug => _jsonModel.Slug;
    public string? CoverImageHash => _jsonModel.CoverImageHash;
    public ApplicationFlags? Flags => _jsonModel.Flags;
    public int? ApproximateGuildCount => _jsonModel.ApproximateGuildCount;
    public IReadOnlyList<string>? RedirectUris => _jsonModel.RedirectUris;
    public string? InteractionsEndpointUrl => _jsonModel.InteractionsEndpointUrl;
    public string? RoleConnectionsVerificationUrl => _jsonModel.RoleConnectionsVerificationUrl;
    public IReadOnlyList<string>? Tags => _jsonModel.Tags;
    public ApplicationInstallParams? InstallParams { get; }
    public IReadOnlyDictionary<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration>? IntegrationTypesConfiguration { get; }
    public string? CustomInstallUrl => _jsonModel.CustomInstallUrl;

    public Application(JsonModels.JsonApplication jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        var bot = jsonModel.Bot;
        if (bot is not null)
            Bot = new(bot, client);

        var owner = jsonModel.Owner;
        if (owner is not null)
            Owner = new(owner, client);

        var team = jsonModel.Team;
        if (team is not null)
            Team = new(team, client);

        var guild = jsonModel.Guild;
        if (guild is not null)
            Guild = new(guild, client);

        var installParams = jsonModel.InstallParams;
        if (installParams is not null)
            InstallParams = new(installParams);

        var integrationTypesConfiguration = jsonModel.IntegrationTypesConfiguration;
        if (integrationTypesConfiguration is not null)
            IntegrationTypesConfiguration = integrationTypesConfiguration.ToDictionary(i => i.Key, i => new ApplicationIntegrationTypeConfiguration(i.Value));
    }
}
