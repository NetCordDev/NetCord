namespace NetCord;

public class ActionButton : Button, IComponent
{
    public ButtonStyle Style => _jsonModel.Style.GetValueOrDefault();
    public string CustomId => _jsonModel.CustomId!;

    public ActionButton(JsonModels.JsonComponent jsonModel) : base(jsonModel)
    {
    }
}
