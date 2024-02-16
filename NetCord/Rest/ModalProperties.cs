using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class ModalProperties(string customId, string title, IEnumerable<TextInputProperties> components)
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; set; } = customId;

    [JsonPropertyName("title")]
    public string Title { get; set; } = title;

    [JsonConverter(typeof(JsonConverters.TextInputPropertiesIEnumerableConverter))]
    [JsonPropertyName("components")]
    public IEnumerable<TextInputProperties> Components { get; set; } = components;
}
