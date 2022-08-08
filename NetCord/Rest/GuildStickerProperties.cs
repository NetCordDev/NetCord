namespace NetCord.Rest;

public class GuildStickerProperties
{
    public GuildStickerProperties(string name, string description, IEnumerable<string> tags, Stream stream, StickerFormat format)
    {
        Name = name;
        Description = description;
        Tags = tags;
        _stream = stream;
        Format = format;
    }

    public string Name { get; }

    public string Description { get; }

    public IEnumerable<string> Tags { get; }

    public StickerFormat Format { get; }

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

    internal MultipartFormDataContent Build()
    {
        StreamContent file = new(Stream);
        file.Headers.ContentType = new(Format switch
        {
            StickerFormat.Png or StickerFormat.Apng => "image/png",
            StickerFormat.Lottie => "application/json",
            _ => throw new System.ComponentModel.InvalidEnumArgumentException(null, (int)Format, typeof(StickerFormat)),
        });
        MultipartFormDataContent content = new()
        {
            { new StringContent(Name), "name" },
            { new StringContent(Description), "description" },
            { new StringContent(string.Join(',', Tags)), "tags" },
            { file, "file", "f" },
        };
        return content;
    }
}