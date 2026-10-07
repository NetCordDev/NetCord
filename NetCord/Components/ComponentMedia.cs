using NetCord.JsonModels;

namespace NetCord;

public enum ComponentMediaFlags
{
    /// <summary>
    /// This image is animated.
    /// </summary>
    IsAnimated = 1 << 0,
}

public class ComponentMedia(JsonComponentMedia jsonModel)
{
    /// <summary>
    /// Source URL of the media item.
    /// </summary>
    public string Url { get; } = jsonModel.Url;

    /// <summary>
    /// A proxied URL of the media item.
    /// </summary>
    public string? ProxyUrl { get; } = jsonModel.ProxyUrl;

    /// <summary>
    /// Height of the media item.
    /// </summary>
    public int? Height { get; } = jsonModel.Height;

    /// <summary>
    /// Width of the media item.
    /// </summary>
    public int? Width { get; } = jsonModel.Width;

    /// <summary>
    /// Thumbhash placeholder of the media item.
    /// </summary>
    public string? Placeholder { get; } = jsonModel.Placeholder;

    /// <summary>
    /// Version of the thumbhash placeholder of the media item.
    /// </summary>
    public int? PlaceholderVersion { get; } = jsonModel.PlaceholderVersion;

    /// <summary>
    /// The media item's media type.
    /// </summary>
    public string? ContentType { get; } = jsonModel.ContentType;

    /// <summary>
    /// Flags of the media item.
    /// </summary>
    public ComponentMediaFlags? Flags { get; } = jsonModel.Flags;

    /// <summary>
    /// The ID of the uploaded attachment. Only present if the media item was uploaded as an attachment.
    /// </summary>
    public ulong? AttachmentId { get; } = jsonModel.AttachmentId;
}
