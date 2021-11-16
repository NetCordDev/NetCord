using NetCord.JsonModels;

namespace NetCord
{
    public class MessageActionRow : IMessageComponent
    {
        public MessageComponentType ComponentType => MessageComponentType.ActionRow;
        public IEnumerable<MessageButton> Buttons { get; }

        internal MessageActionRow(JsonMessageComponent jsonEntity)
        {
            Buttons = jsonEntity.Components.SelectOrEmpty(b => MessageButton.CreateFromJson(b));
        }
    }
}