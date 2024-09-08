﻿using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonInteraction : JsonEntity
{
    [JsonPropertyName("application_id")]
    public ulong ApplicationId { get; set; }

    [JsonPropertyName("type")]
    public InteractionType Type { get; set; }

    [JsonPropertyName("data")]
    public JsonInteractionData? Data { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("guild")]
    public JsonInteractionGuildReference? GuildReference { get; set; }

    [JsonPropertyName("channel")]
    public JsonChannel? Channel { get; set; }

    [JsonPropertyName("member")]
    public JsonGuildUser? GuildUser { get; set; }

    [JsonPropertyName("user")]
    public JsonUser? User { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("message")]
    public JsonMessage? Message { get; set; }

    [JsonPropertyName("app_permissions")]
    public Permissions AppPermissions { get; set; }

    [JsonPropertyName("locale")]
    public string? UserLocale { get; set; }

    [JsonPropertyName("guild_locale")]
    public string? GuildLocale { get; set; }

    [JsonPropertyName("entitlements")]
    public JsonEntitlement[] Entitlements { get; set; }

    [JsonPropertyName("authorizing_integration_owners")]
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong>? AuthorizingIntegrationOwners { get; set; }

    [JsonPropertyName("context")]
    public InteractionContextType? Context { get; set; }
}
