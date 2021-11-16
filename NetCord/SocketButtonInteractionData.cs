using System.Text.Json.Serialization;

namespace NetCord
{
    public class SocketButtonInteractionData
    {
        [JsonInclude]
        [JsonPropertyName("custom_id")]
        public string CustomId { get; private init; }
    }
}