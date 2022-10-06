using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class UserActivityButtonProperties
{
    public UserActivityButtonProperties(string label, string url)
    {
        Label = label;
        Url = url;
    }

    [JsonPropertyName("label")]
    public string Label { get; }

    [JsonPropertyName("url")]
    public string Url { get; }

    [JsonSerializable(typeof(UserActivityButtonProperties))]
    public partial class UserActivityButtonPropertiesSerializerContext : JsonSerializerContext
    {
        public static UserActivityButtonPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
