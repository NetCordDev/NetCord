using System.Text.Json.Serialization;

namespace NetCord.JsonModels;

public class JsonMessagePollMedia : JsonEntity
{
    // Despite this being nullable, it should never be null. Discord is weird...
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    [JsonPropertyName("emoji")]
    public JsonEmoji? Emoji { get; set; }
}
