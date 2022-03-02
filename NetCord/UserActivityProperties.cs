using System.Text.Json.Serialization;

namespace NetCord;

public class UserActivityProperties
{
    public UserActivityProperties(string name, UserActivityType type)
    {
        Name = name;
        Type = type;
    }

    [JsonPropertyName("name")]
    public string Name { get; }

    [JsonPropertyName("type")]
    public UserActivityType Type { get; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("created_at")]
    public int CreatedAt { get; set; }

    [JsonPropertyName("timestamps")]
    public UserActivityTimestampsProperties? Timestamps { get; set; }

    [JsonPropertyName("application_id")]
    public DiscordId? ApplicationId { get; set; }

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
}