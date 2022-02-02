using System.Text.Json.Serialization;

namespace NetCord
{
    public class ActionRowProperties : ComponentProperties
    {
        [JsonPropertyName("components")]
        public List<ButtonProperties> Buttons { get; } = new();

        public ActionRowProperties() : base(ComponentType.ActionRow)
        {
        }
    }
}
