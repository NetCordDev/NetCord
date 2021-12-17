using System.Text.Json.Serialization;

namespace NetCord
{
    internal class InteractionCallback
    {
        [JsonPropertyName("type")]
        public InteractionCallbackType Type { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("data")]
        public InteractionMessage? Message { get; }

        public InteractionCallback(InteractionCallbackType type)
        {
            Type = type;
        }

        public InteractionCallback(InteractionCallbackType type, InteractionMessage message)
        {
            Type = type;
            Message = message;
        }
    }
}
