using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class MenuProperties : ComponentProperties
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; }

    [JsonPropertyName("options")]
    public IEnumerable<MenuSelectOptionProperties> Options { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("min_values")]
    public int? MinValues { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("max_values")]
    public int? MaxValues { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }

    public MenuProperties(string customId, IEnumerable<MenuSelectOptionProperties> options) : base(ComponentType.Menu)
    {
        CustomId = customId;
        Options = options;
    }

    [JsonSerializable(typeof(MenuProperties))]
    public partial class MenuPropertiesSerializerContext : JsonSerializerContext
    {
        public static MenuPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
