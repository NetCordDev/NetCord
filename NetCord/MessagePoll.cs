using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a poll within a message.
/// </summary>
public class MessagePoll : IJsonModel<JsonMessagePoll>
{
    JsonMessagePoll IJsonModel<JsonMessagePoll>.JsonModel => _jsonModel;
    private readonly JsonMessagePoll _jsonModel;

    public MessagePoll(JsonMessagePoll jsonModel)
    {
        _jsonModel = jsonModel;

        Question = new(jsonModel.Question);
        Answers = jsonModel.Answers.Select(a => new MessagePollAnswer(a)).ToArray();

        var results = jsonModel.Results;
        if (results is not null)
            Results = new(results);
    }

    /// <summary>
    /// The question displayed in the poll.
    /// </summary>
    public MessagePollMedia Question { get; }

    /// <summary>
    /// The set of answers available in the poll.
    /// </summary>
    public IReadOnlyList<MessagePollAnswer> Answers { get; }

    /// <summary>
    /// A timestamp specifying the poll's expiry date.
    /// </summary>
    public DateTimeOffset? ExpiresAt => _jsonModel.ExpiresAt;

    /// <summary>
    /// Whether a user can submit multiple answers to the poll.
    /// </summary>
    public bool AllowMultiselect => _jsonModel.AllowMultiselect;

    /// <summary>
    /// The poll's displayed layout type.
    /// </summary>
    public MessagePollLayoutType LayoutType => _jsonModel.LayoutType;

    /// <summary>
    /// The poll's results.
    /// </summary>
    public MessagePollResults? Results { get; }
}
