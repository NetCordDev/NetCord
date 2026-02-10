using NetCord.JsonModels;

namespace NetCord;

public class ActionRow(JsonActionRowComponent jsonModel) : IMessageComponent, IComponentContainerComponent, IJsonModel<JsonActionRowComponent>
{
    JsonActionRowComponent IJsonModel<JsonActionRowComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;

    public IReadOnlyList<IActionRowComponent> Components { get; } = jsonModel.Components!.Select(IActionRowComponent.CreateFromJson).ToArray();
}
