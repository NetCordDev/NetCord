using NetCord.JsonModels;

namespace NetCord;

public class MessagePollAnswerCount(JsonMessagePollAnswerCount jsonModel) : IJsonModel<JsonMessagePollAnswerCount>
{
    JsonMessagePollAnswerCount IJsonModel<JsonMessagePollAnswerCount>.JsonModel => jsonModel;
    public ulong AnswerId => jsonModel.Id;
    public int Count => jsonModel.Count;
    public bool MeVoted => jsonModel.MeVoted;
}
