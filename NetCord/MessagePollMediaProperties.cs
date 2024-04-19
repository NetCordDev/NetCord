using System.Text.Json.Serialization;

namespace NetCord;

public partial class MessagePollMediaProperties(string? text = null, EmojiProperties? emoji = null)
{
    // Despite this being nullable, it should never be null. Discord is weird...
    [JsonPropertyName("text")]
    public string? Text { get; set; } = text;
    
    [JsonPropertyName("emoji")]
    public EmojiProperties? Emoji { get; set; } = emoji;
}
