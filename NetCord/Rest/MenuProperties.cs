using System.Text.Json.Serialization;

namespace NetCord.Rest;

public abstract class MenuProperties : ComponentProperties
{
    /// <summary>
    /// Id for the menu (max 100 characters).
    /// </summary>
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; }

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customId">Id for the menu (max 100 characters).</param>
    /// <param name="type">Type of the component.</param>
    protected MenuProperties(string customId, ComponentType type) : base(type)
    {
        CustomId = customId;
    }
}
