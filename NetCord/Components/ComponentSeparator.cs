using NetCord.JsonModels;

namespace NetCord;

public class ComponentSeparator(JsonComponentSeparatorComponent jsonModel) : IMessageComponent, IComponentContainerComponent, IJsonModel<JsonComponentSeparatorComponent>
{
    JsonComponentSeparatorComponent IJsonModel<JsonComponentSeparatorComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public bool Divider => jsonModel.Divider;
    public ComponentSeparatorSpacingSize Spacing => jsonModel.Spacing;
}
