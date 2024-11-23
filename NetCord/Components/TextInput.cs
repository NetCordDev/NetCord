using NetCord.JsonModels;

namespace NetCord;

public class TextInput(JsonComponent jsonModel) : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public string CustomId => jsonModel.CustomId!;
    public string Value => jsonModel.Value!;
}
