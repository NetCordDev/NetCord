using NetCord.JsonModels;

namespace NetCord;

public class ComponentSeparator(JsonComponent jsonModel) : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public bool Divider => jsonModel.Divider.GetValueOrDefault();
    public ComponentSeparatorSpacingSize Spacing => jsonModel.Spacing.GetValueOrDefault();
}
