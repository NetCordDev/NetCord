using NetCord.JsonModels;

namespace NetCord;

public class Checkbox(JsonCheckboxComponent jsonModel) : IInteractiveComponent, ILabelComponent, IJsonModel<JsonCheckboxComponent>
{
    JsonCheckboxComponent IJsonModel<JsonCheckboxComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;

    public string CustomId => jsonModel.CustomId;

    public bool Checked => jsonModel.Checked;
}
