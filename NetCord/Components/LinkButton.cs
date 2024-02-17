namespace NetCord;

public class LinkButton(JsonModels.JsonComponent jsonModel) : Button(jsonModel)
{
#pragma warning disable CA1822 // Mark members as static
    public ButtonStyle Style => (ButtonStyle)5;
#pragma warning restore CA1822 // Mark members as static
    public string Url => _jsonModel.Url!;
}
