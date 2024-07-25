using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ApplicationEmojiOptions
{
    internal ApplicationEmojiOptions()
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
