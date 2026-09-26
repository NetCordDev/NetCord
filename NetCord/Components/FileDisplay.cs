using NetCord.JsonModels;

namespace NetCord;

public class FileDisplay(JsonFileDisplayComponent jsonModel) : IMessageComponent, IComponentContainerComponent, IJsonModel<JsonFileDisplayComponent>
{
    JsonFileDisplayComponent IJsonModel<JsonFileDisplayComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public ComponentMedia File { get; } = new(jsonModel.File);
    public bool Spoiler => jsonModel.Spoiler.GetValueOrDefault();
    public string Name => jsonModel.Name ?? string.Empty;
    public int Size => jsonModel.Size.GetValueOrDefault();
}
