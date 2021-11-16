using System.Text.Json.Serialization;

namespace NetCord
{
    public class ActionRow : Component
    {
        [JsonPropertyName("components")]
        public List<Button> Buttons { get; } = new();

        public ActionRow() : base(MessageComponentType.ActionRow)
        {
        }
    }
}
