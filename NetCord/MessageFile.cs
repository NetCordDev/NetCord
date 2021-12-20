namespace NetCord;

public class MessageFile
{
    public string FileName { get; }
    public string? Description { get; init; }

    internal Stream _stream;

    public MessageFile(string fileName, string filePath) : this(fileName, new StreamReader(filePath).BaseStream)
    {
    }

    public MessageFile(string fileName, Stream stream)
    {
        if (stream.Length > 8_000_000)
            throw new IOException("File too large");
        FileName = fileName;
        _stream = stream;
    }
}