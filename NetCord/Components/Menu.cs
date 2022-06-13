using NetCord.JsonModels;

namespace NetCord;

public class Menu : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => _jsonModel;
    private readonly JsonComponent _jsonModel;
    public ComponentType ComponentType => ComponentType.Menu;
    public string CustomId => _jsonModel.CustomId!;
    public IEnumerable<SelectOption> Options { get; }
    public string? Placeholder => _jsonModel.Placeholder;
    public int? MinValues => _jsonModel.MinValues;
    public int? MaxValues => _jsonModel.MaxValues;
    public bool Disabled => _jsonModel.Disabled;

    public Menu(JsonComponent jsonModel)
    {
        _jsonModel = jsonModel.Components[0];
        Options = _jsonModel.Options.SelectOrEmpty(o => new SelectOption(o));
    }

    public class SelectOption
    {
        private readonly JsonComponent.SelectOption _jsonModel;

        public string Label => _jsonModel.Label;
        public string Value => _jsonModel.Value;
        public string? Description => _jsonModel.Description;
        public ComponentEmoji? Emoji { get; }
        public bool? IsDefault => _jsonModel.IsDefault;

        public SelectOption(JsonComponent.SelectOption jsonModel)
        {
            _jsonModel = jsonModel;
            if (jsonModel.Emoji != null)
                Emoji = new(jsonModel.Emoji);
        }
    }
}