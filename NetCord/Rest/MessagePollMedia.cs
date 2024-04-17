using System.Text.Json.Serialization;

using NetCord.JsonModels;

namespace NetCord.Rest;

public class MessagePollMedia : JsonEntity
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    [JsonPropertyName("emoji")]
    public Emoji? Emoji { get; set; }
}
