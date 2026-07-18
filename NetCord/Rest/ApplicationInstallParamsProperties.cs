using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <inheritdoc cref="ApplicationInstallParams"/>
[GenerateMethodsForProperties]
public partial class ApplicationInstallParamsProperties
{
    /// <inheritdoc cref="ApplicationInstallParams.Scopes"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("scopes")]
    public IEnumerable<string>? Scopes { get; set; }

    /// <inheritdoc cref="ApplicationInstallParams.Permissions"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("permissions")]
    public Permissions? Permissions { get; set; }
}
