using NetCord.JsonModels;

namespace NetCord;

public class ActionRow(JsonComponent jsonModel) : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public ComponentType ComponentType => ComponentType.ActionRow;
    public string CustomId => jsonModel.CustomId!;
    public IReadOnlyList<Button> Buttons { get; } = jsonModel.Components.SelectOrEmpty(Button.CreateFromJson).ToArray();
}
