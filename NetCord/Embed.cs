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
        if (jsonModel.Footer != null)
            Footer = new(jsonModel.Footer);
        if (jsonModel.Image != null)
            Image = new(jsonModel.Image);
        if (jsonModel.Thumbnail != null)
            Thumbnail = new(jsonModel.Thumbnail);
        if (jsonModel.Video != null)
            Video = new(jsonModel.Video);
        if (jsonModel.Provider != null)
            Provider = new(jsonModel.Provider);
        if (jsonModel.Author != null)
            Author = new(jsonModel.Author);
        Fields = jsonModel.Fields.SelectOrEmpty(f => new EmbedField(f)).ToArray();
    }
}
