using System.Text.Json.Serialization;

namespace NetCord;

public partial class MessagePollMediaProperties
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    
    [JsonPropertyName("emoji")]
    public EmojiProperties? Emoji { get; set; }
}
