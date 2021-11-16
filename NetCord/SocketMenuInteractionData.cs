using System.Text.Json.Serialization;

namespace NetCord
{
    public class SocketMenuInteractionData : SocketButtonInteractionData
    {
        [JsonInclude]
        [JsonPropertyName("values")]
        public IEnumerable<string> SelectedOptions { get; private init; }
    }
}