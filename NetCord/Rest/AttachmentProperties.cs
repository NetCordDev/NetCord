using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace NetCord.Rest;

public partial class AttachmentProperties : IHttpSerializable, IJsonSerializable<AttachmentProperties, int>
{
    private static readonly JsonEncodedText _id = JsonEncodedText.Encode("id");
    private static readonly JsonEncodedText _fileName = JsonEncodedText.Encode("filename");
    private static readonly JsonEncodedText _title = JsonEncodedText.Encode("title");
    private static readonly JsonEncodedText _description = JsonEncodedText.Encode("description");

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName">Name of the file (max 1024 characters for attachments sent by message, 2-30 characters for attachments used for sticker creation).</param>
    /// <param name="stream">Content of the file.</param>
    public AttachmentProperties(string fileName, Stream stream) : this(fileName)
    {
        _stream = stream;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName">Name of the file (max 1024 characters for attachments sent by message, 2-30 characters for attachments used for sticker creation).</param>
    protected AttachmentProperties(string fileName)
    {
        FileName = fileName;
    }

    /// <summary>
    /// Name of the file (max 1024 characters for attachments sent by message, 2-30 characters for attachments used for sticker creation).
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Title of the attachment.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Description for the file (max 1024 characters for attachments sent by message, max 200 characters for attachments used for sticker creation).
    /// </summary>
    public string? Description { get; set; }

    protected Stream? GetStream()
    {
        if (Interlocked.Exchange(ref _read, 1) is 1)
            ThrowAttachmentAlreadySent();

        return _stream;
    }

    [DoesNotReturn]
    private static void ThrowAttachmentAlreadySent()
    {
        throw new InvalidOperationException("The attachment has already been sent.");
    }

    private readonly Stream? _stream;
    private byte _read;

    public virtual bool SupportsHttpSerialization => true;

    public virtual HttpContent Serialize() => new StreamContent(GetStream()!);

    private protected void WriteCommonProperties(Utf8JsonWriter writer, int attachmentId)
    {
        writer.WriteNumber(_id, attachmentId);

        writer.WriteString(_fileName, FileName);

        var title = Title;
        if (title is not null)
            writer.WriteString(_title, title);

        var description = Description;
        if (description is not null)
            writer.WriteString(_description, description);
    }

    void IJsonSerializable<AttachmentProperties, int>.WriteTo(Utf8JsonWriter writer, int attachmentId) => WriteTo(writer, attachmentId);

    protected internal virtual void WriteTo(Utf8JsonWriter writer, int attachmentId)
    {
        writer.WriteStartObject();

        WriteCommonProperties(writer, attachmentId);

        writer.WriteEndObject();
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="fileName">Name of the file (max 1024 characters for attachments sent by message, 2-30 characters for attachments used for sticker creation).</param>
/// <param name="stream">Content of the file encoded in Base64.</param>
public partial class Base64AttachmentProperties(string fileName, Stream stream) : AttachmentProperties(fileName, stream)
{
    public override HttpContent Serialize()
    {
        var content = base.Serialize();
        content.Headers.Add("Content-Transfer-Encoding", "base64");
        return content;
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="fileName">Name of the file (max 1024 characters for attachments sent by message, 2-30 characters for attachments used for sticker creation).</param>
/// <param name="stream">Content of the file encoded in Quoted-Printable.</param>
public partial class QuotedPrintableAttachmentProperties(string fileName, Stream stream) : AttachmentProperties(fileName, stream)
{
    public override HttpContent Serialize()
    {
        var content = base.Serialize();
        content.Headers.Add("Content-Transfer-Encoding", "quoted-printable");
        return content;
    }
}

/// <summary>
/// 
/// </summary>
/// <param name="fileName">Name of the file (max 1024 characters for attachments sent by message, 2-30 characters for attachments used for sticker creation).</param>
/// <param name="uploadedFileName">Name of the upload.</param>
public partial class GoogleCloudPlatformAttachmentProperties(string fileName, string uploadedFileName) : AttachmentProperties(fileName)
{
    private static readonly JsonEncodedText _uploadedFileName = JsonEncodedText.Encode("uploaded_filename");

    /// <summary>
    /// Name of the upload.
    /// </summary>
    public string UploadedFileName { get; set; } = uploadedFileName;

    public override bool SupportsHttpSerialization => false;

    public override HttpContent Serialize()
    {
        throw new NotSupportedException($"'{nameof(GoogleCloudPlatformAttachmentProperties)}' does not support HTTP serialization.");
    }

    protected internal override void WriteTo(Utf8JsonWriter writer, int attachmentId)
    {
        writer.WriteStartObject();

        WriteCommonProperties(writer, attachmentId);

        writer.WriteString(_uploadedFileName, UploadedFileName);

        writer.WriteEndObject();
    }
}
