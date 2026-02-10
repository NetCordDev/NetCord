using NetCord.JsonModels;

namespace NetCord;

public class ComponentSeparator(JsonComponentSeparatorComponent jsonModel) : IMessageComponent, IComponentContainerComponent, IJsonModel<JsonComponentSeparatorComponent>
{
    JsonComponentSeparatorComponent IJsonModel<JsonComponentSeparatorComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public bool Divider => jsonModel.Divider.GetValueOrDefault();
    public ComponentSeparatorSpacingSize Spacing => jsonModel.Spacing.GetValueOrDefault();
}
