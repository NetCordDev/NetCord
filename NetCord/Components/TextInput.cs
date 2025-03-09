using NetCord.JsonModels;

namespace NetCord;

public class TextInput(JsonComponent jsonModel, int parentId) : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public string CustomId => jsonModel.CustomId!;
    public string Value => jsonModel.Value!;
    public int ParentId => parentId;
}
