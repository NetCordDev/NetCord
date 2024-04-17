using NetCord.JsonModels;

namespace NetCord;

public partial class MessagePollAnswerCount(JsonMessagePollAnswerCount jsonModel) : IJsonModel<JsonMessagePollAnswerCount>
{
    public JsonMessagePollAnswerCount JsonModel { get; } = jsonModel;
    public ulong AnswerId { get; } = jsonModel.Id;
    public int Count { get; } = jsonModel.Count;
    public bool MeVoted { get; } = jsonModel.MeVoted;
}
