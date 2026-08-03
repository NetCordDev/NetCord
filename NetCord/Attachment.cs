using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a message attachment, and its contained data.
/// </summary>
public class Attachment(JsonModels.JsonAttachment jsonModel) : Entity, IJsonModel<JsonModels.JsonAttachment>
{
    JsonModels.JsonAttachment IJsonModel<JsonModels.JsonAttachment>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonAttachment _jsonModel = jsonModel;

    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// Name of the attachment (max 1024 characters for attachments sent by message, 2-30 characters for attachments used for sticker creation).
    /// </summary>
    public string FileName => _jsonModel.FileName;

    /// <summary>
    /// The title of the attachment.
    /// </summary>
    public string? Title => _jsonModel.Title;

    /// <summary>
    /// Description for the attachment (max 1024 characters for attachments sent by message, max 200 characters for attachments used for sticker creation).
    /// </summary>
    public string? Description => _jsonModel.Description;

    /// <summary>
    /// The attachment's media (MIME) type.
    /// </summary>
    public string? ContentType => _jsonModel.ContentType;

    /// <summary>
    /// The attachment's size in bytes.
    /// </summary>
    public long Size => _jsonModel.Size;

    /// <summary>
    /// The attachment's source URL.
    /// </summary>
    public string Url => _jsonModel.Url;

    /// <summary>
    /// The attachment's source URL, proxied through Discord's CDN.
    /// </summary>
    public string ProxyUrl => _jsonModel.ProxyUrl;

    /// <summary>
    /// Whether this attachment is ephemeral.
    /// </summary>
    public bool Ephemeral => _jsonModel.Ephemeral;

    /// <summary>
    /// Additional information about the attachment's type.
    /// </summary>
    public AttachmentFlags Flags => _jsonModel.Flags;

    /// <summary>
    /// Returns expiration and issue info for the attachment's source URL.
    /// </summary>
    public AttachmentExpirationInfo GetExpirationInfo() => new(Url);

    public static Attachment CreateFromJson(JsonModels.JsonAttachment jsonModel, RestClient client)
    {
        if (jsonModel.Width.HasValue)
            return new ImageAttachment(jsonModel);
        else if (jsonModel.DurationSeconds.HasValue)
            return new VoiceAttachment(jsonModel);
        else if (jsonModel.ClipCreatedAt.HasValue)
            return new ClipAttachment(jsonModel, client);
        else
            return new Attachment(jsonModel);
    }
}
