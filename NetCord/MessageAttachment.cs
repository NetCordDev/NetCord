namespace NetCord;

public class MessageAttachment
{
    public string FileName { get; }
    public string? Description { get; init; }

    internal Stream Stream
    {
        get
        {
            if (_read == true)
                throw new InvalidOperationException("The attachment was already sent");
            else
                _read = true;
            return _stream;
        }
    }

    private readonly Stream _stream;
    private bool _read;

    public MessageAttachment(string name, string filePath) : this(name, new StreamReader(filePath).BaseStream)
    {
    }

    public MessageAttachment(string name, Stream stream)
    {
        FileName = name;
        _stream = stream;
    }
}