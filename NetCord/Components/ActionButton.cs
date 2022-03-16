namespace NetCord;

public class ActionButton : Button, IComponent
{
    public ButtonStyle Style => _jsonEntity.Style.GetValueOrDefault();
    public string CustomId => _jsonEntity.CustomId!;

    internal ActionButton(JsonModels.JsonComponent jsonEntity) : base(jsonEntity)
    {
    }
}