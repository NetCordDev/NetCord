using System.Text.Json.Serialization;

namespace NetCord;

public partial class MessagePollMediaProperties
{
    // Despite this being nullable, it should never be null. Discord is weird...
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    [JsonPropertyName("emoji")]
    public EmojiProperties? Emoji { get; set; }
}
