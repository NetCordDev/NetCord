using NetCord.JsonModels;

namespace NetCord;

public class CheckboxGroup(JsonCheckboxGroupComponent jsonModel) : IInteractiveComponent, ILabelComponent, IJsonModel<JsonCheckboxGroupComponent>
{
    JsonCheckboxGroupComponent IJsonModel<JsonCheckboxGroupComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;

    public string CustomId => jsonModel.CustomId;

    public IReadOnlyList<string> CheckedValues => jsonModel.CheckedValues;
}
