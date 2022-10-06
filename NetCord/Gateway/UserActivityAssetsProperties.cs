using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class UserActivityAssetsProperties
{
    [JsonPropertyName("large_image")]
    public string? LargeImage { get; set; }

    [JsonPropertyName("large_text")]
    public string? LargeText { get; set; }

    [JsonPropertyName("small_image")]
    public string? SmallImage { get; set; }

    [JsonPropertyName("small_text")]
    public string? SmallText { get; set; }

    [JsonSerializable(typeof(UserActivityAssetsProperties))]
    public partial class UserActivityAssetsPropertiesSerializerContext : JsonSerializerContext
    {
        public static UserActivityAssetsPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
