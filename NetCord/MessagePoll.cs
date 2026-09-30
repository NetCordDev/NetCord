using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a poll within a message.
/// </summary>
public class MessagePoll(JsonMessagePoll jsonModel)
{
    /// <summary>
    /// The question displayed in the poll.
    /// </summary>
    public MessagePollMedia Question { get; } = new(jsonModel.Question);

    /// <summary>
    /// The set of answers available in the poll.
    /// </summary>
    public IReadOnlyList<MessagePollAnswer> Answers { get; } = [.. jsonModel.Answers.Select(a => new MessagePollAnswer(a))];

    /// <summary>
    /// A timestamp specifying the poll's expiry date.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; } = jsonModel.ExpiresAt;

    /// <summary>
    /// Whether a user can submit multiple answers to the poll.
    /// </summary>
    public bool AllowMultiselect { get; } = jsonModel.AllowMultiselect;

    /// <summary>
    /// The poll's displayed layout type.
    /// </summary>
    public MessagePollLayoutType LayoutType { get; } = jsonModel.LayoutType;

    /// <summary>
    /// The poll's results.
    /// </summary>
    public MessagePollResults? Results { get; } = jsonModel.Results is { } results ? new(results) : null;
}

/// <summary>
/// Represents the media displayed within a poll.
/// </summary>
public class MessagePollMedia(JsonMessagePollMedia jsonModel)
{
    /// <summary>
    /// The text to display in the poll, up to 300 characters.
    /// </summary>
    public string? Text { get; } = jsonModel.Text;

    /// <summary>
    /// The emoji to display in the poll. Only supported for answers.
    /// </summary>
    public EmojiReference? Emoji { get; } = jsonModel.Emoji is { } emoji ? new(emoji) : null;
}

/// <summary>
/// Represents an answer choice within a poll.
/// </summary>
/// <param name="jsonModel"></param>
public class MessagePollAnswer(JsonMessagePollAnswer jsonModel)
{
    /// <summary>
    /// The answer's ID.
    /// </summary>
    public int AnswerId { get; } = jsonModel.AnswerId;

    /// <summary>
    /// The answer's displayed contents.
    /// </summary>
    public MessagePollMedia PollMedia { get; } = new(jsonModel.PollMedia);
}

/// <summary>
/// Represents the results of a poll.
/// </summary>
/// <param name="jsonModel"></param>
public class MessagePollResults(JsonMessagePollResults jsonModel)
{
    /// <summary>
    /// If <see langword="true"/>, the counts provided in <see cref="Answers"/> are accurately tallied, otherwise small deviations can occur.
    /// </summary>
    public bool IsFinalized { get; } = jsonModel.IsFinalized;

    /// <summary>
    /// A list of vote counts for each answer. If an answer is not included, it held no votes.
    /// </summary>
    public IReadOnlyList<MessagePollAnswerCount> Answers { get; } = [.. jsonModel.Answers.Select(x => new MessagePollAnswerCount(x))];
}

/// <summary>
/// Specifies a poll's displayed layout type.
/// </summary>
public enum MessagePollLayoutType : byte
{
    /// <summary>
    /// The default poll layout.
    /// </summary>
    Default = 1,
}

/// <summary>
/// Represents a count of votes for a poll answer.
/// </summary>
public class MessagePollAnswerCount(JsonMessagePollAnswerCount jsonModel)
{
    /// <summary>
    /// The ID of the count's corresponding answer.
    /// </summary>
    public int AnswerId { get; } = jsonModel.AnswerId;

    /// <summary>
    /// The count of votes for the answer.
    /// </summary>
    public int Count { get; } = jsonModel.Count;

    /// <summary>
    /// Whether the current user has also voted for the answer.
    /// </summary>
    public bool MeVoted { get; } = jsonModel.MeVoted;
}
