using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a poll within a message.
/// </summary>
public class MessagePoll(JsonMessagePoll jsonModel) : IJsonModel<JsonMessagePoll>
{
    JsonMessagePoll IJsonModel<JsonMessagePoll>.JsonModel => jsonModel;

    /// <summary>
    /// The question displayed in the poll.
    /// </summary>
    public MessagePollMedia Question { get; } = new(jsonModel.Question);

    /// <summary>
    /// The set of answers available in the poll.
    /// </summary>
    public IReadOnlyList<MessagePollAnswer> Answers { get; } = jsonModel.Answers.Select(a => new MessagePollAnswer(a)).ToArray();

    /// <summary>
    /// A timestamp specifying the poll's expiry date.
    /// </summary>
    public DateTimeOffset? ExpiresAt => jsonModel.ExpiresAt;

    /// <summary>
    /// Whether a user can submit multiple answers to the poll.
    /// </summary>
    public bool AllowMultiselect => jsonModel.AllowMultiselect;

    /// <summary>
    /// The poll's displayed layout type.
    /// </summary>
    public MessagePollLayoutType LayoutType => jsonModel.LayoutType;

    /// <summary>
    /// The poll's results.
    /// </summary>
    public MessagePollResults? Results { get; } = jsonModel.Results is { } results ? new(results) : null;
}
