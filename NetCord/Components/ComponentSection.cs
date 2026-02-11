using NetCord.JsonModels;

namespace NetCord;

public class ComponentSection(JsonComponentSectionComponent jsonModel) : IMessageComponent, IComponentContainerComponent, IJsonModel<JsonComponentSectionComponent>
{
    JsonComponentSectionComponent IJsonModel<JsonComponentSectionComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public IComponentSectionAccessoryComponent Accessory { get; } = IComponentSectionAccessoryComponent.CreateFromJson(jsonModel.Accessory);
    public IReadOnlyList<IComponentSectionComponent> Components { get; } = jsonModel.Components.Select(IComponentSectionComponent.CreateFromJson).ToArray();
}
