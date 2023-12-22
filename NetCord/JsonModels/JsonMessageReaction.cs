using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessageReaction
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("count_details")]
    public JsonMessageReactionCountDetails CountDetails { get; set; }

    [JsonPropertyName("me")]
    public bool Me { get; set; }

    [JsonPropertyName("me_burst")]
    public bool MeBurst { get; set; }

    [JsonPropertyName("emoji")]
    public JsonEmoji Emoji { get; set; }

    [JsonPropertyName("burst_colors")]
    public Color[] BurstColors { get; set; }
}
