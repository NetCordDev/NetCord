namespace NetCord;

/// <summary>
/// Contains information used for the rendering and display of videos in embeds.
/// </summary>
public class EmbedVideo(JsonModels.JsonEmbedVideo jsonModel) : IJsonModel<JsonModels.JsonEmbedVideo>
{
    JsonModels.JsonEmbedVideo IJsonModel<JsonModels.JsonEmbedVideo>.JsonModel => jsonModel;

    /// <summary>
    /// The URL of the video displayed in the embed.
    /// </summary>
    public string? Url => jsonModel.Url;

    /// <summary>
    /// The URL of the video displayed in the embed, proxied by the Discord CDN server.
    /// </summary>
    public string? ProxyUrl => jsonModel.ProxyUrl;

    /// <summary>
    /// The height of the video in pixels.
    /// </summary>
    public int? Height => jsonModel.Height;

    /// <summary>
    /// The width of the video in pixels.
    /// </summary>
    public int? Width => jsonModel.Width;
}
