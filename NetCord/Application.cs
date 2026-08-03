using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Applications or 'apps', are containers for developer platform features, and can contain bots installable to guilds and/or user accounts.
/// </summary>
public partial class Application : ClientEntity, IJsonModel<JsonModels.JsonApplication>
{
    JsonModels.JsonApplication IJsonModel<JsonModels.JsonApplication>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonApplication _jsonModel;

    /// <summary>
    /// Constructs an <see cref="Application"/> using a JSON Model and <see cref="RestClient"/>.
    /// </summary>
    /// <param name="jsonModel">The JSON model to create an <see cref="Application"/> from.</param>
    /// <param name="client">The <see cref="RestClient"/> to use for construction.</param>
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

    /// <summary>
    /// The application's ID..
    /// </summary>
    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// The application's name.
    /// </summary>
    public string Name => _jsonModel.Name;

    /// <summary>
    /// The application's icon hash.
    /// </summary>
    public string? IconHash => _jsonModel.IconHash;

    /// <summary>
    /// The application's description.
    /// </summary>
    public string Description => _jsonModel.Description;

    /// <summary>
    /// A list of the application's RPC origin URLs if enabled, otherwise <see langword="null"/>.
    /// </summary>
    public IReadOnlyList<string> RpcOrigins => _jsonModel.RpcOrigins;

    /// <summary>
    /// Whether users other than the owner can add the application to guilds.
    /// </summary>
    public bool? BotPublic => _jsonModel.BotPublic;

    /// <summary>
    /// Whether the application's bot will only join upon completion of the full OAuth2 code grant flow.
    /// </summary>
    public bool? BotRequireCodeGrant => _jsonModel.BotRequireCodeGrant;

    /// <summary>
    /// The application's user object, representing its bot.
    /// </summary>
    public User? Bot { get; }

    /// <summary>
    /// The application's Terms of Service URL.
    /// </summary>
    public string? TermsOfServiceUrl => _jsonModel.TermsOfServiceUrl;

    /// <summary>
    /// The application's Privacy Policy URL.
    /// </summary>
    public string? PrivacyPolicyUrl => _jsonModel.PrivacyPolicyUrl;

    /// <summary>
    /// The application owner's user object.
    /// </summary>
    public User? Owner { get; }

    /// <summary>
    /// A hex-encoded verification key, used for HTTP interactions and the GameSDK's GetTicket endpoint.
    /// </summary>
    public string VerifyKey => _jsonModel.VerifyKey;

    /// <summary>
    /// The team the application belongs to, if any.
    /// </summary>
    public Team? Team { get; }

    /// <summary>
    /// The ID corresponding to the application's guild.
    /// </summary>
    public ulong? GuildId => _jsonModel.GuildId;

    /// <summary>
    /// The application guild's object.
    /// </summary>
    public RestGuild? Guild { get; }

    /// <summary>
    /// The ID of the appliication's Game SKU if it exists, otherwise <see langword="null"/>.
    /// </summary>
    public ulong? PrimarySkuId => _jsonModel.PrimarySkuId;

    /// <summary>
    /// The URL slug that links to an application's store pageif it exists, otherwise <see langword="null"/>.
    /// </summary>
    public string? Slug => _jsonModel.Slug;

    /// <summary>
    /// The cover image hash for the application's default rich presence invite.
    /// </summary>
    public string? CoverImageHash => _jsonModel.CoverImageHash;

    /// <summary>
    /// The application's public flags.
    /// </summary>
    public ApplicationFlags? Flags => _jsonModel.Flags;

    /// <summary>
    /// The approximate number of guilds the application has been added to.
    /// </summary>
    public int? ApproximateGuildCount => _jsonModel.ApproximateGuildCount;

    /// <summary>
    /// The approximate number of users that have installed the application.
    /// </summary>
    public int? ApproximateUserInstallCount => _jsonModel.ApproximateUserInstallCount;

    /// <summary>
    /// A list of the application's redirect URIs.
    /// </summary>
    public IReadOnlyList<string>? RedirectUris => _jsonModel.RedirectUris;

    /// <summary>
    /// The application's interactions endpoint URL.
    /// </summary>
    public string? InteractionsEndpointUrl => _jsonModel.InteractionsEndpointUrl;

    /// <summary>
    /// The application's role connection verification URL.
    /// </summary>
    public string? RoleConnectionsVerificationUrl => _jsonModel.RoleConnectionsVerificationUrl;

    /// <summary>
    /// The application's event webhooks URL to receive webhook events.
    /// </summary>
    public string? EventWebhooksUrl => _jsonModel.EventWebhooksUrl;

    /// <summary>
    /// The application's configuration for webhook events.
    /// </summary>
    public ApplicationEventWebhooksStatus EventWebhooksStatus => _jsonModel.EventWebhooksStatus;

    /// <summary>
    /// A list of event webhook types that the application supports.
    /// </summary>
    public IReadOnlyList<string>? EventWebhooksTypes => _jsonModel.EventWebhooksTypes;

    /// <summary>
    /// A list of the application's tags, describing its content and functionality.
    /// </summary>
    /// <remarks>
    /// A maximum of 5 tags is supported.
    /// </remarks>
    public IReadOnlyList<string>? Tags => _jsonModel.Tags;

    /// <summary>
    /// The application's default in-app authorization URL if enabled, otherwise <see langword="null"/>.
    /// </summary>
    public ApplicationInstallParams? InstallParams { get; }

    /// <summary>
    /// A list of the app's default scopes and permissions, for each supported installation context.
    /// </summary>
    public IReadOnlyDictionary<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration>? IntegrationTypesConfiguration { get; }

    /// <summary>
    /// The application's default custom install URL if enabled, otherwise <see langword="null"/>.
    /// </summary>
    public string? CustomInstallUrl => _jsonModel.CustomInstallUrl;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the application's icon.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    public ImageUrl? GetIconUrl(ImageFormat format) => IconHash is string hash ? ImageUrl.ApplicationIcon(Id, hash, format) : null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the application's cover.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    public ImageUrl? GetCoverUrl(ImageFormat format) => CoverImageHash is string hash ? ImageUrl.ApplicationCover(Id, hash, format) : null;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the an asset associated with the application.
    /// </summary>
    /// <param name="assetId">The ID of the asset to get an <see cref="ImageUrl"/> for.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    public ImageUrl? GetAssetUrl(ulong assetId, ImageFormat format) => ImageUrl.ApplicationAsset(Id, assetId, format);

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of an achievement associated with the application.
    /// </summary>
    /// <param name="achievementId">The ID of the achievement to get an <see cref="ImageUrl"/> for.</param>
    /// <param name="iconHash">The hash of the achievement's icon.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    public ImageUrl? GetAchievementIconUrl(ulong achievementId, string iconHash, ImageFormat format) => ImageUrl.AchievementIcon(Id, achievementId, iconHash, format);

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of a store page asset associated with the application.
    /// </summary>
    /// <param name="assetId">The ID of the asset to get an <see cref="ImageUrl"/> for.</param>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>.</param>
    public ImageUrl? GetStorePageAssetUrl(ulong assetId, ImageFormat format) => ImageUrl.StorePageAsset(Id, assetId, format);
}
