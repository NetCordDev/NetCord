using NetCord.JsonModels;

namespace NetCord;

public class TextDisplay(JsonComponent jsonModel) : IMessageComponent, IModalComponent, IComponentContainerComponent, IComponentSectionComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public string Content { get; } = jsonModel.Content ?? string.Empty;
}
