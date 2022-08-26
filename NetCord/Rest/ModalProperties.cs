using System.Text.Json.Serialization;

namespace NetCord.Rest;

public class ModalProperties
{
    [JsonPropertyName("custom_id")]
    public string CustomId { get; }

    [JsonPropertyName("title")]
    public string Title { get; }

    [JsonConverter(typeof(JsonConverters.TextInputPropertiesIEnumerableConverter))]
    [JsonPropertyName("components")]
    public IEnumerable<TextInputProperties> Components { get; }

    public ModalProperties(string customId, string title, IEnumerable<TextInputProperties> components)
    {
        CustomId = customId;
        Title = title;
        Components = components;
    }
}
