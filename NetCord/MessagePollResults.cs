using NetCord.JsonModels;

namespace NetCord;

public partial class MessagePollResults : IJsonModel<JsonMessagePollResults>
{
    public JsonMessagePollResults JsonModel { get; }
    public bool IsFinalized { get; }
    public MessagePollAnswerCount[]? Answers { get; }
    public bool ContainsAnswers => Answers != null;

    public MessagePollResults(JsonMessagePollResults jsonModel)
    {
        JsonModel = jsonModel;
        IsFinalized = jsonModel.IsFinalized;
        Answers = jsonModel.Answers;
    }
}
