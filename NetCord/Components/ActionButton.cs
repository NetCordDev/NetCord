namespace NetCord;

public class ActionButton : Button
{
    public ButtonStyle Style => (ButtonStyle)_jsonEntity.Style!;
    public string CustomId => _jsonEntity.CustomId!;

    internal ActionButton(JsonModels.JsonComponent jsonEntity) : base(jsonEntity)
    {
    }
}