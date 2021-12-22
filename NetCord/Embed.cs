using System.Text.Json.Serialization;

namespace NetCord;

public class EmbedBuilder
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public Color Color { get; set; }
    public EmbedFooter? Footer { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public EmbedAuthor? Author { get; set; }
    public List<EmbedField>? Fields { get; set; }

    public Embed Build()
        => new(Title, Description, Url, Timestamp, Color, Footer, ImageUrl == null ? null : new(ImageUrl), ThumbnailUrl == null ? null : new(ThumbnailUrl), Author, Fields);
}

public class Embed
{
    internal Embed(string? title,
                 string? description,
                 string? url,
                 DateTimeOffset? timestamp,
                 Color color,
                 EmbedFooter? footer,
                 EmbedImage? image,
                 EmbedThumbnail? thumbnail,
                 EmbedAuthor? author,
                 List<EmbedField>? fields)
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

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("title")]
    public string? Title { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("description")]
    public string? Description { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("color")]
    public Color Color { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("footer")]
    public EmbedFooter? Footer { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("image")]
    public EmbedImage? Image { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("thumbnail")]
    public EmbedThumbnail? Thumbnail { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("author")]
    public EmbedAuthor? Author { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("fields")]
    public IReadOnlyList<EmbedField>? Fields { get; }
}

public class EmbedFooter
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; init; }
}

public abstract class EmbedPartBase
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; }

    private protected EmbedPartBase(string? url)
    {
        Url = url;
    }
}

public class EmbedImage : EmbedPartBase
{
    internal EmbedImage(string? url) : base(url)
    {
    }
}

public class EmbedThumbnail : EmbedPartBase
{
    internal EmbedThumbnail(string? url) : base(url)
    {
    }
}

public class EmbedProvider
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; init; }
}

public class EmbedAuthor
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; init; }
}

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

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("inline")]
    public bool Inline { get; init; }
}