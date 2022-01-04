using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public class MessageActionButton : MessageButton
    {
        [JsonPropertyName("custom_id")]
        public string CustomId { get; }

        public MessageActionButton(string label, string customId, ButtonStyle style) : base(label, style)
        {
            CustomId = customId;
        }
    }
}
