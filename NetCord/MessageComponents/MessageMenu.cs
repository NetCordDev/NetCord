
using NetCord.JsonModels;

namespace NetCord
{
    public class MessageMenu : IMessageComponent
    {
        private readonly JsonMessageComponent _jsonEntity;
        public MessageComponentType ComponentType => MessageComponentType.Menu;
        public string CustomId => _jsonEntity.CustomId;
        public IEnumerable<SelectOption> Options { get; }
        public string? Placeholder => _jsonEntity.Placeholder;
        public int? MinValues => _jsonEntity.MinValues;
        public int? MaxValues => _jsonEntity.MaxValues;
        public bool Disabled => _jsonEntity.Disabled;

        internal MessageMenu(JsonMessageComponent jsonEntity)
        {
            _jsonEntity = jsonEntity.Components[0];
            Options = _jsonEntity.Options.SelectOrEmpty(o => new SelectOption(o));
        }

        public class SelectOption
        {
            private readonly JsonMessageComponent.SelectOption _jsonEntity;

            public string Label => _jsonEntity.Label;
            public string Value => _jsonEntity.Value;
            public string? Description => _jsonEntity.Description;
            public MessageComponentEmoji? Emoji { get; }
            public bool? IsDefault => _jsonEntity.IsDefault;

            internal SelectOption(JsonMessageComponent.SelectOption jsonEntity)
            {
                _jsonEntity = jsonEntity;
                Emoji = new(jsonEntity.Emoji);
            }
        }
    }
}
