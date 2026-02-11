using NetCord.JsonModels;

namespace NetCord;

public class Thumbnail(JsonThumbnailComponent jsonModel) : IComponentSectionAccessoryComponent, IJsonModel<JsonThumbnailComponent>
{
    JsonThumbnailComponent IJsonModel<JsonThumbnailComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public ComponentMedia Media { get; } = new(jsonModel.Media);
    public string? Description => jsonModel.Description;
    public bool Spoiler => jsonModel.Spoiler.GetValueOrDefault();
}
