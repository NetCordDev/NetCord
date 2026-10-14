using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents properties used to create a guild from a guild template.
/// </summary>
/// <remarks>
/// Discord deprecated application-driven guild creation in April 2025
/// and removed the corresponding API endpoint in July 2025.
/// This type is retained for compatibility with
/// <see cref="RestClient.CreateGuildFromGuildTemplateAsync(string, GuildFromGuildTemplateProperties, RestRequestProperties?, CancellationToken)"/>.
/// </remarks>
/// <param name="name">The name of the guild.</param>
[GenerateMethodsForProperties]
[Obsolete("Discord deprecated application-driven guild creation in April 2025 and removed the corresponding API endpoint in July 2025.")]
public partial class GuildFromGuildTemplateProperties(string name)
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = name;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }
}
