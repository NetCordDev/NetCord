using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// A set of properties representing a webhook's initial configuration.
/// </summary>
/// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
[GenerateMethodsForProperties]
public partial class WebhookProperties(string name)
{
    /// <inheritdoc cref="Webhook.Name"/>
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    /// <summary>
    /// The <see cref="ImageProperties"/> for the default webhook avatar.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("avatar")]
    public ImageProperties? Avatar { get; set; }
}
