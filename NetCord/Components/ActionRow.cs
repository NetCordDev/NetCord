using NetCord.JsonModels;

namespace NetCord;

public class ActionRow : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => _jsonModel;
    private readonly JsonComponent _jsonModel;

    public ComponentType ComponentType => ComponentType.ActionRow;
    public string CustomId => _jsonModel.CustomId!;
    public IEnumerable<Button> Buttons { get; }

    public ActionRow(JsonComponent jsonModel)
    {
        _jsonModel = jsonModel;
        Buttons = jsonModel.Components.SelectOrEmpty(b => Button.CreateFromJson(b));
    }
}
