using NetCord.JsonModels;

namespace NetCord;

public class MessagePollResults(JsonMessagePollResults jsonModel) : IJsonModel<JsonMessagePollResults>
{
    JsonMessagePollResults IJsonModel<JsonMessagePollResults>.JsonModel => jsonModel;

    public bool IsFinalized => jsonModel.IsFinalized;
    public IReadOnlyList<MessagePollAnswerCount> Answers { get; } = jsonModel.Answers.Select(x => new MessagePollAnswerCount(x)).ToArray();
}
