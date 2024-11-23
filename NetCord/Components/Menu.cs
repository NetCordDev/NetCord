using NetCord.JsonModels;

namespace NetCord;

public abstract class Menu(JsonComponent jsonModel) : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => _jsonModel;
    private protected readonly JsonComponent _jsonModel = jsonModel;

    public string CustomId => _jsonModel.CustomId!;
    public string? Placeholder => _jsonModel.Placeholder;
    public int? MinValues => _jsonModel.MinValues;
    public int? MaxValues => _jsonModel.MaxValues;
    public bool Disabled => _jsonModel.Disabled.GetValueOrDefault();
}
