namespace NetCord;

/// <summary>
/// Contains information used for the rendering and display of thumbnails in embeds.
/// </summary>
public class EmbedThumbnail : IJsonModel<JsonModels.JsonEmbedThumbnail>
{
    JsonModels.JsonEmbedThumbnail IJsonModel<JsonModels.JsonEmbedThumbnail>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedThumbnail _jsonModel;
    public EmbedThumbnail(JsonModels.JsonEmbedThumbnail jsonModel)
    {
        _jsonModel = jsonModel;
    }

    /// <summary>
    /// The URL of the image displayed as the thumbnail in the embed.
    /// </summary>
    public string? Url => _jsonModel.Url;
    
    /// <summary>
    /// The URL of the image displayed as the thumbnail in the embed, proxied by the discord CDN server.
    /// </summary>
    public string? ProxyUrl => _jsonModel.ProxyUrl;
    
    /// <summary>
    /// The height of the image in pixels.
    /// </summary>
    public int? Height => _jsonModel.Height;
    
    /// <summary>
    /// The width of the image in pixels.
    /// </summary>
    public int? Width => _jsonModel.Width;
}
