using NetCord.JsonModels;

namespace NetCord;

public class Menu : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => _jsonModel;
    private readonly JsonComponent _jsonModel;
    public ComponentType ComponentType => ComponentType.Menu;
    public string CustomId => _jsonModel.CustomId!;
    public IEnumerable<MenuSelectOption> Options { get; }
    public string? Placeholder => _jsonModel.Placeholder;
    public int? MinValues => _jsonModel.MinValues;
    public int? MaxValues => _jsonModel.MaxValues;
    public bool Disabled => _jsonModel.Disabled.GetValueOrDefault();

    public Menu(JsonComponent jsonModel)
    {
        _jsonModel = jsonModel.Components[0];
        Options = _jsonModel.Options.Select(o => new MenuSelectOption(o));
    }
}