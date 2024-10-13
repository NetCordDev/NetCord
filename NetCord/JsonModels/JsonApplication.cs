using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonApplication : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("icon")]
    public string? IconHash { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("rpc_origins")]
    public string[] RpcOrigins { get; set; }

    [JsonPropertyName("bot_public")]
    public bool? BotPublic { get; set; }

    [JsonPropertyName("bot_require_code_grant")]
    public bool? BotRequireCodeGrant { get; set; }

    [JsonPropertyName("bot")]
    public JsonUser? Bot { get; set; }

    [JsonPropertyName("terms_of_service_url")]
    public string? TermsOfServiceUrl { get; set; }

    [JsonPropertyName("privacy_policy_url")]
    public string? PrivacyPolicyUrl { get; set; }

    [JsonPropertyName("owner")]
    public JsonUser? Owner { get; set; }

    [JsonPropertyName("verify_key")]
    public string VerifyKey { get; set; }

    [JsonPropertyName("team")]
    public JsonTeam? Team { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("guild")]
    public JsonGuild? Guild { get; set; }

    [JsonPropertyName("primary_sku_id")]
    public ulong? PrimarySkuId { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("cover_image")]
    public string? CoverImageHash { get; set; }

    [JsonPropertyName("flags")]
    public ApplicationFlags? Flags { get; set; }

    [JsonPropertyName("approximate_guild_count")]
    public int? ApproximateGuildCount { get; set; }

    [JsonPropertyName("approximate_user_install_count")]
    public int? ApproximateUserInstallCount { get; set; }

    [JsonPropertyName("redirect_uris")]
    public string[]? RedirectUris { get; set; }

    [JsonPropertyName("interactions_endpoint_url")]
    public string? InteractionsEndpointUrl { get; set; }

    [JsonPropertyName("role_connections_verification_url")]
    public string? RoleConnectionsVerificationUrl { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    [JsonPropertyName("install_params")]
    public JsonApplicationInstallParams? InstallParams { get; set; }

    [JsonPropertyName("integration_types_config")]
    public IReadOnlyDictionary<ApplicationIntegrationType, JsonApplicationIntegrationTypeConfiguration> IntegrationTypesConfiguration { get; set; }

    [JsonPropertyName("custom_install_url")]
    public string? CustomInstallUrl { get; set; }
}
