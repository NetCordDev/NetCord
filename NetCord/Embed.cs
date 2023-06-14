namespace NetCord;

public class Embed : IJsonModel<JsonModels.JsonEmbed>
{
    JsonModels.JsonEmbed IJsonModel<JsonModels.JsonEmbed>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonEmbed _jsonModel;

    public string? Title => _jsonModel.Title;
    public EmbedType? Type => _jsonModel.Type;
    public string? Description => _jsonModel.Description;
    public string? Url => _jsonModel.Url;
    public DateTimeOffset? Timestamp => _jsonModel.Timestamp;
    public Color? Color => _jsonModel.Color;
    public EmbedFooter? Footer { get; }
    public EmbedImage? Image { get; }
    public EmbedThumbnail? Thumbnail { get; }
    public EmbedVideo? Video { get; }
    public EmbedProvider? Provider { get; }
    public EmbedAuthor? Author { get; }
    public IReadOnlyList<EmbedField> Fields { get; }

    public Embed(JsonModels.JsonEmbed jsonModel)
    {
        _jsonModel = jsonModel;

        var footer = jsonModel.Footer;
        if (footer is not null)
            Footer = new(footer);

        var image = jsonModel.Image;
        if (image is not null)
            Image = new(image);

        var thumbnail = jsonModel.Thumbnail;
        if (thumbnail is not null)
            Thumbnail = new(thumbnail);

        var video = jsonModel.Video;
        if (video is not null)
            Video = new(video);

        var provider = jsonModel.Provider;
        if (provider is not null)
            Provider = new(provider);

        var author = jsonModel.Author;
        if (author is not null)
            Author = new(author);

        Fields = jsonModel.Fields.SelectOrEmpty(f => new EmbedField(f)).ToArray();
    }
}
