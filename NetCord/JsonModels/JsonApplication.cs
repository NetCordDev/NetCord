using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

internal record JsonApplication : JsonEntity
{
    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("icon")]
    public string? IconHash { get; init; }

    [JsonPropertyName("description")]
    public string Description { get; init; }

    [JsonPropertyName("rpc_origins")]
    public string[] RpcOrigins { get; init; }

    [JsonPropertyName("bot_public")]
    public bool IsBotPublic { get; init; }

    [JsonPropertyName("bot_require_code_grant")]
    public bool BotRequireCodeGrant { get; init; }

    [JsonPropertyName("terms_of_service_url")]
    public string? TermsOfServiceUrl { get; init; }

    [JsonPropertyName("privacy_policy_url")]
    public string? PrivacyPolicyUrl { get; init; }

    [JsonPropertyName("owner")]
    public JsonUser Owner { get; init; }

    [JsonPropertyName("summary")]
    public string Summary { get; init; }

    [JsonPropertyName("verify_key")]
    public string VerifyKey { get; init; }

    [JsonPropertyName("team")]
    public JsonTeam? Team { get; init; }

    [JsonPropertyName("guild_id")]
    /// <summary>
    /// If this application is a game sold on Discord, this field will be the guild to which it has been linked
    /// </summary>
    public DiscordId? GuildId { get; init; }

    [JsonPropertyName("primary_sku_id")]
    public DiscordId? PrimarySkuId { get; init; }

    [JsonPropertyName("slug")]
    public string? Slug { get; init; }

    [JsonPropertyName("cover_image")]
    public string? CoverImageHash { get; init; }

    [JsonPropertyName("flags")]
    public ApplicationFlags? Flags { get; init; }
}
