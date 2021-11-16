using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public class ActionButton : Button
    {
        [JsonPropertyName("custom_id")]
        public string CustomId { get; }

        public ActionButton(string label, string customId, MessageButtonStyle style) : base(label, style)
        {
            CustomId = customId;
        }
    }
}
