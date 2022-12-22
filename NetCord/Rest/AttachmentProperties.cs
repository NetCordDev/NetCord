namespace NetCord.Rest;

public partial class AttachmentProperties
{
    public AttachmentProperties(string fileName, Stream stream) : this(fileName)
    {
        _stream = stream;
    }

    private protected AttachmentProperties(string fileName)
    {
        FileName = fileName;
    }

    public string FileName { get; }

    public string? Description { get; set; }

    internal Stream? Stream
    {
        get
        {
            if (_read == true)
                throw new InvalidOperationException("The attachment was already sent.");
            else
                _read = true;
            return _stream;
        }
    }

    private readonly Stream? _stream;
    private bool _read;
}
