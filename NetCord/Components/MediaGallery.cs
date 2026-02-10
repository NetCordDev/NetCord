using NetCord.JsonModels;

namespace NetCord;

public class MediaGallery(JsonMediaGalleryComponent jsonModel) : IMessageComponent, IComponentContainerComponent, IJsonModel<JsonMediaGalleryComponent>
{
    JsonMediaGalleryComponent IJsonModel<JsonMediaGalleryComponent>.JsonModel => jsonModel;

    public int Id => jsonModel.Id;
    public IReadOnlyList<MediaGalleryItem> Items { get; } = jsonModel.Items.Select(c => new MediaGalleryItem(c)).ToArray();
}

public class MediaGalleryItem(JsonMediaGalleryItem jsonModel) : IJsonModel<JsonMediaGalleryItem>
{
    JsonMediaGalleryItem IJsonModel<JsonMediaGalleryItem>.JsonModel => jsonModel;

    public ComponentMedia Media { get; } = new(jsonModel.Media);
    public string? Description => jsonModel.Description;
    public bool Spoiler => jsonModel.Spoiler.GetValueOrDefault();
}
