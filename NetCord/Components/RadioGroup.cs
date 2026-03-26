using NetCord.JsonModels;

namespace NetCord;

public class RadioGroup(JsonRadioGroupComponent jsonModel) : IInteractiveComponent, ILabelComponent, IJsonModel<JsonRadioGroupComponent>
{
    JsonRadioGroupComponent IJsonModel<JsonRadioGroupComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;

    public string CustomId => jsonModel.CustomId;

    public string? SelectedValue => jsonModel.SelectedValue;
}
