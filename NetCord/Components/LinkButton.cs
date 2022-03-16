namespace NetCord;

public class LinkButton : Button
{
    public ButtonStyle Style => (ButtonStyle)5;
    public string Url => _jsonEntity.Url!;

    internal LinkButton(JsonModels.JsonComponent jsonEntity) : base(jsonEntity)
    {

    }
}