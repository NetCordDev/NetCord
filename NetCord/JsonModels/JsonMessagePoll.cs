using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessagePoll : JsonEntity
{
    [JsonPropertyName("question")]
    public JsonMessagePollMedia Question { get; set; }
    
    [JsonPropertyName("answers")]
    public JsonMessagePollAnswer[] Answers { get; set; }

    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }
    
    [JsonPropertyName("layout_type")]
    public MessagePollLayoutType LayoutType { get; set; }
    
    // Non-expiring posts are possible in the future, see: https://github.com/discord/discord-api-docs/blob/e4bdf50f11f9ca61ace2636285e029a2b3dfd0ec/docs/resources/Poll.md#poll-object
    [JsonPropertyName("expiry")]
    public DateTimeOffset? ExpiresAt { get; set; }
    
    [JsonPropertyName("results")]
    public JsonMessagePollResults? Results { get; set; }
}
