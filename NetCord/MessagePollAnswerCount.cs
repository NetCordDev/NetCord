using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents a count of votes for a poll answer.
/// </summary>
public class MessagePollAnswerCount(JsonMessagePollAnswerCount jsonModel) : IJsonModel<JsonMessagePollAnswerCount>
{
    JsonMessagePollAnswerCount IJsonModel<JsonMessagePollAnswerCount>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the count's corresponding answer.
    /// </summary>
    public int AnswerId => jsonModel.AnswerId;

    /// <summary>
    /// The count of votes for the answer.
    /// </summary>
    public int Count => jsonModel.Count;

    /// <summary>
    /// Whether the current user has also voted for the answer.
    /// </summary>
    public bool MeVoted => jsonModel.MeVoted;
}
