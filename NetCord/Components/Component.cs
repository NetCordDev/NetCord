using System.Text.Json.Serialization;

namespace NetCord
{
    [JsonConverter(typeof(JsonConverters.ComponentConverter))]
    public abstract class Component
    {
        [JsonPropertyName("type")]
        public MessageComponentType ComponentType { get; }

        protected Component(MessageComponentType type)
        {
            ComponentType = type;
        }
    }
}
