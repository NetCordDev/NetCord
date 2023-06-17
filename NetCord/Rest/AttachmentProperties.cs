namespace NetCord.Rest;

public class AttachmentProperties : IHttpSerializable
{
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
    /// Description for the file (max 1024 characters for attachments sent by message, max 200 characters for attachments used for sticker creation).
    /// </summary>
    public string? Description { get; set; }

    protected Stream? GetStream()
    {
        if (_read)
            throw new InvalidOperationException("The attachment has already been sent.");
        else
            _read = true;

        return _stream;
    }

    private readonly Stream? _stream;
    private bool _read;

    public virtual bool SupportsHttpSerialization => true;

    public virtual HttpContent Serialize() => new StreamContent(GetStream()!);

    internal static void AddAttachments(MultipartFormDataContent content, IEnumerable<AttachmentProperties>? attachments)
    {
        if (attachments is not null)
        {
            int i = 0;
            foreach (var attachment in attachments)
            {
                if (attachment.SupportsHttpSerialization)
                    content.Add(attachment.Serialize(), $"files[{i}]", attachment.FileName);
                i++;
            }
        }
    }
}

public class Base64AttachmentProperties : AttachmentProperties
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName">Name of the file (max 1024 characters for attachments sent by message, 2-30 characters for attachments used for sticker creation).</param>
    /// <param name="stream">Content of the file encoded in Base64.</param>
    public Base64AttachmentProperties(string fileName, Stream stream) : base(fileName, stream)
    {
    }

    public override HttpContent Serialize()
    {
        var content = base.Serialize();
        content.Headers.Add("Content-Transfer-Encoding", "base64");
        return content;
    }
}

public class QuotedPrintableAttachmentProperties : AttachmentProperties
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName">Name of the file (max 1024 characters for attachments sent by message, 2-30 characters for attachments used for sticker creation).</param>
    /// <param name="stream">Content of the file encoded in Quoted-Printable.</param>
    public QuotedPrintableAttachmentProperties(string fileName, Stream stream) : base(fileName, stream)
    {
    }

    public override HttpContent Serialize()
    {
        var content = base.Serialize();
        content.Headers.Add("Content-Transfer-Encoding", "quoted-printable");
        return content;
    }
}

public class GoogleCloudPlatformAttachmentProperties : AttachmentProperties
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName">Name of the file (max 1024 characters for attachments sent by message, 2-30 characters for attachments used for sticker creation).</param>
    /// <param name="uploadedFileName">Name of the upload.</param>
    public GoogleCloudPlatformAttachmentProperties(string fileName, string uploadedFileName) : base(fileName)
    {
        UploadedFileName = uploadedFileName;
    }

    /// <summary>
    /// Name of the upload.
    /// </summary>
    public string UploadedFileName { get; set; }

    public override bool SupportsHttpSerialization => false;

    public override HttpContent Serialize()
    {
        throw new NotSupportedException($"'{nameof(GoogleCloudPlatformAttachmentProperties)}' does not support HTTP serialization.");
    }
}
