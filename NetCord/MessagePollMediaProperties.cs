using System.Text.Json.Serialization;

namespace NetCord;

[GenerateMethodsForProperties]
public partial class MessagePollMediaProperties
{
    /// <summary>
    /// The text of the poll media.
    /// </summary>
    /// <remarks>
    /// This value should currently be non-null for both poll questions and poll answers.
    /// Discord may support other forms of poll media in the future, which may not require text.
    /// </remarks>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>
    /// The emoji of the poll media.
    /// </summary>
    /// <remarks>
    /// This may be specified for poll answers. Poll questions currently only support <see cref="Text"/>.
    /// </remarks>
    [JsonPropertyName("emoji")]
    public EmojiProperties? Emoji { get; set; }
}
