using NetCord.JsonModels;

namespace NetCord;

public class TextDisplay(JsonTextDisplayComponent jsonModel) : IMessageComponent, IModalComponent, IComponentContainerComponent, IComponentSectionComponent, IJsonModel<JsonTextDisplayComponent>
{
    JsonTextDisplayComponent IJsonModel<JsonTextDisplayComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public string Content { get; } = jsonModel.Content ?? string.Empty;
}
