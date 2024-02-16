namespace NetCord;

public class LinkButton(JsonModels.JsonComponent jsonModel) : Button(jsonModel)
{
    public ButtonStyle Style => (ButtonStyle)5;
    public string Url => _jsonModel.Url!;
}
