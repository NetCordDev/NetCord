using NetCord.JsonModels;

namespace NetCord;

public class FileDisplay(JsonComponent jsonModel) : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public ComponentMedia File { get; } = new(jsonModel.File!);
    public bool Spoiler => jsonModel.Spoiler.GetValueOrDefault();
}
