using NetCord.JsonModels;

namespace NetCord;

public class ActionRow(JsonComponent jsonModel) : IMessageComponent, IComponentContainerComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;

    public IReadOnlyList<IActionRowComponent> Components { get; } = jsonModel.Components!.Select(IActionRowComponent.CreateFromJson).ToArray();
}
