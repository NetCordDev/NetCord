using System.Text.Json.Serialization;

namespace NetCord;

/// <summary>
/// 
/// </summary>
/// <param name="question">The question of the poll.</param>
/// <param name="answers">Each of the answers available in the poll, up to 10.</param>
public partial class MessagePollProperties(MessagePollMediaProperties question, IEnumerable<MessagePollAnswerProperties> answers)
{
    /// <summary>
    /// The question of the poll.
    /// </summary>
    [JsonPropertyName("question")]
    public MessagePollMediaProperties Question { get; set; } = question;

    /// <summary>
    /// Each of the answers available in the poll, up to 10.
    /// </summary>
    [JsonPropertyName("answers")]
    public IEnumerable<MessagePollAnswerProperties> Answers { get; set; } = answers;

    /// <summary>
    /// Number of hours the poll should be open for, up to 32 days. Defaults to 24.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("duration")]
    public int? DurationInHours { get; set; }

    /// <summary>
    /// Whether a user can select multiple answers. Defaults to <see langword="false"/>.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("allow_multiselect")]
    public bool AllowMultiselect { get; set; }

    /// <summary>
    /// The layout of the poll.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("layout_type")]
    public MessagePollLayoutType? LayoutType { get; set; }
}
