using System.Diagnostics.CodeAnalysis;

using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class MessagePollAnswer : IJsonModel<JsonMessagePollAnswer>
{
    public JsonMessagePollAnswer JsonModel { get; }
    public ulong AnswerId { get; }
    public MessagePollMedia PollMedia { get; }

    [SetsRequiredMembers]
    public MessagePollAnswer(JsonMessagePollAnswer jsonModel, ulong guildId, RestClient client)
    {
        JsonModel = jsonModel;
        AnswerId = jsonModel.AnswerId;
        PollMedia = new(jsonModel.PollMedia, guildId, client);
    }
}
