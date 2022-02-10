namespace NetCord;

public class Embed
{
    private readonly JsonModels.JsonEmbed _jsonEntity;

    public string? Title => _jsonEntity.Title;
    public EmbedType? Type => _jsonEntity.Type;
    public string? Description => _jsonEntity.Description;
    public string? Url => _jsonEntity.Url;
    public DateTimeOffset? Timestamp => _jsonEntity.Timestamp;
    public Color? Color => _jsonEntity.Color;
    public EmbedFooter? Footer { get; }
    public EmbedImage? Image { get; }
    public EmbedThumbnail? Thumbnail { get; }
    public EmbedVideo? Video { get; }
    public EmbedProvider? Provider { get; }
    public EmbedAuthor? Author { get; }
    public IEnumerable<EmbedField> Fields { get; }

    internal Embed(JsonModels.JsonEmbed jsonEntity)
    {
        _jsonEntity = jsonEntity;
        if (jsonEntity.Footer != null) Footer = new(jsonEntity.Footer);
        if (jsonEntity.Image != null) Image = new(jsonEntity.Image);
        if (jsonEntity.Thumbnail != null) Thumbnail = new(jsonEntity.Thumbnail);
        if (jsonEntity.Video != null) Video = new(jsonEntity.Video);
        if (jsonEntity.Provider != null) Provider = new(jsonEntity.Provider);
        if (jsonEntity.Author != null) Author = new(jsonEntity.Author);
        Fields = jsonEntity.Fields.Select(f => new EmbedField(f));
    }
}