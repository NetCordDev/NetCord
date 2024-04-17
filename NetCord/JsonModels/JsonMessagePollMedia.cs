using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessagePollMedia : JsonEntity
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; set; }
}
