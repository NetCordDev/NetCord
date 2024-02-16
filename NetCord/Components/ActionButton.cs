namespace NetCord;

public class ActionButton(JsonModels.JsonComponent jsonModel) : Button(jsonModel), IComponent
{
    public ButtonStyle Style => _jsonModel.Style.GetValueOrDefault();
    public string CustomId => _jsonModel.CustomId!;
}
