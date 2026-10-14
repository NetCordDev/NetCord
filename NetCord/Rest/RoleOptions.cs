using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// Represents options for modifying a guild role.
/// </summary>
/// <remarks>
/// Modifying a guild role requires the <c>MANAGE_ROLES</c> permission.
/// </remarks>
[GenerateMethodsForProperties]
public partial class RoleOptions
{
    internal RoleOptions()
    {
    }

    /// <summary>
    /// The new name of the role.
    /// </summary>
    /// <remarks>
    /// The name can contain at most 100 characters.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The new permissions of the role.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("permissions")]
    public Permissions? Permissions { get; set; }

    /// <summary>
    /// The new colors of the role.
    /// </summary>
    /// <remarks>
    /// Secondary and tertiary role colors require the guild to have the <c>ENHANCED_ROLE_COLORS</c> guild feature.
    /// </remarks>
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
    /// The new icon of the role.
    /// </summary>
    /// <remarks>
    /// The guild must have the <c>ROLE_ICONS</c> guild feature.
    /// </remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("icon")]
    public ImageProperties? Icon { get; set; }

    /// <summary>
    /// The new Unicode emoji used as the role icon.
    /// </summary>
    /// <remarks>
    /// The guild must have the <c>ROLE_ICONS</c> guild feature.
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
