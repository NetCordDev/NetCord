using NetCord.JsonModels;

namespace NetCord;

public class TextInput(JsonComponent jsonModel) : ILabelComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public string CustomId => jsonModel.CustomId!;
    public string Value => jsonModel.Value!;
}
