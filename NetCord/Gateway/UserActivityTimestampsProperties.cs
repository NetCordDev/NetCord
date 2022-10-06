using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class UserActivityTimestampsProperties
{
    [JsonPropertyName("start")]
    public int? Start { get; set; }

    [JsonPropertyName("end")]
    public int? End { get; set; }

    [JsonSerializable(typeof(UserActivityTimestampsProperties))]
    public partial class UserActivityTimestampsPropertiesSerializerContext : JsonSerializerContext
    {
        public static UserActivityTimestampsPropertiesSerializerContext WithOptions { get; } = new(new(ToObjectExtensions._options));
    }
}
