using NetCord.JsonModels;

namespace NetCord;

public class TextInput : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => _jsonModel;
    private readonly JsonComponent _jsonModel;

    public ComponentType ComponentType => ComponentType.TextInput;
    public string CustomId => _jsonModel.CustomId!;
    public string Value => _jsonModel.Value!;

    public TextInput(JsonComponent jsonModel)
    {
        _jsonModel = jsonModel.Components[0];
    }
}
