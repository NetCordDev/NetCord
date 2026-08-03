using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents a modification to apply to the bot user's current application.
/// </summary>
[GenerateMethodsForProperties]
public partial class CurrentApplicationOptions
{
    internal CurrentApplicationOptions()
    {
    }

    /// <inheritdoc cref="Application.CustomInstallUrl"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("custom_install_url")]
    public string? CustomInstallUrl { get; set; }

    /// <inheritdoc cref="Application.Description"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <inheritdoc cref="Application.RoleConnectionsVerificationUrl"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("role_connections_verification_url")]
    public string? RoleConnectionsVerificationUrl { get; set; }

    /// <inheritdoc cref="Application.InstallParams"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("install_params")]
    public ApplicationInstallParamsProperties? InstallParams { get; set; }

    /// <inheritdoc cref="Application.IntegrationTypesConfiguration"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("integration_types_config")]
    public IReadOnlyDictionary<ApplicationIntegrationType, ApplicationIntegrationTypeConfigurationProperties>? IntegrationTypesConfiguration { get; set; }

    /// <inheritdoc cref="Application.Flags"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("flags")]
    public ApplicationFlags? Flags { get; set; }

    /// <summary>
    /// The application's icon.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }

    /// <summary>
    /// The application's cover image.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cover_image")]
    public ImageProperties? CoverImage { get; set; }

    /// <inheritdoc cref="Application.InteractionsEndpointUrl"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("interactions_endpoint_url")]
    public string? InteractionsEndpointUrl { get; set; }

    /// <inheritdoc cref="Application.Tags"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("tags")]
    public IEnumerable<string>? Tags { get; set; }

    /// <inheritdoc cref="Application.EventWebhooksUrl"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("event_webhooks_url")]
    public string? EventWebhooksUrl { get; set; }

    /// <inheritdoc cref="Application.EventWebhooksStatus"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("event_webhooks_status")]
    public ApplicationEventWebhooksStatus? EventWebhooksStatus { get; set; }

    /// <inheritdoc cref="Application.EventWebhooksTypes"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("event_webhooks_types")]
    public IEnumerable<string>? EventWebhooksTypes { get; set; }
}
