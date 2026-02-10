using NetCord.JsonModels;

namespace NetCord;

public class FileDisplay(JsonFileDisplayComponent jsonModel) : IMessageComponent, IComponentContainerComponent, IJsonModel<JsonFileDisplayComponent>
{
    JsonFileDisplayComponent IJsonModel<JsonFileDisplayComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public ComponentMedia File { get; } = new(jsonModel.File);
    public bool Spoiler => jsonModel.Spoiler;
    public string Name => jsonModel.Name;
    public int Size => jsonModel.Size;
}
