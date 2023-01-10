using System.Text.Json.Serialization;

namespace NetCord.Gateway;

public partial class UserActivityProperties
{
    public UserActivityProperties(string name, UserActivityType type)
    {
        Name = name;
        Type = type;
    }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public UserActivityType Type { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("created_at")]
    public int CreatedAt { get; set; }

    [JsonPropertyName("timestamps")]
    public UserActivityTimestampsProperties? Timestamps { get; set; }

    [JsonPropertyName("application_id")]
    public ulong? ApplicationId { get; set; }

    [JsonPropertyName("details")]
    public string? Details { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("emoji")]
    public EmojiProperties? Emoji { get; set; }

    [JsonPropertyName("party")]
    public PartyProperties? Party { get; set; }

    [JsonPropertyName("assets")]
    public UserActivityAssetsProperties? Assets { get; set; }

    [JsonPropertyName("secrets")]
    public UserActivitySecretsProperties? Secrets { get; set; }

    [JsonPropertyName("instance")]
    public bool? Instance { get; set; }

    [JsonPropertyName("flags")]
    public UserActivityFlags? Flags { get; set; }

    [JsonPropertyName("buttons")]
    public IEnumerable<UserActivityButtonProperties>? Buttons { get; set; }

    [JsonSerializable(typeof(UserActivityProperties))]
    public partial class UserActivityPropertiesSerializerContext : JsonSerializerContext
    {
        public static UserActivityPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
