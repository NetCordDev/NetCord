using NetCord.JsonModels;

namespace NetCord;

public class TextInput : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => _jsonModel;
    private readonly JsonComponent _jsonModel;

    public ComponentType ComponentType => ComponentType.TextInput;
    public string CustomId => _jsonModel.CustomId!;
    public string? Placeholder => _jsonModel.Placeholder;
    public string? Label => _jsonModel.Label!;
    public int? MinLength => _jsonModel.MinLength;
    public int? MaxLength => _jsonModel.MaxLength;
    public bool? Required => _jsonModel.Required;
    public string Value => _jsonModel.Value;

    public TextInput(JsonComponent jsonModel)
    {
        _jsonModel = jsonModel.Components[0];
    }
}
