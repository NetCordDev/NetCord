using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationOptions
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("custom_install_url")]
    public string? CustomInstallUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("role_connections_verification_url")]
    public string? RoleConnectionsVerificationUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("install_params")]
    public ApplicationInstallParamsProperties? InstallParams { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("flags")]
    public ApplicationFlags? Flags { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("cover_image")]
    public ImageProperties? CoverImage { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("interactions_endpoint_url")]
    public string? InteractionsEndpointUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("tags")]
    public IEnumerable<string>? Tags { get; set; }

    [JsonSerializable(typeof(ApplicationOptions))]
    public partial class ApplicationOptionsSerializerContext : JsonSerializerContext
    {
        public static ApplicationOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
