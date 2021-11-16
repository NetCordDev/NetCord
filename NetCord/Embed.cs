using System.Text.Json.Serialization;

namespace NetCord;

public class EmbedBuilder
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public Color? Color { get; set; }
    public EmbedFooter Footer { get; set; }
    public string ImageUrl { get; set; }
    public string ThumbnailUrl { get; set; }
    public EmbedAuthor Author { get; set; }
    public List<EmbedField> Fields { get; set; }

    public Embed Build()
        => new(Title, Description, Url, Timestamp, Color, Footer, new(ImageUrl), new(ThumbnailUrl), Author, Fields);
}

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public class Embed
{
    internal Embed(string title,
                 string description,
                 string url,
                 DateTimeOffset? timestamp,
                 Color? color,
                 EmbedFooter footer,
                 EmbedImage image,
                 EmbedThumbnail thumbnail,
                 EmbedAuthor author,
                 List<EmbedField> fields)
    {
        Title = title;
        Description = description;
        Url = url;
        Timestamp = timestamp;
        Color = color;
        Footer = footer;
        Image = image;
        Thumbnail = thumbnail;
        Author = author;
        Fields = fields;
    }

    [JsonPropertyName("title")]
    public string? Title { get; }
    [JsonPropertyName("description")]
    public string? Description { get; }
    [JsonPropertyName("url")]
    public string? Url { get; }
    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; }
    [JsonPropertyName("color")]
    public Color? Color { get; }
    [JsonPropertyName("footer")]
    public EmbedFooter? Footer { get; }
    [JsonPropertyName("image")]
    public EmbedImage? Image { get; }
    [JsonPropertyName("thumbnail")]
    public EmbedThumbnail? Thumbnail { get; }
    [JsonPropertyName("author")]
    public EmbedAuthor? Author { get; }
    [JsonPropertyName("fields")]
    public List<EmbedField> Fields { get; }
}

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public class EmbedFooter
{
    [JsonPropertyName("text")]
    public string Text { get; init; }
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; init; }
}

public abstract class EmbedPartBase
{
    [JsonPropertyName("url")]
    public string? Url { get; }

    internal EmbedPartBase(string url)
    {
        Url = url;
    }
}

public class EmbedImage : EmbedPartBase
{
    internal EmbedImage(string url) : base(url)
    {
    }
}

public class EmbedThumbnail : EmbedPartBase
{
    internal EmbedThumbnail(string url) : base(url)
    {
    }
}

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public class EmbedProvider
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    [JsonPropertyName("url")]
    public string? Url { get; init; }
}

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public class EmbedAuthor
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; init; }
}

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public class EmbedField
{
    [JsonPropertyName("name")]
    public string Title
    {
        get => _title;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                _title = "­";
            else
                _title = value;
        }
    }

    private string _title = "­";

    [JsonPropertyName("value")]
    public string Description
    {
        get => _description;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                _description = "­";
            else
                _description = value;
        }
    }

    private string _description = "­";

    [JsonPropertyName("inline")]
    public bool? Inline { get; init; }
}