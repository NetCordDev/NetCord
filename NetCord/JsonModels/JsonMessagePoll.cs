using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessagePoll : JsonEntity
{
    [JsonPropertyName("question")]
    public JsonMessagePollMedia Question { get; set; }
    
    [JsonPropertyName("answers")]
    public JsonMessagePollAnswer[] Answers { get; set; }

    [JsonPropertyName("expiry")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }
    
    [JsonPropertyName("layout_type")]
    public MessagePollLayoutType LayoutType { get; set; }
    
    [JsonPropertyName("results")]
    public JsonMessagePollResults? Results { get; set; }
}
