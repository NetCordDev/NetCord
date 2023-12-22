using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class ConnectionPropertiesProperties
{
    public static ConnectionPropertiesProperties Default => new()
    {
        Os = "linux",
        Browser = "NetCord",
        Device = "NetCord",
    };

    public static ConnectionPropertiesProperties Android => new()
    {
        Os = "linux",
        Browser = "Discord Android",
        Device = "NetCord",
    };

    public static ConnectionPropertiesProperties IOS => new()
    {
        Os = "linux",
        Browser = "Discord iOS",
        Device = "NetCord",
    };

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("os")]
    public string? Os { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("browser")]
    public string? Browser { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("device")]
    public string? Device { get; set; }
}
