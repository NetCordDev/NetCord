using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents properties used to create a guild role.
/// </summary>
[GenerateMethodsForProperties]
public partial class RoleProperties
{
    /// <summary>
    /// The name of the role.
    /// </summary>
    /// <remarks>
    /// Role names can contain at most 100 characters.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The permissions granted to the role.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("permissions")]
    public Permissions? Permissions { get; set; }

    /// <summary>
    /// The colors of the role.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("colors")]
    public RoleColorsProperties? Colors { get; set; }

    /// <summary>
    /// Whether members with the role should be displayed separately in the member list.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("hoist")]
    public bool? Hoist { get; set; }

    /// <summary>
    /// The icon of the role.
    /// </summary>
    /// <remarks>
    /// The guild must have the <c>ROLE_ICONS</c> feature.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }

    /// <summary>
    /// The Unicode emoji used as the role icon.
    /// </summary>
    /// <remarks>
    /// The guild must have the <c>ROLE_ICONS</c> feature.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("unicode_emoji")]
    public string? UnicodeIcon { get; set; }

    /// <summary>
    /// Whether the role should be mentionable.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mentionable")]
    public bool? Mentionable { get; set; }
}
