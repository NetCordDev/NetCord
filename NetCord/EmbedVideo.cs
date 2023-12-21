namespace NetCord;

/// <summary>
/// Contains information used for the rendering and display of videos in embeds.
/// </summary>
public class EmbedVideo : IJsonModel<JsonModels.JsonEmbedVideo>
{
    JsonModels.JsonEmbedVideo IJsonModel<JsonModels.JsonEmbedVideo>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbedVideo _jsonModel;
    public EmbedVideo(JsonModels.JsonEmbedVideo jsonModel)
    {
        _jsonModel = jsonModel;
    }

    /// <summary>
    /// The URL of the video displayed in the embed.
    /// </summary>
    public string? Url => _jsonModel.Url;

    /// <summary>
    /// The URL of the video displayed in the embed, proxied by the Discord CDN server.
    /// </summary>
    public string? ProxyUrl => _jsonModel.ProxyUrl;

    /// <summary>
    /// The height of the video in pixels.
    /// </summary>
    public int? Height => _jsonModel.Height;

    /// <summary>
    /// The width of the video in pixels.
    /// </summary>
    public int? Width => _jsonModel.Width;
}
