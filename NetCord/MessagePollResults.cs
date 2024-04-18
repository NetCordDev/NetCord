using NetCord.JsonModels;

namespace NetCord;

public partial class MessagePollResults(JsonMessagePollResults jsonModel) : IJsonModel<JsonMessagePollResults>
{
    JsonMessagePollResults IJsonModel<JsonMessagePollResults>.JsonModel => jsonModel;
    public bool IsFinalized => jsonModel.IsFinalized;
    public MessagePollAnswerCount[]? Answers => jsonModel.Answers;
    public bool ContainsAnswers => Answers != null;
}
