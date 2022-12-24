using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class SelfUserOptions
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("avatar")]
    public ImageProperties? Avatar { get; set; }

    [JsonSerializable(typeof(SelfUserOptions))]
    public partial class SelfUserOptionsSerializerContext : JsonSerializerContext
    {
        public static SelfUserOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
