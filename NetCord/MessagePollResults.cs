using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents the results of a poll.
/// </summary>
/// <param name="jsonModel"></param>
public class MessagePollResults(JsonMessagePollResults jsonModel) : IJsonModel<JsonMessagePollResults>
{
    JsonMessagePollResults IJsonModel<JsonMessagePollResults>.JsonModel => jsonModel;

    /// <summary>
    /// If <see langword="true"/>, the counts provided in <see cref="Answers"/> are accurately tallied, otherwise small deviations can occur.
    /// </summary>
    public bool IsFinalized => jsonModel.IsFinalized;

    /// <summary>
    /// A list of vote counts for each answer. If an answer is not included, it held no votes.
    /// </summary>
    public IReadOnlyList<MessagePollAnswerCount> Answers { get; } = jsonModel.Answers.Select(x => new MessagePollAnswerCount(x)).ToArray();
}
