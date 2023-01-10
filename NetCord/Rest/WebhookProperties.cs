using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class WebhookProperties
{
    public WebhookProperties(string name)
    {
        Name = name;
    }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("avatar")]
    public ImageProperties? Avatar { get; set; }

    [JsonSerializable(typeof(WebhookProperties))]
    public partial class WebhookPropertiesSerializerContext : JsonSerializerContext
    {
        public static WebhookPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
