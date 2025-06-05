using NetCord.JsonModels;

namespace NetCord;

public class MediaGallery(JsonComponent jsonModel) : IComponent, IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public IReadOnlyList<MediaGalleryItem> Items { get; } = jsonModel.Items!.Select(c => new MediaGalleryItem(c)).ToArray();
}

public class MediaGalleryItem(JsonComponent jsonModel) : IJsonModel<JsonComponent>
{
    JsonComponent IJsonModel<JsonComponent>.JsonModel => jsonModel;

    public ComponentMedia Media { get; } = new(jsonModel.Media!);
    public string? Description => jsonModel.Description;
    public bool Spoiler => jsonModel.Spoiler.GetValueOrDefault();
}
