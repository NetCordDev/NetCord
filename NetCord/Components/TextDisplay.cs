using NetCord.JsonModels;

namespace NetCord;

public class TextDisplay(JsonComponent jsonModel) : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public string Content => jsonModel.Content!;
}
