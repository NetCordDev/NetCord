using NetCord.JsonModels;

namespace NetCord;

public class MessagePollAnswer(JsonMessagePollAnswer jsonModel) : IJsonModel<JsonMessagePollAnswer>
{
    JsonMessagePollAnswer IJsonModel<JsonMessagePollAnswer>.JsonModel => jsonModel;

    public int AnswerId => jsonModel.AnswerId;
    public MessagePollMedia PollMedia { get; } = new(jsonModel.PollMedia);
}
