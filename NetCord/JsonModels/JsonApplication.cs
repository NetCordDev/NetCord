using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public partial class JsonApplication : JsonEntity
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

    [JsonPropertyName("primary_sku_id")]
    public ulong? PrimarySkuId { get; set; }

    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [JsonPropertyName("cover_image")]
    public string? CoverImageHash { get; set; }

    [JsonPropertyName("flags")]
    public ApplicationFlags? Flags { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    [JsonPropertyName("install_params")]
    public JsonApplicationInstallParams? InstallParams { get; set; }

    [JsonPropertyName("custom_install_url")]
    public string? CustomInstallUrl { get; set; }

    [JsonPropertyName("role_connections_verification_url")]
    public string? RoleConnectionsVerificationUrl { get; set; }

    [JsonSerializable(typeof(JsonApplication))]
    public partial class JsonApplicationSerializerContext : JsonSerializerContext
    {
        public static JsonApplicationSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
