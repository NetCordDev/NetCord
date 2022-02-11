namespace NetCord
{
    public abstract class Button
    {
        private protected readonly JsonModels.JsonComponent _jsonEntity;

        public ComponentType ComponentType => ComponentType.Button;

        public string? Label => _jsonEntity.Label;

        public ComponentEmoji? Emoji { get; }

        public bool Disabled => _jsonEntity.Disabled;

        private protected Button(JsonModels.JsonComponent jsonEntity)
        {
            _jsonEntity = jsonEntity;
            if (jsonEntity.Emoji != null)
                Emoji = new(jsonEntity.Emoji);
        }

        internal static Button CreateFromJson(JsonModels.JsonComponent jsonEntity)
        {
            return (int)jsonEntity.Style! switch
            {
                5 => new LinkButton(jsonEntity),
                _ => new ActionButton(jsonEntity),
            };
        }
    }
}
