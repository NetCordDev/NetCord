namespace NetCord
{
    public class MessageActionButton : MessageButton
    {
        public MessageButtonStyle Style => (MessageButtonStyle)_jsonEntity.Style;
        public string CustomId => _jsonEntity.CustomId;

        internal MessageActionButton(JsonModels.JsonMessageComponent jsonEntity) : base(jsonEntity)
        {

        }
    }
}
