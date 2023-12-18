namespace NetCord;

/// <summary>
/// Contains information used for the rendering and display of images in embeds.
/// </summary>
public class EmbedImage : IJsonModel<JsonModels.JsonEmbedImage>
{
    JsonModels.JsonEmbedImage IJsonModel<JsonModels.JsonEmbedImage>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedImage _jsonModel;

    public EmbedImage(JsonModels.JsonEmbedImage jsonModel)
    {
        _jsonModel = jsonModel;
    }

    /// <summary>
    /// The URL of the image displayed in the embed.
    /// </summary>
    public string? Url => _jsonModel.Url;
    
    /// <summary>
    /// The URL of the image, proxied by the Discord CDN server.
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
