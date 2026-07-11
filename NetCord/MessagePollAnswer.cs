using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents an answer choice within a poll.
/// </summary>
/// <param name="jsonModel"></param>
public class MessagePollAnswer(JsonMessagePollAnswer jsonModel) : IJsonModel<JsonMessagePollAnswer>
{
    JsonMessagePollAnswer IJsonModel<JsonMessagePollAnswer>.JsonModel => jsonModel;

    /// <summary>
    /// The answer's ID.
    /// </summary>
    public int AnswerId => jsonModel.AnswerId;

    /// <summary>
    /// The answer's displayed contents.
    /// </summary>
    public MessagePollMedia PollMedia { get; } = new(jsonModel.PollMedia);
}
