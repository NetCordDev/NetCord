using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class CurrentUserOptions
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("avatar")]
    public ImageProperties? Avatar { get; set; }

    [JsonSerializable(typeof(CurrentUserOptions))]
    public partial class CurrentUserOptionsSerializerContext : JsonSerializerContext
    {
        public static CurrentUserOptionsSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
