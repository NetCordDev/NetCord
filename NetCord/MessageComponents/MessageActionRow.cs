using System.Text.Json.Serialization;

namespace NetCord
{
    public class MessageActionRow : MessageComponent
    {
        [JsonPropertyName("components")]
        public List<MessageButton> Buttons { get; } = new();

        public MessageActionRow() : base(ComponentType.ActionRow)
        {
        }
    }
}
