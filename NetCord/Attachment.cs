using System.Net.Mime;

namespace NetCord;

public class Attachment(JsonModels.JsonAttachment jsonModel) : Entity, IJsonModel<JsonModels.JsonAttachment>
{
    JsonModels.JsonAttachment IJsonModel<JsonModels.JsonAttachment>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonAttachment _jsonModel = jsonModel;

    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// Name of the attachment.
    /// </summary>
    public string FileName => _jsonModel.FileName;

    /// <summary>
    /// Title of the attachment.
    /// </summary>
    public string? Title => _jsonModel.Title;

    /// <summary>
    /// Description for the attachment (max 1024 characters).
    /// </summary>
    public string? Description => _jsonModel.Description;

    /// <summary>
    /// The attachment's media type.
    /// </summary>
    public ContentType? ContentType => _jsonModel.ContentType;

    /// <summary>
    /// Size of file in bytes.
    /// </summary>
    public int Size => _jsonModel.Size;

    /// <summary>
    /// Source url of file.
    /// </summary>
    public string Url => _jsonModel.Url;

    /// <summary>
    /// A proxied url of file.
    /// </summary>
    public string ProxyUrl => _jsonModel.ProxyUrl;

    /// <summary>
    /// Whether this attachment is ephemeral.
    /// </summary>
    public bool Ephemeral => _jsonModel.Ephemeral;

    public AttachmentFlags Flags => _jsonModel.Flags;

    public AttachmentExpirationInfo GetExpirationInfo() => new(Url);

    public static Attachment CreateFromJson(JsonModels.JsonAttachment jsonModel)
    {
        if (jsonModel.Width.HasValue)
            return new ImageAttachment(jsonModel);
        else if (jsonModel.DurationSeconds.HasValue)
            return new VoiceAttachment(jsonModel);
        else
            return new Attachment(jsonModel);
    }
}
