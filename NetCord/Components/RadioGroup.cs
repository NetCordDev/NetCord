using NetCord.JsonModels;

namespace NetCord;

internal class RadioGroup(JsonComponent jsonModel) : IInteractiveComponent, ILabelComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public string CustomId => jsonModel.CustomId!;
    //public IReadOnlyList<RadioGroupOption> Options { get; }
}
