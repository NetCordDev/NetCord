using System.Text.Json.Serialization;

namespace NetCord.Rest;

public partial class GuildStickerProperties
{
    public GuildStickerProperties(string name, string description, IEnumerable<string> tags, Stream stream, StickerFormat format)
    {
        Name = name;
        Description = description;
        Tags = tags;
        _stream = stream;
        Format = format;
    }

    public string Name { get; set; }

    public string Description { get; set; }

    public IEnumerable<string> Tags { get; set; }

    public StickerFormat Format { get; set; }

    internal Stream Stream
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

    private readonly Stream _stream;
    private bool _read;

    internal HttpContent Build()
    {
        StreamContent file = new(Stream);
        file.Headers.ContentType = new(Format switch
        {
            StickerFormat.Png or StickerFormat.Apng => "image/png",
            StickerFormat.Lottie => "application/json",
            StickerFormat.Gif => "image/gif",
            _ => throw new System.ComponentModel.InvalidEnumArgumentException(null, (int)Format, typeof(StickerFormat)),
        });
        return new MultipartFormDataContent()
        {
            { new StringContent(Name), "name" },
            { new StringContent(Description), "description" },
            { new StringContent(string.Join(',', Tags)), "tags" },
            { file, "file", "f" },
        };
    }

    [JsonSerializable(typeof(GuildStickerProperties))]
    public partial class GuildStickerPropertiesSerializerContext : JsonSerializerContext
    {
        public static GuildStickerPropertiesSerializerContext WithOptions { get; } = new(Serialization.Options);
    }
}
