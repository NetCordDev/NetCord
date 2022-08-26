namespace NetCord;

public class LinkButton : Button
{
    public ButtonStyle Style => (ButtonStyle)5;
    public string Url => _jsonModel.Url!;

    public LinkButton(JsonModels.JsonComponent jsonModel) : base(jsonModel)
    {
    }
}
