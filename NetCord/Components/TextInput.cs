using NetCord.JsonModels;

namespace NetCord;

public class TextInput(JsonTextInputComponent jsonModel) : IInteractiveComponent, ILabelComponent, IJsonModel<JsonTextInputComponent>
{
    JsonTextInputComponent IJsonModel<JsonTextInputComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public string CustomId => jsonModel.CustomId;
    public string Value => jsonModel.Value;
}
