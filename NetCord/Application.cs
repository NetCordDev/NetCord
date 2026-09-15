using System.Linq;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Applications or 'apps', are containers for developer platform features, and can contain bots installable to guilds and/or user accounts.
/// </summary>
public partial class Application(JsonApplication jsonModel, RestClient client) 
    : ClientEntity(client), IJsonModel<JsonApplication>
{
    JsonApplication IJsonModel<JsonApplication>.JsonModel => jsonModel;

    /// <summary>
    /// The application's ID..
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The application's name.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The application's icon hash.
    /// </summary>
    public string? IconHash => jsonModel.IconHash;

    /// <summary>
    /// The application's description.
    /// </summary>
    public string Description => jsonModel.Description;

    /// <summary>
    /// A list of the application's RPC origin URLs if enabled, otherwise <see langword="null"/>.
    /// </summary>
    public IReadOnlyList<string> RpcOrigins => jsonModel.RpcOrigins;

    /// <summary>
    /// Whether users other than the owner can add the application to guilds.
    /// </summary>
    public bool? BotPublic => jsonModel.BotPublic;

    /// <summary>
    /// Whether the application's bot will only join upon completion of the full OAuth2 code grant flow.
    /// </summary>
    public bool? BotRequireCodeGrant => jsonModel.BotRequireCodeGrant;

    /// <summary>
    /// The application's user object, representing its bot.
    /// </summary>
    public User? Bot { get; } = jsonModel.Bot is { } bot ? new(bot, client) : null;

    /// <summary>
    /// The application's Terms of Service URL.
    /// </summary>
    public string? TermsOfServiceUrl => jsonModel.TermsOfServiceUrl;

    /// <summary>
    /// The application's Privacy Policy URL.
    /// </summary>
    public string? PrivacyPolicyUrl => jsonModel.PrivacyPolicyUrl;

    /// <summary>
    /// The application owner's user object.
    /// </summary>
    public User? Owner { get; } = jsonModel.Owner is { } owner ? new(owner, client) : null;

    /// <summary>
    /// A hex-encoded verification key, used for HTTP interactions and the GameSDK's GetTicket endpoint.
    /// </summary>
    public string VerifyKey => jsonModel.VerifyKey;

    /// <summary>
    /// The team the application belongs to, if any.
    /// </summary>
    public Team? Team { get; } = jsonModel.Team is { } team ? new(team, client) : null;

    /// <summary>
    /// The ID corresponding to the application's guild.
    /// </summary>
    public ulong? GuildId => jsonModel.GuildId;

    /// <summary>
    /// The application guild's object.
    /// </summary>
    public RestGuild? Guild { get; } = jsonModel.Guild is { } guild ? new(guild, client) : null;

    /// <summary>
    /// The ID of the application's Game SKU if it exists, otherwise <see langword="null"/>.
    /// </summary>
    public ulong? PrimarySkuId => jsonModel.PrimarySkuId;

    /// <summary>
    /// The URL slug that links to an application's store page if it exists, otherwise <see langword="null"/>.
    /// </summary>
    public string? Slug => jsonModel.Slug;

    /// <summary>
    /// The cover image hash for the application's default rich presence invite.
    /// </summary>
    public string? CoverImageHash => jsonModel.CoverImageHash;

    /// <summary>
    /// The application's public flags.
    /// </summary>
    public ApplicationFlags? Flags => jsonModel.Flags;

    /// <summary>
    /// The approximate number of guilds the application has been added to.
    /// </summary>
    public int? ApproximateGuildCount => jsonModel.ApproximateGuildCount;

    /// <summary>
    /// The approximate number of users that have installed the application.
    /// </summary>
    public int? ApproximateUserInstallCount => jsonModel.ApproximateUserInstallCount;

    /// <summary>
    /// A list of the application's redirect URIs.
    /// </summary>
    public IReadOnlyList<string>? RedirectUris => jsonModel.RedirectUris;

    /// <summary>
    /// The application's interactions endpoint URL.
    /// </summary>
    public string? InteractionsEndpointUrl => jsonModel.InteractionsEndpointUrl;

    /// <summary>
    /// The application's role connection verification URL.
    /// </summary>
    public string? RoleConnectionsVerificationUrl => jsonModel.RoleConnectionsVerificationUrl;

    /// <summary>
    /// The application's event webhooks URL to receive webhook events.
    /// </summary>
    public string? EventWebhooksUrl => jsonModel.EventWebhooksUrl;

    /// <summary>
    /// The application's configuration for webhook events.
    /// </summary>
    public ApplicationEventWebhooksStatus EventWebhooksStatus => jsonModel.EventWebhooksStatus;

    /// <summary>
    /// A list of event webhook types that the application supports.
    /// </summary>
    public IReadOnlyList<string>? EventWebhooksTypes => jsonModel.EventWebhooksTypes;

    /// <summary>
    /// A list of the application's tags, describing its content and functionality.
    /// </summary>
    /// <remarks>
    /// A maximum of 5 tags is supported.
    /// </remarks>
    public IReadOnlyList<string>? Tags => jsonModel.Tags;

    /// <summary>
    /// The application's default in-application authorization URL if enabled, otherwise <see langword="null"/>.
    /// </summary>
    public ApplicationInstallParams? InstallParams { get; } = jsonModel.InstallParams is { } installParams ? new(installParams) : null;

    /// <summary>
    /// A list of the application's default scopes and permissions, for each supported installation context.
    /// </summary>
    public IReadOnlyDictionary<ApplicationIntegrationType, ApplicationIntegrationTypeConfiguration>? IntegrationTypesConfiguration { get; }
        = jsonModel.IntegrationTypesConfiguration?.ToDictionary(i => i.Key, i => new ApplicationIntegrationTypeConfiguration(i.Value));

    /// <summary>
    /// The application's default custom install URL if enabled, otherwise <see langword="null"/>.
    /// </summary>
    public string? CustomInstallUrl => jsonModel.CustomInstallUrl;

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
}
