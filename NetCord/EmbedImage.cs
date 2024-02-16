namespace NetCord;

/// <summary>
/// Contains information used for the rendering and display of images in embeds.
/// </summary>
public class EmbedImage(JsonModels.JsonEmbedImage jsonModel) : IJsonModel<JsonModels.JsonEmbedImage>
{
    JsonModels.JsonEmbedImage IJsonModel<JsonModels.JsonEmbedImage>.JsonModel => jsonModel;

    /// <summary>
    /// The URL of the image displayed in the embed.
    /// </summary>
    public string? Url => jsonModel.Url;

    /// <summary>
    /// The URL of the image, proxied by the Discord CDN server.
    /// </summary>
    public string? ProxyUrl => jsonModel.ProxyUrl;

    /// <summary>
    /// The height of the image in pixels.
    /// </summary>
    public int? Height => jsonModel.Height;

    /// <summary>
    /// The width of the image in pixels.
    /// </summary>
    public int? Width => jsonModel.Width;
}
