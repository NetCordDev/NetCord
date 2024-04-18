using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class MessagePollAnswer(JsonMessagePollAnswer jsonModel, ulong guildId, RestClient client) : IJsonModel<JsonMessagePollAnswer>
{
    JsonMessagePollAnswer IJsonModel<JsonMessagePollAnswer>.JsonModel => jsonModel;
    public ulong AnswerId => jsonModel.AnswerId;
    public MessagePollMedia PollMedia { get; } = new(jsonModel.PollMedia, guildId, client);
}
