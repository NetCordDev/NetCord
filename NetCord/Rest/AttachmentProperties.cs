namespace NetCord.Rest;

public partial class AttachmentProperties
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="stream">Content of the file.</param>
    public AttachmentProperties(string fileName, Stream stream) : this(fileName)
    {
        _stream = stream;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    private protected AttachmentProperties(string fileName)
    {
        FileName = fileName;
    }

    /// <summary>
    /// Name of the file.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Description for the file (max 1024 characters).
    /// </summary>
    public string? Description { get; set; }

    internal Stream? Stream
    {
        get
        {
            if (_read)
                throw new InvalidOperationException("The attachment was already sent.");
            else
                _read = true;
            return _stream;
        }
    }

    private readonly Stream? _stream;
    private bool _read;
}
