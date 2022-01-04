
using NetCord.JsonModels;

namespace NetCord
{
    public class Menu : IComponent
    {
        private readonly JsonComponent _jsonEntity;
        public ComponentType ComponentType => ComponentType.Menu;
        public string CustomId => _jsonEntity.CustomId!;
        public IEnumerable<SelectOption> Options { get; }
        public string? Placeholder => _jsonEntity.Placeholder;
        public int? MinValues => _jsonEntity.MinValues;
        public int? MaxValues => _jsonEntity.MaxValues;
        public bool Disabled => _jsonEntity.Disabled;

        internal Menu(JsonComponent jsonEntity)
        {
            _jsonEntity = jsonEntity.Components[0];
            Options = _jsonEntity.Options.SelectOrEmpty(o => new SelectOption(o));
        }

        public class SelectOption
        {
            private readonly JsonComponent.SelectOption _jsonEntity;

            public string Label => _jsonEntity.Label;
            public string Value => _jsonEntity.Value;
            public string? Description => _jsonEntity.Description;
            public ComponentEmoji? Emoji { get; }
            public bool? IsDefault => _jsonEntity.IsDefault;

            internal SelectOption(JsonComponent.SelectOption jsonEntity)
            {
                _jsonEntity = jsonEntity;
                if (jsonEntity.Emoji != null) Emoji = new(jsonEntity.Emoji);
            }
        }
    }
}
