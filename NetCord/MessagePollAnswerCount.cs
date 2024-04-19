using NetCord.JsonModels;

namespace NetCord;

public class MessagePollAnswerCount(JsonMessagePollAnswerCount jsonModel) : IJsonModel<JsonMessagePollAnswerCount>
{
    JsonMessagePollAnswerCount IJsonModel<JsonMessagePollAnswerCount>.JsonModel => jsonModel;
    public int AnswerId => jsonModel.AnswerId;
    public int Count => jsonModel.Count;
    public bool MeVoted => jsonModel.MeVoted;
}
