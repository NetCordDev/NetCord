using NetCord.JsonModels;

namespace NetCord;

public class TextInput(JsonComponent jsonModel) : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => _jsonModel;
    private readonly JsonComponent _jsonModel = jsonModel.Components[0];

    public string CustomId => _jsonModel.CustomId!;
    public string Value => _jsonModel.Value!;
}
