using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="customId">ID for the menu (max 100 characters).</param>
public abstract partial class MenuProperties(string customId) : ComponentProperties
{
    /// <summary>
    /// ID for the menu (max 100 characters).
    /// </summary>
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; } = customId;

    /// <summary>
    /// Placeholder text if nothing is selected (max 150 characters).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Minimum number of items that must be chosen, default 1 (0-25).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("min_values")]
    public int? MinValues { get; set; }

    /// <summary>
    /// Maximum number of items that can be chosen, default 1 (max 25).
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("max_values")]
    public int? MaxValues { get; set; }

    /// <summary>
    /// Whether the menu is disabled.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }
}
