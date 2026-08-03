using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace NetCord.Rest;

/// <inheritdoc cref="Attachment"/>
[GenerateMethodsForProperties]
public partial class AttachmentProperties : IHttpSerializable, IJsonSerializable<AttachmentProperties, int>
{
    private static readonly JsonEncodedText _id = JsonEncodedText.Encode("id");
    private static readonly JsonEncodedText _fileName = JsonEncodedText.Encode("filename");
    private static readonly JsonEncodedText _title = JsonEncodedText.Encode("title");
    private static readonly JsonEncodedText _description = JsonEncodedText.Encode("description");

    /// <summary>
    /// Creates an attachment from the provided stream, with the given filename.
    /// </summary>
    /// <param name="fileName"><inheritdoc cref="Attachment.FileName" path="/summary"/></param>
    /// <param name="stream">A stream containing the attachment's contents.</param>
    public AttachmentProperties(string fileName, Stream stream) : this(fileName)
    {
        _stream = stream;
    }

    /// <summary>
    /// Creates an empty attachment with the given filename.
    /// </summary>
    /// <param name="fileName"><inheritdoc cref="Attachment.FileName" path="/summary"/></param>
    protected AttachmentProperties(string fileName)
    {
        FileName = fileName;
    }

    /// <inheritdoc cref="Attachment.FileName"/>
    public string FileName { get; set; }

    /// <inheritdoc cref="Attachment.Title"/>
    public string? Title { get; set; }

    /// <inheritdoc cref="Attachment.Description"/>
    public string? Description { get; set; }

    protected Stream? GetStream()
    {
        if (Interlocked.Exchange(ref _read, 1) is 1)
            ThrowAttachmentAlreadySent();

        return _stream;
    }

    [DoesNotReturn]
    [StackTraceHidden]
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
/// Represents an attachment with Base64 encoded contents.
/// </summary>
/// <param name="fileName"><inheritdoc cref="Attachment.FileName" path="/summary"/></param>
/// <param name="stream">A stream containing the attachment's contents, encoded in Base64.</param>
[GenerateMethodsForProperties]
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
/// Represents an attachment with quoted-printable encoded contents.
/// </summary>
/// <param name="fileName"><inheritdoc cref="Attachment.FileName" path="/summary"/></param>
/// <param name="stream">A stream containing the attachment's contents, encoded in quoted-printable.</param>
[GenerateMethodsForProperties]
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
/// Represents an attachment hosted on Google Cloud.
/// </summary>
/// <param name="fileName"><inheritdoc cref="Attachment.FileName" path="/summary"/></param>
/// <param name="uploadedFileName"><inheritdoc cref="UploadedFileName" path="/summary"/></param>
[GenerateMethodsForProperties]
public partial class GoogleCloudPlatformAttachmentProperties(string fileName, string uploadedFileName) : AttachmentProperties(fileName)
{
    private static readonly JsonEncodedText _uploadedFileName = JsonEncodedText.Encode("uploaded_filename");

    /// <summary>
    /// The filename to use for the upload.
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
