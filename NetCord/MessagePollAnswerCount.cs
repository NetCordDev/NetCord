using NetCord.JsonModels;

namespace NetCord;

public partial class MessagePollAnswerCount(JsonMessagePollAnswerCount jsonModel) : IJsonModel<JsonMessagePollAnswerCount>
{
    public JsonMessagePollAnswerCount JsonModel { get; } = jsonModel;
    public ulong AnswerId => JsonModel.Id;
    public int Count => JsonModel.Count;
    public bool MeVoted => JsonModel.MeVoted;
}
