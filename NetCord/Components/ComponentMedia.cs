using NetCord.JsonModels;

namespace NetCord;

public class ComponentMedia(JsonComponentMedia jsonModel) : IJsonModel<JsonComponentMedia>
{
    JsonComponentMedia IJsonModel<JsonComponentMedia>.JsonModel => jsonModel;

    /// <summary>
    /// Source URL of the media item.
    /// </summary>
    public string Url => jsonModel.Url;

    /// <summary>
    /// A proxied URL of the media item.
    /// </summary>
    public string? ProxyUrl => jsonModel.ProxyUrl;

    /// <summary>
    /// Height of the media item.
    /// </summary>
    public int? Height => jsonModel.Height;

    /// <summary>
    /// Width of the media item.
    /// </summary>
    public int? Width => jsonModel.Width;

    /// <summary>
    /// The media item's media type.
    /// </summary>
    public string? ContentType => jsonModel.ContentType;

    /// <summary>
    /// Loading state of the media item.
    /// </summary>
    public ComponentMediaLoadingState? LoadingState => jsonModel.LoadingState;

    /// <summary>
    /// The ID of the uploaded attachment. Only present if the media item was uploaded as an attachment.
    /// </summary>
    public ulong? AttachmentId => jsonModel.AttachmentId;
}
