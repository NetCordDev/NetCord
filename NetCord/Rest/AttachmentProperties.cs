namespace NetCord.Rest;

public class AttachmentProperties
{
    public string FileName { get; }
    public string? Description { get; set; }

    internal Stream Stream
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

    private readonly Stream _stream;
    private bool _read;

    public AttachmentProperties(string name, string filePath) : this(name, new StreamReader(filePath).BaseStream)
    {
    }

    public AttachmentProperties(string name, Stream stream)
    {
        FileName = name;
        _stream = stream;
    }
}