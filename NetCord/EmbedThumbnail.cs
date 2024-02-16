namespace NetCord;

/// <summary>
/// Contains information used for the rendering and display of thumbnails in embeds.
/// </summary>
public class EmbedThumbnail(JsonModels.JsonEmbedThumbnail jsonModel) : IJsonModel<JsonModels.JsonEmbedThumbnail>
{
    JsonModels.JsonEmbedThumbnail IJsonModel<JsonModels.JsonEmbedThumbnail>.JsonModel => jsonModel;

    /// <summary>
    /// The URL of the image displayed as the thumbnail in the embed.
    /// </summary>
    public string? Url => jsonModel.Url;

    /// <summary>
    /// The URL of the image displayed as the thumbnail in the embed, proxied by the Discord CDN server.
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
