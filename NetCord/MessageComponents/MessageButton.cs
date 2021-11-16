namespace NetCord
{
    public abstract class MessageButton
    {
        private protected readonly JsonModels.JsonMessageComponent _jsonEntity;

        public MessageComponentType ComponentType => MessageComponentType.Button;

        public string Label => _jsonEntity.Label;

        public MessageComponentEmoji? Emoji { get; }

        public bool Disabled => (bool)_jsonEntity.Disabled;

        private protected MessageButton(JsonModels.JsonMessageComponent jsonEntity)
        {
            _jsonEntity = jsonEntity;
            Emoji = new(jsonEntity.Emoji);
        }

        internal static MessageButton CreateFromJson(JsonModels.JsonMessageComponent jsonEntity)
        {
            return (int)jsonEntity.Style switch
            {
                5 => new MessageLinkButton(jsonEntity),
                _ => new MessageActionButton(jsonEntity),
            };
        }
    }
}
